using UnityEngine;
using TMPro; // для TextMeshPro
using UnityEngine.SceneManagement;

public class GameManagerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;   // текст для отображения очков
    public GameObject gameOverPanel;

    [Header("Настройки")]
    public int pointsPerSecond = 50;    // очков в секунду
    public int scoreToLevel2 = 500;     // очки для перехода на Level2

    private float score = 0f;           // используем float для плавного начисления
    private bool isGameOver = false;    // флаг окончания игры

    // Публичное свойство, чтобы другие скрипты могли проверять GameOver
    public bool IsGameOver => isGameOver;

    void Start()
    {
        score = 0f;
        UpdateScore();
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameOver)
            return;

        // Начисляем очки каждый кадр
        score += pointsPerSecond * Time.deltaTime;
        UpdateScore();

        // Проверка перехода на Level2
        if (score >= scoreToLevel2)
        {
            SceneManager.LoadScene("Level2");
        }
    }

    public void AddScore(int s)
    {
        if (isGameOver)
            return;

        score += s;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = Mathf.FloorToInt(score).ToString();
    }

    public void GameOver()
    {
        isGameOver = true;
        FindObjectOfType<PipeSpawnerUI>().StopSpawning();

        gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
