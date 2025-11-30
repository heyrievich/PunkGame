using UnityEngine;

public class BirdUI : MonoBehaviour
{
    [Header("Physics")]
    public float gravity = -1000f;
    public float jumpForce = 450f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip deathClip;
    public GameObject clickAnim;

    private RectTransform rect;
    private float verticalSpeed = 0f;
    private bool isAlive = true;
    private bool gameStarted = false; // игра началась после клика

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!isAlive) return;

        // Проверяем первый клик для старта игры
        if (!gameStarted && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            clickAnim.SetActive(false);
            gameStarted = true;
            Jump();
            return;
        }

        if (!gameStarted) return; // пока игрок не кликнул — физика не действует

        // Управление прыжками после старта
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Гравитация
        verticalSpeed += gravity * Time.deltaTime;
        rect.anchoredPosition += new Vector2(0, verticalSpeed * Time.deltaTime);

        // Проверка падения
        if (rect.anchoredPosition.y < -900f)
        {
            Die();
        }
    }

    void Jump()
    {
        verticalSpeed = jumpForce;
        if (audioSource != null && jumpClip != null)
        {
            audioSource.PlayOneShot(jumpClip);
        }
    }

    public void Die()
    {
        if (!isAlive) return;

        isAlive = false;

        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }

        FindObjectOfType<GameManagerUI>().GameOver();
    }
}
