using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPackNew", menuName = "ColorPack/MyColorPack", order = 1)]
public class ColorPack : ScriptableObject
{
    [System.Serializable]
    public class ColorPackInfo
    {
        public ColorInfo.ColorEnum SelectedColorEnum = ColorInfo.ColorEnum.None;
        public Color HexColor = new Color32(255, 255, 255, 255);
        public Material colorMat;
    }
    public List<ColorPackInfo> HexagonColorInfo = new List<ColorPackInfo>();
    public int GetColorEnumIndex(ColorInfo.ColorEnum ControlColorEnum)
    {
        for (int i = 0; i < HexagonColorInfo.Count; i++)
        {
            if (ControlColorEnum == HexagonColorInfo[i].SelectedColorEnum)
            {
                return i;
            }
        }
        return 999;
    }
}