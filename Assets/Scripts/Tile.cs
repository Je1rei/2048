using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour  // 55
{
    public TileState State { get; private set; }
    public TileCell Cell { get; private set; }
    public int Number { get; private set; }
    public bool Locked { get; private set; }    

    private Image _backgroundImage;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _backgroundImage = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetState(TileState state, int number)
    {
        State = state;
        Number = number;

        _backgroundImage.color = State.BackgroundColor;
        _text.color = State.TextColor;
        _text.text = Number.ToString();
    }

    public void SetLocked(bool value) => Locked = value;

    public void Spawn(TileCell cell)
    {
        ClearCell();

        Cell = cell;

        Cell.SetTile(this);

        transform.position = Cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        ClearCell();

        Cell = cell;

        Cell.SetTile(this);

        StartCoroutine(Animate(cell.transform.position, false));
    }

    public void Merge(TileCell cell)
    {
        ClearCell();

        Cell = null;
        cell.Tile.Locked = true;

        StartCoroutine(Animate(cell.transform.position, true));
    }

    private IEnumerator Animate(Vector3 positionTo, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 positionFrom = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(positionFrom, positionTo, elapsed / duration);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = positionTo;

        if (merging)
        {
            Destroy(gameObject);
        }
    }

    private void ClearCell()
    {
        if (Cell != null)
        {
            Cell.SetTile(null);
            Cell = null;
        }
    }
}
