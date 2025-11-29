using UnityEngine;

public class GroundUI : MonoBehaviour
{
    private RectTransform rect;
    private BirdUI bird;
    private GameManagerUI gameManager;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        bird = FindObjectOfType<BirdUI>();
        gameManager = FindObjectOfType<GameManagerUI>();
    }

    void Update()
    {
        if (bird == null || gameManager == null) return;

        // Проверяем столкновение с птичкой
        if (RectOverlaps(rect, bird.GetComponent<RectTransform>()))
        {
            gameManager.GameOver();
        }
    }

    // Проверка пересечения двух RectTransform
    bool RectOverlaps(RectTransform a, RectTransform b)
    {
        Vector3[] aCorners = new Vector3[4];
        Vector3[] bCorners = new Vector3[4];

        a.GetWorldCorners(aCorners);
        b.GetWorldCorners(bCorners);

        float aLeft = aCorners[0].x;
        float aRight = aCorners[2].x;
        float aBottom = aCorners[0].y;
        float aTop = aCorners[2].y;

        float bLeft = bCorners[0].x;
        float bRight = bCorners[2].x;
        float bBottom = bCorners[0].y;
        float bTop = bCorners[2].y;

        return aLeft < bRight && aRight > bLeft && aTop > bBottom && aBottom < bTop;
    }
}
