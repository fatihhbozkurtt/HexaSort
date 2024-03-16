using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public bool isOnAction;
    [Header("References")]
    public Transform BlockParent;

    [Header("Debug")]
    public bool isOccupied;
    [SerializeField] Vector2 _coordinates = Vector2.zero;
    public List<CellController> neighbors = new List<CellController>();

    [Header("Hexagons Related")]
    [SerializeField] ColorInfo.ColorEnum lastHexColor;
    [SerializeField] List<HexagonController> hexagons = new List<HexagonController>();
    [SerializeField] HexSortWrapper hexSorter = new();
    ColorInfo.ColorEnum firstColor;
    CellController cellController;
    private const float VERTICAL_PLACEMENT_OFFSET = 0.2f;

    IEnumerator Start()
    {
        yield return null;

        SetNeighbours(GridManager.instance.FindNeighbors((int)_coordinates.x, (int)_coordinates.y));

        SetHexagonLists();

        cellController = transform.GetComponent<CellController>();
        InputManager.instance.CheckPossibleMovesEvent += OnCheckPossibleMoves;
    }

   

    private void OnCheckPossibleMoves()
    {
        if (hexagons.Count < 1) return;

        if (IsPure()) return; // ben root cell im, destination benim

        if (hexagons.Count > 0) // Before every check last color should be updated
            lastHexColor = GetLastColor();

        List<CellController> neighbours = new();
        neighbours.AddRange(cellController.neighbors);
        bool breakLoop = false;

        if (neighbours.Count == 0) Debug.LogWarning("No neighbour: " + neighbours.Count);

        for (int i = 0; i < neighbours.Count; i++)
        {
            ColorInfo.ColorEnum neighbourLastColor = neighbours[i].GetLastColor();

            if (neighbourLastColor == lastHexColor)
            {
                CellController targetNeighbor = neighbours[i];

                SendHexagonToNeighbor(targetNeighbor, GetLastHexagon()); // Send the hexagon to the neighbor
                breakLoop = true;
                // köprü hareketi için buraya eklemeler yapýlmalý
                return;
            }

            if (breakLoop) break;
        }
    }

    bool IsPure()
    {
        bool isPure = true;

        if (hexagons.Count > 1) // 1 den fazla eleman var
        {
            firstColor = hexagons[0].GetColor();

            for (int i = 0; i < hexagons.Count; i++)
            {
                if (firstColor != hexagons[i].GetColor())
                    return isPure = false;
            }
        }
        else
        {
            isPure = false;
        }
        return isPure;
    }

    public void UpdateHexagonsList(List<HexagonController> hexes)
    {
        hexagons.Clear();
        hexagons.AddRange(hexes);

        lastHexColor = hexagons[hexagons.Count - 1].GetColor();
    }

    // Check if the top hexagon of this cell's stack can be sent to the neighbor
    public void CheckAndSendHexagonToNeighbor(CellController neighbor)
    {
        if (hexagons.Count > 0 && neighbor != null && neighbor.hexagons.Count > 0)
        {
            HexagonController myHexagon = hexagons[hexagons.Count - 1]; // Get the top hexagon from this cell's stack
            HexagonController neighborHexagon = neighbor.hexagons[neighbor.hexagons.Count - 1]; // Get the top hexagon from the neighbor's stack

            if (myHexagon.GetColor() == neighborHexagon.GetColor()) // Check if the colors match
            {
                SendHexagonToNeighbor(neighbor, myHexagon); // Send the hexagon to the neighbor
            }
        }
    }

    // Send the given hexagon to the neighbor cell
    private void SendHexagonToNeighbor(CellController neighbor, HexagonController hexagon)
    {
        hexagons.RemoveAt(hexagons.Count - 1); // Remove the hexagon from this cell's stack
        neighbor.hexagons.Add(hexagon); // Add the hexagon to the neighbor's stack

        hexagon.GoOtherCell(neighbor);
        isOccupied = hexagons.Count > 0 ? true : false;
        // Update visuals or animations to reflect the hexagon moving from this cell to the neighbor
    }

    private void RecieveHexagonsFromNeighbor()
    {

    }

    #region Getters / Setters
    public ColorInfo.ColorEnum GetLastColor()
    {
        ColorInfo.ColorEnum lastHexColor =
            hexagons.Count > 0 ?
            GetLastHexagon().GetColor() :
            ColorInfo.ColorEnum.None;
        return lastHexColor;
    }
    public Vector2 GetCoordinates()
    {
        return _coordinates;
    }
    public Vector3 GetCenter()
    {
        Vector3 centerPos = new Vector3(transform.position.x, transform.position.y + .2f, transform.position.z);
        return centerPos;
    }
    public Vector3 GetVerticalPosForHex()
    {
        float verticalOffset = (hexagons.Count - 1) * VERTICAL_PLACEMENT_OFFSET;
        Vector3 pos = new Vector3(0, verticalOffset, 0);

        return GetCenter() + pos;
    }

    HexagonController GetLastHexagon()
    {
        if (hexagons.Count == 0)
            return null;
        else
            return hexagons[hexagons.Count - 1];
    }

    // SETTERS
    private void SetHexagonLists()
    {
        foreach (Transform hex in BlockParent)
        {
            HexagonController hexagonController = hex.GetComponent<HexagonController>();
            hexagons.Add(hexagonController);
        }
        for (int i = 0; i < hexagons.Count; i++)
        {
            HexagonController hex = hexagons[i];
            //            if(hex.GetColor() == ColorInfo.ColorEnum.RED)

            switch (hex.GetColor())
            {
                case ColorInfo.ColorEnum.RED
                    :
                    hexSorter.Red.Add(hex);
                    break;
                case ColorInfo.ColorEnum.YELLOW
                    :
                    hexSorter.Yellow.Add(hex);
                    break;
                case ColorInfo.ColorEnum.BLUE
                    :
                    hexSorter.Blue.Add(hex);
                    break;
                case ColorInfo.ColorEnum.GREEN
                    :
                    hexSorter.Green.Add(hex);
                    break;
                case ColorInfo.ColorEnum.PURPLE
                    :
                    hexSorter.Purple.Add(hex);
                    break;

            }
        }
    }
    public void SetCoordinates(float x, float y)
    {
        _coordinates.x = x;
        _coordinates.y = y;
    }

    public void SetOccupied(bool state)
    {
        isOccupied = state;
    }
    void SetNeighbours(List<CellController> _neighbors)
    {
        neighbors.AddRange(_neighbors);
    }


    #endregion
}

[Serializable]
public class HexSortWrapper
{
    public List<HexagonController> Red;
    public List<HexagonController> Yellow;
    public List<HexagonController> Blue;
    public List<HexagonController> Green;
    public List<HexagonController> Purple;
}