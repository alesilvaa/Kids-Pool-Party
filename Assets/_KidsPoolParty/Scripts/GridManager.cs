using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int rows = 6;
    public int cols = 6;
    public float cellSize = 1.0f;
    public Material gridLineMaterial; // Material para las líneas de la grilla.
    private bool[,] grid;

    void Start()
    {
        //InitializeGrid();
        grid = new bool[rows, cols];
    }

    public bool IsCellEmpty(int x, int y)
    {
        return x >= 0 && y >= 0 && x < rows && y < cols && !grid[x, y];
    }

    public void SetCellOccupied(int x, int y, bool isOccupied)
    {
        if (x >= 0 && y >= 0 && x < rows && y < cols)
        {
            grid[x, y] = isOccupied;
        }
    }

   
    void InitializeGrid()
    {
        DrawGridLines();
    }

    void DrawGridLines()
    {
        for (int row = 0; row <= rows; row++)
        {
            CreateGridLine(
                new Vector3(0, 0, row * cellSize), 
                new Vector3(cols * cellSize, 0, row * cellSize)
            );
        }

        for (int col = 0; col <= cols; col++)
        {
            CreateGridLine(
                new Vector3(col * cellSize, 0, 0), 
                new Vector3(col * cellSize, 0, rows * cellSize)
            );
        }
    }

    void CreateGridLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject("GridLine");
        line.transform.parent = this.transform;
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = gridLineMaterial;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.useWorldSpace = true;
    }
    
    
    
}