using System.Collections;
using UnityEngine;
using TMPro;

public class Game : MonoBehaviour 
{
    [SerializeField] private TileBoard _board;
    [SerializeField] private CanvasGroup _gameOver;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;

    public int Score { get; private set; }

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        _highScoreText.text = LoadHighScore().ToString();

        _gameOver.alpha = 0f;
        _gameOver.interactable = false;

        _board.ClearBoard();
        _board.CreateTile();
        _board.CreateTile();

        _board.enabled = true;
    }

    public void GameOver()
    {
        _board.enabled = false;
        _gameOver.interactable = true;

        StartCoroutine(Fade(_gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay) 
    {

        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while(elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            
            yield return null;  
        }

        canvasGroup.alpha = to;
    }

    public void IncreaseScore(int points)
    {
        SetScore(Score + points);
    }

    private void SetScore(int score)
    {
        Score = score;
        _scoreText.text = Score.ToString();

        SaveHighScore();
    }

    private void SaveHighScore()
    {
        int highScore = LoadHighScore();

        if(Score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", Score);
        }
    }

    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }
}
