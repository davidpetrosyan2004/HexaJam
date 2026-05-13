using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridData))]
[CanEditMultipleObjects]
public class GridDataDrawer : Editor
{
    private GridData Data => (GridData)target;
    private GridData.CellType selectedType;
    private int selectedRotation;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        DrawSizeFields();
        EditorGUILayout.Space();

        DrawButtons();
        EditorGUILayout.Space();

        if (IsBoardValid())
        {
            DrawBoard();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(Data);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSizeFields()
    {
        SerializedProperty columnsProp = serializedObject.FindProperty("columns");
        SerializedProperty rowsProp = serializedObject.FindProperty("rows");

        EditorGUILayout.PropertyField(columnsProp);
        EditorGUILayout.PropertyField(rowsProp);

        if (serializedObject.hasModifiedProperties)
        {
            serializedObject.ApplyModifiedProperties();

            Undo.RecordObject(Data, "Resize Grid");
            Data.CreateNewBoard();
            EditorUtility.SetDirty(Data);
        }
        selectedType = (GridData.CellType)EditorGUILayout.EnumPopup("Color", selectedType);

        selectedRotation = EditorGUILayout.IntPopup(
            "Rotation",
            selectedRotation,
            new[] { "0", "60", "-60", "-120", "120", "180"},
            new[] { 0, 60, -60, -120, 120, 180}
        );
    }

    private void DrawButtons()
    {
        if (GUILayout.Button("Clear Board"))
        {
            Data.Clear();
        }
    }

    private bool IsBoardValid()
    {
        return Data.board != null &&
               Data.board.Length == Data.rows &&
               Data.columns > 0 &&
               Data.rows > 0;
    }

    private void DrawBoard()
    {
        float size = 25f;
        float offsetX = size * 0.5f; 
        for (int row = 0; row < Data.rows; row++)
        {
            EditorGUILayout.BeginHorizontal();

            if (row % 2 == 1)
            {
                GUILayout.Space(offsetX);
            }

            for (int col = 0; col < Data.columns; col++)
            {
                var cell = Data.board[row].column[col];

                Color old = GUI.color;
                GUI.color = GetColor(cell.type);

                if (GUILayout.Button("", GUILayout.Width(size), GUILayout.Height(size)))
                {
                    cell.type = selectedType;
                    cell.rotation = selectedRotation;
                }

                GUI.color = old;
                GUI.Label(
                        GUILayoutUtility.GetLastRect(),
                        cell.rotation.ToString(),
                        new GUIStyle { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.black } }
                    );
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private Color GetColor(GridData.CellType type)
    {
        switch (type)
        {
            case GridData.CellType.Normal: return Color.white;
            case GridData.CellType.Purple: return new Color(0.5f, 0f, 0.5f);
            case GridData.CellType.Orange: return new Color(1f, 0.5f, 0f);
            case GridData.CellType.Yellow: return Color.yellow;
            case GridData.CellType.Green: return Color.green;
            case GridData.CellType.Blue: return Color.blue;
            case GridData.CellType.Black: return Color.black;
            default: return Color.gray;
        }
    }
}