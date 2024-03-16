using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public bool isOnAction;
    [Header("References")]
    public Transform HexStackParent;

    [Header("Debug")]
    public bool isOccupied;
    [SerializeField] Vector2 _coordinates = Vector2.zero;
    public List<CellController> neighbors = new List<CellController>();

    [Header("Hexagons Related")]
    [SerializeField] ColorInfo.ColorEnum lastHexColor;
    [SerializeField] List<HexagonController> hexagons = new List<HexagonController>();
    [SerializeField] List<HexagonController> hexToSendList = new List<HexagonController>();
    HexSortWrapper hexSorter = new();
    ColorInfo.ColorEnum firstColor;
    CellController cellController;
    private const float VERTICAL_PLACEMENT_OFFSET = 0.2f;
    Dictionary<ColorInfo.ColorEnum, List<HexagonController>> colorToHexagonDict = new Dictionary<ColorInfo.ColorEnum, List<HexagonController>>();
    IEnumerator Start()
    {
        yield return null;

        SetNeighbours(GridManager.instance.FindNeighbors((int)_coordinates.x, (int)_coordinates.y));

        SetHexagonLists();
        SeparateHexagonsByColor();

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

        CellController targetNeighbor = null;
        ColorInfo.ColorEnum neighbourLastColor = ColorInfo.ColorEnum.None;

        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbourLastColor = neighbours[i].GetLastColor();

            if (neighbourLastColor == lastHexColor)
            {
                targetNeighbor = neighbours[i];
                StartCoroutine(SendHexagonToNeighbor(hexToSendList, targetNeighbor));
                breakLoop = true;
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
        SeparateHexagonsByColor();

        lastHexColor = hexagons[hexagons.Count - 1].GetColor();
    }

    // Check if the top hexagon of this cell's stack can be sent to the neighbor
    //public void CheckAndSendHexagonToNeighbor(CellController neighbor)
    //{
    //    if (hexagons.Count > 0 && neighbor != null && neighbor.hexagons.Count > 0)
    //    {
    //        HexagonController myHexagon = hexagons[hexagons.Count - 1]; // Get the top hexagon from this cell's stack
    //        HexagonController neighborHexagon = neighbor.hexagons[neighbor.hexagons.Count - 1]; // Get the top hexagon from the neighbor's stack

    //        if (myHexagon.GetColor() == neighborHexagon.GetColor()) // Check if the colors match
    //        {
    //            SendHexagonToNeighbor(neighbor, myHexagon); // Send the hexagon to the neighbor
    //        }
    //    }
    //}

    #region Send & Receive Hexagons
    private void SendHexagonToNeighbor(CellController neighbor, HexagonController hexagon)
    {
        HexagonController removedHex = GetLastHexagon();
        ColorInfo.ColorEnum hexColor = GetLastHexagon().GetColor();
        hexagons.Remove(removedHex);
        colorToHexagonDict[hexColor].Remove(removedHex);
        neighbor.hexagons.Add(hexagon);
    }


    // Send the given hexagon to the neighbor cell
    private IEnumerator SendHexagonToNeighbor(List<HexagonController> hexToSendList, CellController neighbor)
    {
        for (int i = hexagons.Count - 1; i >= 0; i--)
        {
            HexagonController hex = hexagons[i];

            if (hex.GetColor() == neighbor.GetLastColor())
                hexToSendList.Add(hex);
            else
                break;
            
        }

        yield return new WaitUntil(() => hexToSendList.Count > 0);

        Debug.Log("BOK, " + hexToSendList.Count);

        for (int i = 0; i < hexToSendList.Count; i++)
        {
            HexagonController hex = hexToSendList[i];

            //if (GetLastHexagon() != colorToHexagonDict[hex.GetColor()][colorToHexagonDict[hex.GetColor()].Count - 1])
            //{
            //    Debug.Log("Bok");
            //    break;
            //}


            hexagons.Remove(hex);

            colorToHexagonDict[hex.GetColor()].Remove(hex);

            hex.GoOtherCell(neighbor,
                triggerEvent: i == hexToSendList.Count - 1);

            yield return new WaitForSeconds(.1f);
        }
        isOccupied = hexagons.Count > 0 ? true : false;
        this.hexToSendList.Clear();
    }

    private void RecieveHexagonsFromNeighbor()
    {

    }
    #endregion

    private void SeparateHexagonsByColor()
    {
        if (hexagons.Count < 1) return;
        for (int i = 0; i < hexagons.Count; i++)
        {
            HexagonController hex = hexagons[i];
            ColorInfo.ColorEnum color = hex.GetColor();

            if (!colorToHexagonDict.ContainsKey(color))
            {
                colorToHexagonDict[color] = new List<HexagonController>();
            }
            colorToHexagonDict[color].Add(hex);
        }
    }

    #region Getters / Setters
    public void AddHex(HexagonController hex)
    {
        if (!hexagons.Contains(hex))
            hexagons.Add(hex);
    }
    // GETTERS
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
        foreach (Transform hex in HexStackParent)
        {
            HexagonController hexagonController = hex.GetComponent<HexagonController>();
            hexagons.Add(hexagonController);
        }
        //for (int i = 0; i < hexagons.Count; i++)
        //{
        //    HexagonController hex = hexagons[i];

        //    switch (hex.GetColor())
        //    {
        //        case ColorInfo.ColorEnum.RED
        //            :
        //            hexSorter.Red.Add(hex);
        //            break;
        //        case ColorInfo.ColorEnum.YELLOW
        //            :
        //            hexSorter.Yellow.Add(hex);
        //            break;
        //        case ColorInfo.ColorEnum.BLUE
        //            :
        //            hexSorter.Blue.Add(hex);
        //            break;
        //        case ColorInfo.ColorEnum.GREEN
        //            :
        //            hexSorter.Green.Add(hex);
        //            break;
        //        case ColorInfo.ColorEnum.PURPLE
        //            :
        //            hexSorter.Purple.Add(hex);
        //            break;

        //    }
        //}
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