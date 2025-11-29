using UnityEngine;

public class LevitationEffect : MonoBehaviour
{
    public float amplitude = 10f; // Амплитуда движения (в пикселях)
    public float speed = 2f;      // Скорость движения (чем больше, тем быстрее)

    private Vector3 originalPosition; // Изначальная позиция объекта

    void Start()
    {
        // Сохраняем изначальную позицию объекта
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Рассчитываем новое смещение по оси Y
        float offsetY = Mathf.Sin(Time.time * speed) * amplitude;

        // Обновляем позицию объекта
        transform.localPosition = originalPosition + new Vector3(0, offsetY, 0);
    }
}
