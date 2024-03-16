using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPackNew", menuName = "ColorPack/MyColorPack", order = 1)]
public class ColorPack : ScriptableObject
{
    [System.Serializable]
    public class ColorPackInfo
    {
        public ColorInfo.ColorEnum SelectedColorEnum = ColorInfo.ColorEnum.None;
        public Color RopeColor = new Color32(255, 255, 255, 255);
        public Sprite ObjectiveSprite;
        public Color ObjectiveSpriteColor;
    }
    public List<ColorPackInfo> BlockColorInfos = new List<ColorPackInfo>();
    public int GetColorEnumIndex(ColorInfo.ColorEnum ControlColorEnum)
    {
        for (int i = 0; i < BlockColorInfos.Count; i++)
        {
            if (ControlColorEnum == BlockColorInfos[i].SelectedColorEnum)
            {
                return i;
            }
        }
        return 999;
    }
}