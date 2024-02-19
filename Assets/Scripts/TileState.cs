using UnityEngine;

[CreateAssetMenu(menuName = "Tile State")]
public class TileState : ScriptableObject
{
    [SerializeField] private Color _backgroundColor;
    [SerializeField] private Color _textColor;

    public Color BackgroundColor { get; private set; }
    public Color TextColor { get; private set; }

    private void Awake()
    {
        BackgroundColor = _backgroundColor;
        TextColor = _textColor;
    }
}
