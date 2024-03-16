using DG.Tweening;
using UnityEngine;

public class HexagonController : MonoBehaviour
{
    [SerializeField] ColorInfo.ColorEnum color;
  
    public void Initialize(ColorInfo.ColorEnum colorEnum, Material material)
    {
        color = colorEnum;
        GetComponentInChildren<Renderer>().material = material;
    }

    public ColorInfo.ColorEnum GetColor()
    {
        return color;
    }
    public void DestroySelf()
    {
        transform.DOScale(Vector3.zero, .25f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}