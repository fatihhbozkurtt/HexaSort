using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class PickableStack : MonoBehaviour
{
    [Header("Configuration")]
    public bool IsPicked;
    public bool IsPlaced;
    [SerializeField] LayerMask cellLayer;

    [Header("Debug")]
    Vector3 _startPos;
    Vector3 offset => new Vector3(0, .5f, 2);
    Collider _collider => GetComponent<Collider>();
    [SerializeField] List<HexagonController> hexagons = new List<HexagonController>();

    private void Awake()
    {
        _startPos = transform.position;
    }

    IEnumerator Start()
    {
        GameManager.instance.LevelEndedEvent += DestroySelf;

        yield return null;

        foreach (Transform hex in transform)
        {
            hexagons.Add(hex.GetComponent<HexagonController>());
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!IsPicked) return;
            IsPicked = false;

            CellController cell = GetCellBelow();

            if (cell != null)
            {
                GoToCell(cell);
                _collider.enabled = false;
            }
            else
            {
                GetReleased();
            }
        }

        if (!gameObject.activeInHierarchy) return;
        if (!IsPicked) return;

        FollowMousePos();
        GetCellBelow();
    }
    private void FollowMousePos()
    {
        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

        // Update the position of the selected object to the mouse position
        transform.position = new Vector3(mousePosition.x + offset.x, transform.position.y, mousePosition.z + offset.z);
    }
    void GoToCell(CellController targetCell)
    {
        targetCell.UpdateHexagonsList(hexagons);

        for (int i = 0; i < hexagons.Count; i++)
        {
            Transform hex = hexagons[i].transform;

            hex.transform.DOLocalMove(new Vector3(0, i * GridManager.instance.VERTICAL_PLACEMENT_OFFSET, 0), 0.3f);
        }

        InputManager.instance.SetBlockPicking(shouldBlock: false);
        InputManager.instance.TriggerStackPlacedOnGridEvent(this);
        targetCell.SetOccupied(true);
        targetCell.StartCoroutine(targetCell.ControlTransfer(.4f));

        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(gameObject, .1f);
        GameManager.instance.LevelEndedEvent -= DestroySelf;
    }

    #region GETTERS

    CellController GetCellBelow()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.up);

        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);

        if (Physics.Raycast(ray, out hit, 100, cellLayer))
        {
            if (hit.collider.transform.parent.parent.TryGetComponent(out CellController cell))
            {
                if (cell.isOccupied) return null;
                return cell;
            }
        }

        return null;
    }
    public void GetPicked()
    {
        IsPicked = true;
    }

    public void GetReleased()
    {
        IsPicked = false;
        InputManager.instance.SetBlockPicking(false);
        transform.DOMove(_startPos, .5f);
    }
    public void GetPlaced(Vector3 cellPosition)
    {
        cellPosition.y += 0.1f;
        transform.position = cellPosition;
        IsPicked = true;
    }

    #endregion
}