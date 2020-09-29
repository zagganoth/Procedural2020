using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid<CellType> : ScriptableObject
{
    private float cellSize;
    private int width, height;
    private CellType[,] gridContents;
    private Vector3Int basePosition;
    public Grid(Vector3Int basePos, float tileSize, int sizeX, int sizeY)
    {
        width = sizeX;
        height = sizeY;
        cellSize = tileSize;
        gridContents = new CellType[width,height];
        basePosition = basePos;
    }
    public void SetCell(Vector3Int tilePosition, CellType tile)
    {
        gridContents[tilePosition.x, tilePosition.y] = tile; 
    }
    public CellType GetCell(Vector3Int tilePosition)
    {
        return gridContents[tilePosition.x - basePosition.x, tilePosition.y - basePosition.y];
    }
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return basePosition + Vector3Int.FloorToInt(worldPosition);
    }
}
