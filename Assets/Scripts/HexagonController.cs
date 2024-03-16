using DG.Tweening;
using UnityEngine;

public class HexagonController : MonoBehaviour
{
    [SerializeField] ColorInfo.ColorEnum color;
    [SerializeField] float moveDuration;
    public void Initialize(ColorInfo.ColorEnum colorEnum, Material material)
    {
        color = colorEnum;
        GetComponentInChildren<Renderer>().material = material;
    }

    public ColorInfo.ColorEnum GetColor()
    {
        return color;
    }

    public void GoOtherCell(CellController targetCell)
    {
        transform.DOMove(targetCell.GetVerticalPosForHex(), moveDuration).OnComplete(() =>
        {
            InputManager.instance.TriggerCheckPossibleMovesEvent();
        });
    }

}