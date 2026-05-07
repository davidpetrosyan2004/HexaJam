using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static GridData;
public class Grid : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap board;

    [Header("Prefabs")]
    [SerializeField] private GridCell hexPrefab;
    [SerializeField] private Turtle turtlePrefab;
    [SerializeField] private GridData gridData;
    
    [Header("Characteristics")]
    [SerializeField] private Vector3 turtleOffset;
    [SerializeField] private Vector3 tileOffset;

    [Header("Lists")]
    [SerializeField] private List<Texture> turtleTextures;
    private int turtleCount;

    private void OnEnable()
    {
        GameEvents.OnTurtleDissapear += OnTurtleDissapear;
    }

    private void OnDisable()
    {
        GameEvents.OnTurtleDissapear -= OnTurtleDissapear;


    }

    public void OnTurtleDissapear()
    {
        turtleCount-=3;
        if (turtleCount <= 0)
        {
            //GameEvents.OnGameWin?.Invoke();
            Debug.Log("Win");
        }
    }


    private void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        for (int i = 0; i < gridData.rows; i++)
        {
            for (int j = 0; j < gridData.columns; j++)
            {
                var pos = new Vector3Int(j, i, 0);
                PlaceTileTypeOnBoard(gridData.board[i].column[j].type, gridData.board[i].column[j].rotation,pos);
            }
        }
    }

    private void PlaceTileTypeOnBoard(GridData.CellType cellType, float rotation, Vector3Int pos)
    {
        var spawnPos = board.GetCellCenterWorld(pos);
        if (cellType == GridData.CellType.Empty)
        {
            return;
        }
        GridCell gridCell = Instantiate(hexPrefab, spawnPos + tileOffset, Quaternion.identity, board.transform);

        Texture turtleTexture = GetTextureOfTurtle(cellType);
        if (turtleTexture == turtleTextures[0]) return;

        Turtle turtle = Instantiate(turtlePrefab, spawnPos + turtleOffset, Quaternion.Euler(-90, rotation+30, 0), gridCell.transform);
        turtle.Texture = turtleTexture;

        gridCell.Turtle = turtle;

        turtleCount++;
    }

    public Texture GetTextureOfTurtle(GridData.CellType cellType)
    {
        if (cellType == GridData.CellType.Purple)
        {
            return turtleTextures[1];
        }
        else if (cellType == GridData.CellType.Orange)
        {
            return turtleTextures[2];
        }
        else if (cellType == GridData.CellType.Yellow)
        {
            return turtleTextures[3];
        }
        return turtleTextures[0];
    }
}
