using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/GridData", order = 1)]
public class GridData : ScriptableObject
{
    public enum CellType
    {
        Empty,
        Normal,
        Red,
        Green,
        Blue,
        Yellow
    }
    [System.Serializable]
    public class Cell
    {
        public CellType type;
        public int rotation;
    }

    [System.Serializable]
    public class Row
    {
        public Cell[] column;

        public Row(int size)
        {
            column = new Cell[size];

            for (int i = 0; i < size; i++)
            {
                column[i] = new Cell
                {
                    type = CellType.Empty,
                    rotation = 0
                };
            }
        }

        public void ClearRow()
        {
            for (int i = 0; i < column.Length; i++)
            {
                column[i].type = CellType.Empty;
                column[i].rotation = 0;
            }
        }
    }

    public int columns;
    public int rows;
    public Row[] board;

    public void Clear()
    {
        if (board == null) return;

        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] != null)
            {
                board[i].ClearRow();
            }
        }
    }

    public void CreateNewBoard()
    {
        if (columns <= 0 || rows <= 0) return;

        board = new Row[rows];

        for (int i = 0; i < rows; i++)
        {
            board[i] = new Row(columns);
        }
        Clear();
    }
}
