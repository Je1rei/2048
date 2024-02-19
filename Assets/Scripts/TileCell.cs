using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int Coordinates { get; private set; }
    public Tile Tile { get; private set; }
    public bool IsEmpty => Tile == null;
    public bool IsOccupied => Tile != null;

    public Vector2Int SetCoordinates(Vector2Int vector2)
    {
        return Coordinates = vector2;
    }

    public void SetTile(Tile tile) => Tile = tile;

}
