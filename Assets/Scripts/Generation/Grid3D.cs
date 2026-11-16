using System.Collections.Generic;
using UnityEngine;

public class Grid3D : MonoBehaviour
{
    Grid _grid;
    [SerializeField]
    int _gridSize;
    [SerializeField]
    int _gridHeight;
    Vector3Int _cellGap;
    Vector3 _cellSize;
    public GameObject GridTile;

    float _spawnChance;
    List<Vector3Int> _gridPositions;
    List<Cell> _gridCells;
    private void Awake()
    {
        _grid = GetComponent<Grid>();
        _cellSize = _grid.cellSize;
        _cellGap = new Vector3Int((int)_grid.cellGap.x, (int)_grid.cellGap.y, (int)_grid.cellGap.z);

    }
    void CreateGrid()
    {
        if (_gridPositions == null)
        {
            _gridCells = new List<Cell>();
            _gridPositions = new List<Vector3Int>();
        }

        _gridCells.Clear();
        _gridPositions.Clear();
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                for (int z = 0; z < _gridSize; z++)
                {
                    Vector3Int gridPos = new Vector3Int(x, y, z);
                    _gridCells.Add(new Cell(gridPos));

                    //InstantiateOnGrid(gridPos);
                }
            }
        }
    }
    void InstantiateOnGrid(Vector3Int gridPos)
    {
        var worldPos = _grid.GetCellCenterWorld(gridPos);
        _gridPositions.Add(gridPos);
        GameObject go = Instantiate(GridTile, worldPos, Quaternion.identity, this.transform);

        go.transform.localScale = _cellSize;
    }

    void CheckNeighbours(List<Cell> cells)
    {

        foreach (Cell cell in cells)
        {
            if (cell == null)
                continue;
            if (cell.SpawnProbability < 0)
            {
                //check neighbour cells
                Cell left = GetCellFromGridPos(cell.NeighbourLeft);
                Cell right = GetCellFromGridPos(cell.NeighbourRight);
                Cell front = GetCellFromGridPos(cell.NeighbourFront);
                Cell back = GetCellFromGridPos(cell.NeighbourBack);
                Cell top = GetCellFromGridPos(cell.NeighbourTop);
                Cell bottom = GetCellFromGridPos(cell.NeighbourBottom);

                float maxSpawnProb = 8f; //meant to be 8 out of 10 later
                float spawnProb = 0;
                bool neihboursHaveValue = false;

                if (left.CanSpawn)
                {
                    spawnProb -= left.SpawnProbability;
                    neihboursHaveValue = true;
                }
                if (right.CanSpawn)
                {
                    spawnProb -= right.SpawnProbability;
                    neihboursHaveValue = true;
                }
                if (front.CanSpawn)
                {
                    spawnProb -= front.SpawnProbability;
                    neihboursHaveValue = true;
                }
                if (back.CanSpawn)
                {
                    spawnProb -= back.SpawnProbability;
                    neihboursHaveValue = true;
                }
                if (top.CanSpawn)
                {
                    spawnProb -= top.SpawnProbability;
                    neihboursHaveValue = true;
                }

                if (!neihboursHaveValue)
                    cell.SpawnProbability = maxSpawnProb;
                else
                    cell.SpawnProbability = 5 - spawnProb;
                //Set a max spawn prob value
                //If this cell gets to cap then spawn cloud
                //Set neighbours to zero spawn ability and start counting up till cap
            }
            else if (cell.SpawnProbability <= 0)
            {
                continue;
            }
        }
    }
    Cell GetCellFromGridPos(Vector3Int gridPos)
    {
        return _gridCells.Find(c => c.GridPosition == gridPos);
    }
    bool InBounds(Vector3Int gridPos)
    {
        return new Bounds(Vector3Int.zero, new Vector3(_gridSize, _gridHeight, _gridSize)).Contains(gridPos + _cellGap);
    }
    //Create point distribution based on likely value calculated by a tweening formula such as EaseInOutBounce

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(.8f, .1f, .5f, .3f);
        if (_gridCells.Count > 0)
        {
            foreach (Cell cell in _gridCells)
            {
                var worldPos = _grid.GetCellCenterWorld(cell.GridPosition);
                Gizmos.DrawCube(worldPos, _grid.cellSize);
            }
        }
    }
}
public class Cell
{
    public float SpawnProbability = -1;
    bool _spawnProb;
    public bool CanSpawn
    {
        get { return _spawnProb; }
        set { _spawnProb = SpawnProbability >= 0; }
    }

    public Vector3Int GridPosition;
    public Vector3Int NeighbourFront;
    public Vector3Int NeighbourBack;
    public Vector3Int NeighbourLeft;
    public Vector3Int NeighbourRight;
    public Vector3Int NeighbourTop;
    public Vector3Int NeighbourBottom;

    public Cell(Vector3Int cellPos)
    {
        GridPosition = cellPos;
        NeighbourFront = new Vector3Int(GridPosition.x + 1, GridPosition.y, GridPosition.z);
        NeighbourBack = new Vector3Int(GridPosition.x - 1, GridPosition.y, GridPosition.z);
        NeighbourLeft = new Vector3Int(GridPosition.x, GridPosition.y - 1, GridPosition.z);
        NeighbourRight = new Vector3Int(GridPosition.x, GridPosition.y + 1, GridPosition.z);
        NeighbourTop = new Vector3Int(GridPosition.x, GridPosition.y, GridPosition.z + 1);
        NeighbourBottom = new Vector3Int(GridPosition.x, GridPosition.y, GridPosition.z - 1);
    }

    public bool HasSpawnProb()
    {
        return SpawnProbability >= 0;
    }
}