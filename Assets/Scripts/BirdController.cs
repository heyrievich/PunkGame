using UnityEngine;

public class BirdUI : MonoBehaviour
{
    public float gravity = -1000f;
    public float jumpForce = 450f;

    RectTransform rect;
    float verticalSpeed = 0f;
    bool isAlive = true;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!isAlive) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            verticalSpeed = jumpForce;
        }

        verticalSpeed += gravity * Time.deltaTime;
        rect.anchoredPosition += new Vector2(0, verticalSpeed * Time.deltaTime);

        // Если упала ниже
        if (rect.anchoredPosition.y < -900f)
        {
            Die();
        }
    }

    public void Die()
    {
        isAlive = false;
        FindObjectOfType<GameManagerUI>().GameOver();
    }
}
