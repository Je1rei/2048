using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private TileState[] _tileStates;
    [SerializeField] private int _capacity = 16;

    private TileGrid _grid;
    private List<Tile> _tiles;

    private bool _isWaiting;

    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(_capacity);
    }

    private void Update()
    {
        if (!_isWaiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0, 1, _grid.Height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right, _grid.Width - 2, -1, 0, 1);
            }
        }
    }


    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < _grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < _grid.Height; y += incrementY)
            {
                TileCell cell = _grid.GetCell(x, y);

                if (cell.IsOccupied)
                {
                    changed |= MoveTile(cell.Tile, direction);
                }
            }
        }

        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = _grid.GetAdjacentCell(tile.Cell, direction);

        bool isCanMove = true;

        while (adjacent != null && isCanMove == true)
        {
            if (adjacent.IsOccupied)
            {
                if (CanMerge(tile, adjacent.Tile))
                {
                    Merge(tile, adjacent.Tile);

                    return true;
                }

                isCanMove = false;
            }

            if (isCanMove == true)
            {
                newCell = adjacent;
                adjacent = _grid.GetAdjacentCell(adjacent, direction);
            }
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile firstTile, Tile secondTile)
    {
        return firstTile.Number == secondTile.Number && !secondTile.Locked;
    }

    private void Merge(Tile firstTile, Tile secondTile)
    {
        _tiles.Remove(firstTile);
        firstTile.Merge(secondTile.Cell);

        int index = Mathf.Clamp(IndexOf(secondTile.State) + 1, 0, _tileStates.Length - 1);

        int multiplier = 2;
        int number = secondTile.Number * multiplier;

        secondTile.SetState(_tileStates[index], number);

        _game.IncreaseScore(number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < _tileStates.Length; i++)
        {
            if (state == _tileStates[i])
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        float duration = 0.1f;

        _isWaiting = true;

        yield return new WaitForSeconds(duration);

        _isWaiting = false;

        foreach (Tile tile in _tiles)
        {
            tile.SetLocked(false);
        }

        if (_tiles.Count != _grid.Size)
        {
            CreateTile();
        }

        if (CheckGameOver())
        {
            _game.GameOver();
        }
    }

    private bool CheckGameOver()
    {
        if (_tiles.Count != _grid.Size)
        {
            return false;
        }

        foreach (Tile tile in _tiles)
        {
            TileCell tileUp = _grid.GetAdjacentCell(tile.Cell, Vector2Int.up);
            TileCell tileDown = _grid.GetAdjacentCell(tile.Cell, Vector2Int.down);

            TileCell tileLeft = _grid.GetAdjacentCell(tile.Cell, Vector2Int.left);
            TileCell tileRight = _grid.GetAdjacentCell(tile.Cell, Vector2Int.right);

            if (tileUp != null && CanMerge(tile, tileUp.Tile))
                return false;

            if (tileDown != null && CanMerge(tile, tileDown.Tile))
                return false;

            if (tileLeft != null && CanMerge(tile, tileLeft.Tile))
                return false;

            if (tileRight != null && CanMerge(tile, tileRight.Tile))
                return false;
        }

        return true;
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(_tilePrefab, _grid.transform);
        tile.SetState(_tileStates[0], 2);

        tile.Spawn(_grid.GetRandomEmptyCell());

        _tiles.Add(tile);
    }

    public void ClearBoard()
    {
        foreach (TileCell cell in _grid.Cells)
        {
            cell.SetTile(null);
        }

        foreach (Tile tile in _tiles)
        {
            Destroy(tile.gameObject);
        }

        _tiles.Clear();
    }
}
