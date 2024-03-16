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

    public void GoOtherCell(CellController targetCell, bool triggerEvent = false)
    {
        targetCell.AddHex(this);
        transform.DOMove(targetCell.GetVerticalPosForHex(), moveDuration).OnComplete(() =>
        {
            transform.SetParent(targetCell.HexStackParent);
            if (triggerEvent)
                InputManager.instance.TriggerCheckPossibleMovesEvent();
        });
    }

}