using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    private ColorInfo.ColorEnum colorEnum;
    private bool addingHexagons;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying) return;

        EditorGUILayout.LabelField("Select Hexagon Color Then Click The Cell In The Scene", EditorStyles.boldLabel);

        GUILayout.Space(10);
        GridManager gridManager = (GridManager)target;

        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("RED", GUILayout.ExpandWidth(true)))
        {
            colorEnum = ColorInfo.ColorEnum.RED;
            addingHexagons = true;
        }
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("BLUE", GUILayout.ExpandWidth(true)))
        {
            colorEnum = ColorInfo.ColorEnum.BLUE;
            addingHexagons = true;
        }
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("YELLOW", GUILayout.ExpandWidth(true)))
        {
            colorEnum = ColorInfo.ColorEnum.YELLOW;
            addingHexagons = true;
        }
        GUI.backgroundColor = Color.white; // Renkleri sýfýrla
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.white;
        if (GUILayout.Button("BLOCK ADDING HEX", GUILayout.ExpandWidth(true)))
        {
            colorEnum = ColorInfo.ColorEnum.None;
            addingHexagons = false;
        }
        GUILayout.EndHorizontal();
    }

    private void OnSceneGUI()
    {
        if (Application.isPlaying) return;

        Event currentEvent = Event.current;

        GridManager gridManager = (GridManager)target;

        if (currentEvent.type == EventType.MouseDown)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridManager.CellLayer))
            {
                if (hit.collider.transform.parent.parent.TryGetComponent(out CellController cellController))
                {
                    HandleCellInteraction(gridManager, cellController);
                }
            }
        }
    }

    private void HandleCellInteraction(GridManager gridManager, CellController cellController)
    {
        if (Application.isPlaying) return;

        if (!addingHexagons) // eðer bir renk buttonuna basýlmadýysa sadece cell aktifliði kontrol edilmeli
        {
            bool isOpen = true;
            cellController.ToggleCellObject(out isOpen);
            cellController.isOpen = isOpen;
        }
        else
        {
            if (colorEnum != ColorInfo.ColorEnum.None)
            {
                cellController.contentInfo.Add(colorEnum);
                int index = cellController.contentInfo.Count - 1;
                Material mat = gridManager.colorPack.HexagonColorInfo[gridManager.colorPack.GetColorEnumIndex(colorEnum)].colorMat;

                gridManager.SpawnHexagon(
                    index,
                    cellController.transform.position,
                    cellController.HexStackParent,
                    mat,
                    colorEnum);

                cellController.isOpen = true;
            }
        }

        gridManager.CurrentGridInfo.AddToStartInfo(cellController);
    }
}
