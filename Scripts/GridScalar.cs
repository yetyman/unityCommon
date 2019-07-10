using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Scale a GridLayoutGroup according to resolution, etc.
 * This is using width-constrained layout
 */
public class GridScalar : MonoBehaviour
{

    private GridLayoutGroup grid;
    private RectOffset gridPadding;
    private RectTransform parent;

    public int rows = 6;
    public int cols = 7;
    public float spacing = 10;

    Vector2 lastSize;

    void Start()
    {
        grid = GetComponent<GridLayoutGroup>();
        grid.spacing = new Vector2(spacing, spacing);
        parent = GetComponent<RectTransform>();
        gridPadding = grid.padding;
        lastSize = Vector2.zero;
        UpdateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrid();
    }
    void UpdateGrid()
    {
        if (lastSize == parent.rect.size)
        {
            return;
        }
        lastSize = parent.rect.size;

        var paddingX = gridPadding.left + gridPadding.right;
        var paddingY = gridPadding.top + gridPadding.bottom;
        //var cellSizeW = Mathf.Round((lastSize.x - paddingX - (cols - 1) * spacing) / cols);
        //var cellSizeH = Mathf.Round((lastSize.y - paddingY - (rows - 1) * spacing) / rows);
        var cellSizeW = (lastSize.x - paddingX - (cols - 1) * spacing) / cols;
        var cellSizeH = (lastSize.y - paddingY - (rows - 1) * spacing) / rows;
        grid.cellSize = new Vector2(cellSizeW, cellSizeH);
    }
}