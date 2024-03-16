using UnityEngine;

public class CellStatsContainer
{
    // Since this class just responsible to containing some data
    // getters/setters functions should not be writtenin this class
    // such as "GetOccupied()" or "SetOccupationStatus(status: )" 

    public int PosX = 0;
    public int PosY = 0;
    public bool IsOccupied = false;
    public GameObject CellObject = null;
}