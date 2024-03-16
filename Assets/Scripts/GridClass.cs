using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridClass : MonoBehaviour
{
    public int PosX;
    public int PosY;
    public bool isOpen;
    public List<ColorInfo.ColorEnum> GridContentList = new();

    public GameObject GridObject;

}
