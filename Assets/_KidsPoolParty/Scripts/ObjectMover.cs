using System.Collections;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject tiltObject; 
    public float maxTiltAngle = 15f;
    public float tiltSpeed = 5f;
    
    public GameObject selectedObject;
    private Vector3 offset;
    private Plane dragPlane;
    private Vector3 lastMousePosition;
    public Quaternion originalRotation;

    // Evitar reproducir sonido varias veces por arrastre
    private bool hasPlayedWaterSound = false;

    // Cacheo de la cámara principal
    private Camera mainCamera;

    // Array preasignado para evitar asignaciones en Physics.BoxCast
    private RaycastHit[] boxCastResults = new RaycastHit[10];
    private int movableLayerMask; // Suponiendo que los objetos "Movable" estén en esta capa

    // Cacheo de propiedades del grid para no acceder constantemente a gridManager
    private float cellSize;
    private int gridRows;
    private int gridCols;

    private void Start()
    {
        mainCamera = Camera.main;
        tiltObject = this.gameObject;
        if (tiltObject != null)
        {
            originalRotation = tiltObject.transform.rotation;
        }
        // Inicializar layer mask (asegúrate de que exista la capa "Movable")
        movableLayerMask = LayerMask.GetMask("Movable");

        // Cachear propiedades del grid (suponiendo que no cambian en tiempo de ejecución)
        if (gridManager != null)
        {
            cellSize = gridManager.cellSize;
            gridRows = gridManager.rows;
            gridCols = gridManager.cols;
        }
    }

    void Update()
    {
        if (!enabled) return;

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
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // Se utiliza el layer mask para limitar el raycast
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, movableLayerMask))
        {
            if (hit.collider.CompareTag("Movable"))
            {
                selectedObject = hit.collider.gameObject;
                // Si se espera que sólo se mueva este objeto, se puede omitir esta comprobación
                if (selectedObject != this.gameObject)
                    return;
                
                dragPlane = new Plane(Vector3.up, hit.point);
                offset = selectedObject.transform.position - hit.point;
                lastMousePosition = Input.mousePosition;

                originalRotation = tiltObject.transform.rotation;
                hasPlayedWaterSound = false;
                ClearOccupiedCells(selectedObject);
            }
        }
    }

    void OnMouseDrag()
    {
        // Se asegura que sólo se procese el movimiento del objeto deseado
        if (selectedObject != this.gameObject)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float distance))
        {
            // Calcular la posición destino sin restricciones
            Vector3 rawTargetPosition = ray.GetPoint(distance) + offset;
            Transform selectedTransform = selectedObject.transform;
            Vector3 currentPosition = selectedTransform.position;
            Vector3 delta = rawTargetPosition - currentPosition;
            
            // Permitir el movimiento solo en el eje con mayor diferencia (X o Z)
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
            {
                rawTargetPosition.z = currentPosition.z;  // Movimiento solo en X
            }
            else
            {
                rawTargetPosition.x = currentPosition.x;  // Movimiento solo en Z
            }

            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            // Aplicar el efecto de tilt
            float tiltX = Mathf.Clamp(mouseDelta.x * -1, -maxTiltAngle, maxTiltAngle);
            float tiltZ = Mathf.Clamp(mouseDelta.y, -maxTiltAngle, maxTiltAngle);
            Quaternion targetRotation = Quaternion.Euler(tiltZ, 0, tiltX);
            tiltObject.transform.localRotation = Quaternion.Lerp(
                tiltObject.transform.localRotation,
                targetRotation,
                Time.deltaTime * tiltSpeed
            );

            // Restringir el movimiento para que el objeto se mantenga dentro del grid
            Vector3 objSize = selectedTransform.localScale;
            float halfWidth = objSize.x * 0.5f;
            float halfDepth = objSize.z * 0.5f;
            float minX = halfWidth;
            float maxX = gridCols * cellSize - halfWidth;
            float minZ = halfDepth;
            float maxZ = gridRows * cellSize - halfDepth;
            rawTargetPosition.x = Mathf.Clamp(rawTargetPosition.x, minX, maxX);
            rawTargetPosition.z = Mathf.Clamp(rawTargetPosition.z, minZ, maxZ);

            // Movimiento suavizado
            float moveSpeed = 10f;
            Vector3 smoothedPosition = Vector3.Lerp(currentPosition, rawTargetPosition, Time.deltaTime * moveSpeed);
            Vector3 movement = smoothedPosition - currentPosition;
            
            if ((Mathf.Abs(movement.x) > 0.01f || Mathf.Abs(movement.z) > 0.01f) && !hasPlayedWaterSound)
            {
                SoundManager.Instance.PlayWaterSound();
                hasPlayedWaterSound = true;
            }
            
            // Comprobación de colisiones mediante BoxCastNonAlloc
            bool pathBlocked = CheckPathBlocked(currentPosition, smoothedPosition, objSize * 0.5f);
            Vector3 newPosition = currentPosition;

            if (!pathBlocked)
            {
                newPosition = smoothedPosition;
            }
            else
            {
                // Probar movimientos por eje para ver si se puede avanzar parcialmente
                if (Mathf.Abs(movement.x) > 0.01f)
                {
                    Vector3 xTestPosition = currentPosition + new Vector3(movement.x, 0, 0);
                    if (!CheckPathBlocked(currentPosition, xTestPosition, objSize * 0.5f))
                    {
                        newPosition.x = xTestPosition.x;
                    }
                }
                if (Mathf.Abs(movement.z) > 0.01f)
                {
                    Vector3 zTestPosition = newPosition + new Vector3(0, 0, movement.z);
                    if (!CheckPathBlocked(newPosition, zTestPosition, objSize * 0.5f))
                    {
                        newPosition.z = zTestPosition.z;
                    }
                }
            }
            
            selectedTransform.position = new Vector3(newPosition.x, currentPosition.y, newPosition.z);
        }
    }

    void OnMouseUp()
    {
        if (selectedObject != null)
        {
            // Iniciar la corrección suave de la rotación (tilt)
            StartCoroutine(SmoothRotateToOriginal());

            Transform selectedTransform = selectedObject.transform;
            Vector3 currentPosition = selectedTransform.position;
            Vector3 scale = selectedTransform.localScale;
            int objWidth = Mathf.CeilToInt(scale.x / cellSize);
            int objHeight = Mathf.CeilToInt(scale.z / cellSize);

            Vector2Int gridPosition = CalculateGridPosition(currentPosition, objWidth, objHeight);

            if (IsPlacementValid(gridPosition, objWidth, objHeight))
            {
                Vector3 alignedPosition = AlignToGrid(gridPosition, objWidth, objHeight);
                selectedTransform.position = alignedPosition;
                MarkOccupiedCells(gridPosition, objWidth, objHeight);
            }
            else
            {
                Debug.Log("No se puede colocar el objeto aquí.");
            }
            
            selectedObject = null;
            hasPlayedWaterSound = false;
        }
    }

    private IEnumerator SmoothRotateToOriginal()
    {
        float elapsedTime = 0f;
        float duration = 0.3f;
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
        
        if (distance < 0.01f)
            return false;

        // Calcular previamente los half extents ajustados
        Vector3 adjustedHalfExtents = halfExtents * 0.9f;
        int hitCount = Physics.BoxCastNonAlloc(
            start,
            adjustedHalfExtents,
            direction.normalized,
            boxCastResults,
            Quaternion.identity,
            distance,
            ~LayerMask.GetMask("Ignore Raycast")
        );

        for (int i = 0; i < hitCount; i++)
        {
            if (boxCastResults[i].collider.gameObject != selectedObject && !boxCastResults[i].collider.isTrigger)
            {
                return true;
            }
        }
        return false;
    }

    bool IsPlacementValid(Vector2Int gridPos, int width, int height)
    {
        if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x + height > gridRows || gridPos.y + width > gridCols)
        {
            return false;
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
            int objWidth = Mathf.CeilToInt(scale.x / cellSize);
            int objHeight = Mathf.CeilToInt(scale.z / cellSize);
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
        int col = Mathf.FloorToInt((worldPosition.x - (width - 1) * cellSize / 2) / cellSize);
        int row = Mathf.FloorToInt((worldPosition.z - (height - 1) * cellSize / 2) / cellSize);
        return new Vector2Int(row, col);
    }

    Vector3 AlignToGrid(Vector2Int gridPos, int width, int height)
    {
        float centerX = (gridPos.y + (float)width / 2) * cellSize;
        float centerZ = (gridPos.x + (float)height / 2) * cellSize;
        return new Vector3(centerX, 0, centerZ);
    }
}
