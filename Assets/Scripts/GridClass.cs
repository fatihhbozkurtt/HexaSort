using System.Collections.Generic;
using UnityEngine;

public class CellData : MonoBehaviour
{
    public int PosX;
    public int PosY;
    public bool isOpen;
    public List<ColorInfo.ColorEnum> CellContentList = new();
    public GameObject CellObject;
}
