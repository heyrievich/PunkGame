using UnityEngine;

public class PipeUI : MonoBehaviour
{
    [Header("Настройки трубы")]
    public float speed = 300f;         // скорость движения трубы
    [Range(0.1f, 1f)]
    public float hitboxShrink = 0.7f;  // уменьшение хитбокса для менее чувствительных столкновений

    private RectTransform rect;
    private RectTransform bird;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        // Находим птичку
        BirdUI birdScript = FindObjectOfType<BirdUI>();
        if (birdScript != null)
            bird = birdScript.GetComponent<RectTransform>();
    }

    void Update()
    {
        // Двигаем трубу влево
        rect.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        // Уничтожаем, если ушла за левый край
        if (rect.anchoredPosition.x < -1200f)
        {
            Destroy(gameObject);
            return;
        }

        // Проверяем столкновение с птичкой
        if (bird != null && RectOverlaps(rect, bird, hitboxShrink))
        {
            bird.GetComponent<BirdUI>().Die();
        }
    }

    // Проверка пересечения RectTransform с уменьшением хитбокса трубы
    bool RectOverlaps(RectTransform a, RectTransform b, float shrink = 1f)
    {
        Vector3[] aCorners = new Vector3[4];
        Vector3[] bCorners = new Vector3[4];

        a.GetWorldCorners(aCorners);
        b.GetWorldCorners(bCorners);

        // a rect с уменьшением
        float aWidth = (aCorners[2].x - aCorners[0].x) * shrink;
        float aHeight = (aCorners[2].y - aCorners[0].y) * shrink;
        float aCenterX = (aCorners[0].x + aCorners[2].x) / 2f;
        float aCenterY = (aCorners[0].y + aCorners[2].y) / 2f;

        float aLeft = aCenterX - aWidth / 2f;
        float aRight = aCenterX + aWidth / 2f;
        float aBottom = aCenterY - aHeight / 2f;
        float aTop = aCenterY + aHeight / 2f;

        // b rect без изменения
        float bLeft = bCorners[0].x;
        float bRight = bCorners[2].x;
        float bBottom = bCorners[0].y;
        float bTop = bCorners[2].y;

        return aLeft < bRight && aRight > bLeft && aTop > bBottom && aBottom < bTop;
    }
}
