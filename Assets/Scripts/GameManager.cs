using UnityEngine;
using TMPro; // дл€ TextMeshPro
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManagerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;   // текст дл€ отображени€ очков
    public GameObject gameOverPanel;

    [Header("Ќастройки")]
    public int pointsPerSecond = 50;    // очков в секунду
    public int scoreToLevel2 = 500;     // очки дл€ перехода на Level2

    [Header("Transition Animator")]
    public Animator transitionAnimator; // аниматор дл€ стартовой и закрывающей анимации

    private float score = 0f;           // используем float дл€ плавного начислени€
    private bool isGameOver = false;    // флаг окончани€ игры
    private bool isGameStarted = false; // игра началась после клика

    public bool IsGameOver => isGameOver;

    void Start()
    {
        score = 0f;
        UpdateScore();
        gameOverPanel.SetActive(false);

        // ѕроигрываем стартовую анимацию, если есть Animator
        if (transitionAnimator != null)
            transitionAnimator.Play("fromPerehod");
    }

    void Update()
    {
        // ∆дЄм первый клик дл€ старта
        if (!isGameStarted)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                isGameStarted = true;
            }
            else
            {
                return; // игра не стартовала, ничего не начисл€ем
            }
        }

        if (isGameOver) return;

        // Ќачисл€ем очки каждый кадр
        score += pointsPerSecond * Time.deltaTime;
        UpdateScore();

        // ѕроверка перехода на Level2
        if (score >= scoreToLevel2)
        {
            StartCoroutine(TransitionToNextLevel("Level2"));
        }
    }

    public void AddScore(int s)
    {
        if (isGameOver || !isGameStarted)
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

    private IEnumerator TransitionToNextLevel(string sceneName)
    {
        isGameOver = true; // останавливаем начисление очков

        // ѕроигрываем анимацию закрыти€, если есть Animator
        if (transitionAnimator != null)
            transitionAnimator.Play("closeFluppy");

        // ∆дЄм 1 секунду перед переходом
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
    }
}
