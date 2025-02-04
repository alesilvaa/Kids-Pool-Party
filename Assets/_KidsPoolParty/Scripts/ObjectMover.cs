using System;
using System.Collections;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public GridManager gridManager;
    private GameObject tiltObject; // Cambiado a private
    public float maxTiltAngle = 15f;
    public float tiltSpeed = 5f;
    
    private GameObject selectedObject;
    private Vector3 offset;
    private Plane dragPlane;
    private Vector3 lastMousePosition;
    private Quaternion originalRotation;

    private void Start()
    {
        //EventsManager.Instance.OnBoxCleared += ReleaseCellsForObject;
        // Asignar el tiltObject como este mismo GameObject
        tiltObject = this.gameObject;
        if (tiltObject != null)
        {
            originalRotation = tiltObject.transform.rotation;
        }
    }
    
    private void OnDestroy()
    {
        //EventsManager.Instance.OnBoxCleared -= ReleaseCellsForObject;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        else if (Input.GetMouseButton(0) && selectedObject != null)
        {
            OnMouseDrag();
        }
        else if (Input.GetMouseButtonUp(0) && selectedObject != null)
        {
            OnMouseUp();
        }
    }

    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Movable"))
            {
                selectedObject = hit.collider.gameObject;
                if (selectedObject != this.gameObject) return;
                
                dragPlane = new Plane(Vector3.up, hit.point);
                offset = selectedObject.transform.position - hit.point;
                lastMousePosition = Input.mousePosition;

                if (tiltObject != null)
                {
                    originalRotation = tiltObject.transform.rotation;
                }

                // Eliminamos la llamada a SmoothMoveToY aquí
                ClearOccupiedCells(selectedObject);
            }
        }
    }

    void OnMouseDrag()
    {
        if (selectedObject != this.gameObject) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float distance))
        {
            // Posición objetivo sin restricciones aún
            Vector3 rawTargetPosition = ray.GetPoint(distance) + offset;
            Vector3 currentPosition = selectedObject.transform.position;

            // Determinar la dirección dominante y bloquear el otro eje:
            Vector3 delta = rawTargetPosition - currentPosition;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
            {
                rawTargetPosition.z = currentPosition.z;  // Mover solo en X
            }
            else
            {
                rawTargetPosition.x = currentPosition.x;  // Mover solo en Z
            }

            // Actualizamos el valor de lastMousePosition y aplicamos inclinación (tilt) si corresponde:
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            if (tiltObject != null)
            {
                float tiltX = Mathf.Clamp(mouseDelta.x * -1, -maxTiltAngle, maxTiltAngle);
                float tiltZ = Mathf.Clamp(mouseDelta.y, -maxTiltAngle, maxTiltAngle);
                Quaternion targetRotation = Quaternion.Euler(tiltZ, 0, tiltX);
                tiltObject.transform.localRotation = Quaternion.Lerp(
                    tiltObject.transform.localRotation,
                    targetRotation,
                    Time.deltaTime * tiltSpeed
                );
            }

            // Calcular límites de la grilla y ajustar la posición destino:
            Vector3 objSize = selectedObject.transform.localScale;
            float halfWidth = objSize.x / 2;
            float halfDepth = objSize.z / 2;
            float minX = halfWidth;
            float maxX = gridManager.cols * gridManager.cellSize - halfWidth;
            float minZ = halfDepth;
            float maxZ = gridManager.rows * gridManager.cellSize - halfDepth;
            rawTargetPosition.x = Mathf.Clamp(rawTargetPosition.x, minX, maxX);
            rawTargetPosition.z = Mathf.Clamp(rawTargetPosition.z, minZ, maxZ);

            // Movimiento suavizado:
            float moveSpeed = 10f;
            Vector3 smoothedPosition = Vector3.Lerp(currentPosition, rawTargetPosition, Time.deltaTime * moveSpeed);
            Vector3 movement = smoothedPosition - currentPosition;

            // Verificar colisiones a lo largo del camino:
            bool pathBlocked = CheckPathBlocked(currentPosition, smoothedPosition, objSize / 2);
            Vector3 newPosition = currentPosition;

            if (!pathBlocked)
            {
                newPosition = smoothedPosition;
            }
            else
            {
                if (Mathf.Abs(movement.x) > 0.01f)
                {
                    Vector3 xTestPosition = currentPosition + new Vector3(movement.x, 0, 0);
                    if (!CheckPathBlocked(currentPosition, xTestPosition, objSize / 2))
                    {
                        newPosition.x = xTestPosition.x;
                    }
                }
                if (Mathf.Abs(movement.z) > 0.01f)
                {
                    Vector3 zTestPosition = newPosition + new Vector3(0, 0, movement.z);
                    if (!CheckPathBlocked(newPosition, zTestPosition, objSize / 2))
                    {
                        newPosition.z = zTestPosition.z;
                    }
                }
            }

            // Actualizamos la posición final sin alterar la Y (se mantiene la altura actual)
            selectedObject.transform.position = new Vector3(newPosition.x, currentPosition.y, newPosition.z);
        }
    }

    void OnMouseUp()
    {
        if (selectedObject != null)
        {
            // Restaura la rotación original suavemente
            if (tiltObject != null)
            {
                StartCoroutine(SmoothRotateToOriginal());
            }

            Vector3 currentPosition = selectedObject.transform.position;

            Vector3 scale = selectedObject.transform.localScale;
            int objWidth = Mathf.CeilToInt(scale.x / gridManager.cellSize);
            int objHeight = Mathf.CeilToInt(scale.z / gridManager.cellSize);

            Vector2Int gridPosition = CalculateGridPosition(currentPosition, objWidth, objHeight);

            if (IsPlacementValid(gridPosition, objWidth, objHeight))
            {
                Vector3 alignedPosition = AlignToGrid(gridPosition, objWidth, objHeight);
                selectedObject.transform.position = alignedPosition;
                MarkOccupiedCells(gridPosition, objWidth, objHeight);
            }
            else
            {
                Debug.Log("No se puede colocar el objeto aquí.");
            }

            // Se elimina la llamada a SmoothMoveToY para evitar la elevación en Y
            selectedObject = null;
        }
    }

    private IEnumerator SmoothRotateToOriginal()
    {
        float elapsedTime = 0f;
        float duration = 0.3f; // Duración de la animación de retorno
        Quaternion startRotation = tiltObject.transform.rotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            tiltObject.transform.rotation = Quaternion.Lerp(startRotation, originalRotation, t);
            yield return null;
        }

        tiltObject.transform.rotation = originalRotation;
    }

    private bool CheckPathBlocked(Vector3 start, Vector3 end, Vector3 halfExtents)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        
        if (distance < 0.01f) return false;

        // Hacemos un BoxCast para verificar todo el camino
        RaycastHit[] hits = Physics.BoxCastAll(
            start,
            halfExtents * 0.9f, // Reducimos ligeramente el tamaño para evitar falsos positivos
            direction.normalized,
            Quaternion.identity,
            distance,
            ~LayerMask.GetMask("Ignore Raycast") // Ignora la capa "Ignore Raycast"
        );

        foreach (var hit in hits)
        {
            // Ignoramos el objeto seleccionado y los triggers
            if (hit.collider.gameObject != selectedObject && !hit.collider.isTrigger)
            {
                return true; // Camino bloqueado
            }
        }

        return false; // Camino libre
    }

    bool IsPlacementValid(Vector2Int gridPos, int width, int height)
    {
        if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x + height > gridManager.rows || gridPos.y + width > gridManager.cols)
        {
            return false; // El objeto se sale de los límites.
        }

        for (int x = gridPos.x; x < gridPos.x + height; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + width; y++)
            {
                if (!gridManager.IsCellEmpty(x, y))
                {
                    return false;
                }
            }
        }

        return true;
    }

    void ClearOccupiedCells(GameObject obj)
    {
        if (obj != null)
        {
            Vector3 scale = obj.transform.localScale;
            int objWidth = Mathf.CeilToInt(scale.x / gridManager.cellSize);
            int objHeight = Mathf.CeilToInt(scale.z / gridManager.cellSize);
            Vector2Int gridPosition = CalculateGridPosition(obj.transform.position, objWidth, objHeight);

            for (int x = gridPosition.x; x < gridPosition.x + objHeight; x++)
            {
                for (int y = gridPosition.y; y < gridPosition.y + objWidth; y++)
                {
                    gridManager.SetCellOccupied(x, y, false);
                }
            }
        }
    }

    public void ReleaseCellsForObject(GameObject obj)
    {
        ClearOccupiedCells(obj);
    }

    void MarkOccupiedCells(Vector2Int gridPos, int width, int height)
    {
        for (int x = gridPos.x; x < gridPos.x + height; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + width; y++)
            {
                gridManager.SetCellOccupied(x, y, true);
            }
        }
    }

    Vector2Int CalculateGridPosition(Vector3 worldPosition, int width, int height)
    {
        int col = Mathf.FloorToInt((worldPosition.x - (width - 1) * gridManager.cellSize / 2) / gridManager.cellSize);
        int row = Mathf.FloorToInt((worldPosition.z - (height - 1) * gridManager.cellSize / 2) / gridManager.cellSize);
        return new Vector2Int(row, col);
    }

    Vector3 AlignToGrid(Vector2Int gridPos, int width, int height)
    {
        float centerX = (gridPos.y + (float)width / 2) * gridManager.cellSize;
        float centerZ = (gridPos.x + (float)height / 2) * gridManager.cellSize;
        return new Vector3(centerX, 0, centerZ);
    }

    // Se eliminó el método SmoothMoveToY ya que no es necesario
}
