using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSizeX = 2;
    public int gridSizeY = 2;
    public float cellSize = 0.5f;
    public Color gridColor = Color.white;
    public float lineWidth = 0.02f;
    public Vector3 gridOffset = Vector3.zero;

    [Header("Control")]
    public bool gridEnabled = true;

    [Header("Renderer Settings")]
    public string sortingLayerName = "UI";
    public int sortingOrder = -1;

    [Header("Highlight")]
    public GameObject highlightPrefab;
    private GameObject highlightInstance;

    private GameObject gridParent;



    void Start()
    {
        GenerateGrid();
        ApplyGridState();
        ApplyOffset();
        CreateHighlightCell();
    }

    void Update()
    {
        ApplyGridState();
        ApplyOffset();
    }
    private void CreateHighlightCell()
    {
        highlightInstance = Instantiate(highlightPrefab, transform);
        highlightInstance.transform.localScale = new Vector3(cellSize, cellSize, 1f);
        highlightInstance.SetActive(false);
    }
    private void ApplyGridState()
    {
        if (gridParent != null)
            gridParent.SetActive(gridEnabled);
    }

    private void ApplyOffset()
    {
        if (gridParent != null)
            gridParent.transform.localPosition = gridOffset;
    }

    private void GenerateGrid()
    {
        gridParent = new GameObject("IsometricGrid");
        gridParent.transform.parent = transform;
        gridParent.transform.localPosition = gridOffset;

        for (int i = 0; i <= gridSizeX; i++)
        {
            Vector3 start = ToIsometric(new Vector3(i * cellSize, 0, 0));
            Vector3 end = ToIsometric(new Vector3(i * cellSize, 0, gridSizeY * cellSize));
            CreateLine(start, end);
        }

        for (int j = 0; j <= gridSizeY; j++)
        {
            Vector3 start = ToIsometric(new Vector3(0, 0, j * cellSize));
            Vector3 end = ToIsometric(new Vector3(gridSizeX * cellSize, 0, j * cellSize));
            CreateLine(start, end);
        }
    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = gridParent.transform;
        lineObj.transform.localPosition = Vector3.zero;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = gridColor;
        lr.endColor = gridColor;
        lr.useWorldSpace = false;

        lr.sortingLayerName = sortingLayerName;
        lr.sortingOrder = sortingOrder;
    }

    private Vector3 ToIsometric(Vector3 point)
    {
        float isoX = point.x - point.z;
        float isoY = (point.x + point.z) * 0.5f;
        return new Vector3(isoX, isoY, 0f);
    }

    public void ToggleGrid(bool enabled)
    {
        gridEnabled = enabled;
        ApplyGridState();
    }

    public void SetGridOffset(Vector3 offset)
    {
        gridOffset = offset;
        ApplyOffset();
    }
    public void HighlightCell(int x, int y)
    {
        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
        {
            highlightInstance.SetActive(false);
            return;
        }

        Vector3 pos = ToIsometric(new Vector3((x + 0.5f) * cellSize, 0, (y + 0.5f) * cellSize));
        highlightInstance.transform.localPosition = pos + gridOffset;
        highlightInstance.SetActive(true);
    }
}
