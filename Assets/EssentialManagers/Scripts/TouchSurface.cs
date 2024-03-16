using UnityEngine.EventSystems;
using UnityEngine;

public class TouchSurface : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData _)
    {
        InputManager.instance.OnPointerDown();
    }

    public void OnPointerUp(PointerEventData _)
    {
        InputManager.instance.OnPointerUp();
    }
}
