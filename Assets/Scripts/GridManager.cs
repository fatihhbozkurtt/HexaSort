using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    [Header("References")]
    [SerializeField] GameObject CellPrefab;
    [SerializeField] HexagonController hexagonBlockPrefab;
    [SerializeField] Material BlockMaterial;
    [SerializeField] ColorPack colorPack;

    [Header("Configuration")]
    [SerializeField] int _gridSizeX = 0;
    [SerializeField] int _gridSizeY = 0;

    [Header("Debug")]
    private GridClass[,] _gridPlan;
    private const float CELL_HORIZONTAL_OFFSET = 0.75f;
    private const float CELL_VERTICAL_OFFSET = 0.8660254f;
    [SerializeField] List<StartInfo> startInfos;

    public void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        if (_gridPlan != null)
        {
            DestroyPreviousGrid();
        }

        _gridPlan = new GridClass[_gridSizeX, _gridSizeY];
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                _gridPlan[x, y] = new GridClass();

                int index;
                _gridPlan[x, y].PosX = x;
                _gridPlan[x, y].PosY = y;

                if (ContainsInStartInfo(x, y, out index))
                {
                    _gridPlan[x, y].isOpen = startInfos[index].isOpen;
                    _gridPlan[x, y].GridContentList = startInfos[index].ContentInfo;
                }
                else
                {
                    _gridPlan[x, y].isOpen = true;
                    _gridPlan[x, y].GridContentList = new();
                }

                if (_gridPlan[x, y].isOpen)
                {
                    GameObject cloneCellGO = Instantiate(CellPrefab, Vector3.zero, CellPrefab.transform.rotation, transform);
                    cloneCellGO.transform.position = 
                        new Vector3(x * CELL_HORIZONTAL_OFFSET, 0,
                        -(((x % 2) * (CELL_VERTICAL_OFFSET / 2)) + y * CELL_VERTICAL_OFFSET));

                    _gridPlan[x, y].GridObject = cloneCellGO;

                    cloneCellGO.name = x.ToString() + "," + y.ToString();
                    CellController cellController = cloneCellGO.GetComponent<CellController>();
                    cellController.SetCoordinates(x, y);

                }

                if (_gridPlan[x, y].GridContentList.RollerRopeInfoList.Count != 0 && _gridPlan[x, y].isOpen)
                {
                    for (int i = 0; i < _gridPlan[x, y].GridContentList.RollerRopeInfoList.Count; i++)
                    {
                        RollerClass targetRoller = _gridPlan[x, y].GridContentList;
                        int amount = targetRoller.RollerRopeInfoList[i].amount;
                        ColorInfo.ColorEnum color = targetRoller.RollerRopeInfoList[i].ColorEnum;
                        Material mat = new Material(BlockMaterial);
                        mat.color = colorPack.BlockColorInfos[colorPack.GetColorEnumIndex(color)].RopeColor;

                        for (int b = 0; b < amount; b++)
                        {
                            SpawnBlock(b,
                                _gridPlan[x, y].GridObject.transform.position,
                                _gridPlan[x, y].GridObject.GetComponent<CellController>().BlockParent,
                                mat,
                                color);
                        }
                    }
                }
            }
        }
    }

    private void DestroyPreviousGrid()
    {
        for (int x = 0; x < _gridPlan.GetLength(0); x++)
        {
            for (int y = 0; y < _gridPlan.GetLength(1); y++)
            {
                Destroy(_gridPlan[x, y].GridObject);
            }
        }
    }

    void SpawnBlock(int index, Vector3 gridPos, Transform parent, Material mat, ColorInfo.ColorEnum color)
    {
        float verticalOffset = 0.2f;
        float verticalPos = (index + 1) * verticalOffset;
        Vector3 spawnPos = gridPos + new Vector3(0, verticalPos, 0);

        HexagonController cloneBlock = Instantiate(hexagonBlockPrefab, spawnPos, Quaternion.identity, parent);

        cloneBlock.Initialize(color, mat);
    }

    bool ContainsInStartInfo(int x, int y, out int index)
    {
        for (int i = 0; i < startInfos.Count; i++)
        {
            StartInfo startInfo = startInfos[i];
            if (startInfo.Coordinates.x == x && startInfo.Coordinates.y == y)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    /* public List<CellController> FindNeighbors(int x, int y)
     {
         List<CellController> neighbors = new List<CellController>();

         int[] xOffset = y % 2 == 0 ? new int[] { 0, 0, 1, 1, -1, -1 } : new int[] { 0, 0, 1, 1, -1, -1 };
         int[] yOffset = y % 2 == 0 ? new int[] { -1, 1, 0, 1, 0, 1 } : new int[] { -1, 1, -1, 0, -1, 0 };

         for (int i = 0; i < xOffset.Length; i++)
         {
             int newX = x + xOffset[i];
             int newY = y + yOffset[i];

             if (IsValidCell(newX, newY) && _gridPlan[newX, newY].isOpen)
             {
                 neighbors.Add(_gridPlan[newX, newY].GridObject.GetComponent<CellController>());
             }
         }

         return neighbors;
     }*/

    public List<CellController> FindNeighbors(int x, int y)
    {
        List<CellController> neighbors = new List<CellController>();

        if (IsValidCell(x, y - 1) && _gridPlan[x, y - 1].isOpen)
        {
            neighbors.Add(_gridPlan[x, y - 1].GridObject.GetComponent<CellController>());
        }
        if (IsValidCell(x, y + 1) && _gridPlan[x, y + 1].isOpen)
        {

            neighbors.Add(_gridPlan[x, y + 1].GridObject.GetComponent<CellController>());
        }
        if (y % 2 == 0) // Y çift 
        {
            if (x % 2 != 0) // X TEK
            {
                if (IsValidCell(x + 1, y) && _gridPlan[x + 1, y].isOpen)
                {
                    neighbors.Add(_gridPlan[x + 1, y].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x + 1, y + 1) && _gridPlan[x + 1, y + 1].isOpen)
                {
                    neighbors.Add(_gridPlan[x + 1, y + 1].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x - 1, y) && _gridPlan[x - 1, y].isOpen)
                {
                    neighbors.Add(_gridPlan[x - 1, y].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x - 1, y + 1) && _gridPlan[x - 1, y + 1].isOpen)
                {
                    neighbors.Add(_gridPlan[x - 1, y + 1].GridObject.GetComponent<CellController>());
                }
            }
            else
            {
                if (IsValidCell(x + 1, y - 1) && _gridPlan[x + 1, y - 1].isOpen)
                {
                    neighbors.Add(_gridPlan[x + 1, y - 1].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x + 1, y) && _gridPlan[x + 1, y].isOpen)
                {
                    neighbors.Add(_gridPlan[x + 1, y].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x - 1, y - 1) && _gridPlan[x - 1, y - 1].isOpen)
                {
                    neighbors.Add(_gridPlan[x - 1, y - 1].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x - 1, y) && _gridPlan[x - 1, y].isOpen)
                {
                    neighbors.Add(_gridPlan[x - 1, y].GridObject.GetComponent<CellController>());
                }
            }

        }
        else
        {
            if (x % 2 != 0) // X TEK
            {
                if (IsValidCell(x + 1, y) && _gridPlan[x + 1, y].isOpen)
                {
                    neighbors.Add(_gridPlan[x + 1, y].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x + 1, y + 1) && _gridPlan[x + 1, y + 1].isOpen)
                {
                    neighbors.Add(_gridPlan[x + 1, y + 1].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x - 1, y) && _gridPlan[x - 1, y].isOpen)
                {
                    neighbors.Add(_gridPlan[x - 1, y].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x - 1, y + 1) && _gridPlan[x - 1, y + 1].isOpen)
                {
                    neighbors.Add(_gridPlan[x - 1, y + 1].GridObject.GetComponent<CellController>());
                }
            }
            else
            {
                if (IsValidCell(x + 1, y - 1) && _gridPlan[x + 1, y - 1].isOpen)
                {
                    neighbors.Add(_gridPlan[x + 1, y - 1].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x + 1, y) && _gridPlan[x + 1, y].isOpen)
                {
                    neighbors.Add(_gridPlan[x + 1, y].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x - 1, y - 1) && _gridPlan[x - 1, y - 1].isOpen)
                {
                    neighbors.Add(_gridPlan[x - 1, y - 1].GridObject.GetComponent<CellController>());
                }
                if (IsValidCell(x - 1, y) && _gridPlan[x - 1, y].isOpen)
                {
                    neighbors.Add(_gridPlan[x - 1, y].GridObject.GetComponent<CellController>());
                }
            }
        }


        return neighbors;
    }

    bool IsValidCell(int x, int y)
    {
        return x >= 0 && x < _gridSizeX && y >= 0 && y < _gridSizeY;
    }
}
[System.Serializable]
public class StartInfo
{
    public Vector2Int Coordinates;
    public RollerClass ContentInfo;
    public bool isOpen;
}