using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameStarter : MonoBehaviour, IPointerDownHandler
{
    bool ready = false;

    private void Start()
    {
        ready = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (ready)
        {
            ready = false;
            GameManager.instance.StartGame();
        }
    }
}
