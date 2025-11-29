using UnityEngine;

public class PipeSpawnerUI : MonoBehaviour
{
    [Header("Префабы труб (UI)")]
    public GameObject[] pipePrefabs1; // список префабов для первого спавнера
    public GameObject[] pipePrefabs2; // список префабов для второго спавнера

    [Header("Точки спавна (UI)")]
    public RectTransform spawnPoint1;
    public RectTransform spawnPoint2;

    [Header("Настройки спавна")]
    public float spawnRate = 1.6f;    // частота спавна
    public float gapOffset = 300f;    // вертикальный разброс

    private float timer = 0f;
    private bool canSpawn = true;

    private bool toggleSpawn = true;  // чтобы поочередно спавнить

    void Update()
    {
        if (!canSpawn) return;

        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            if (toggleSpawn)
                SpawnPipe(spawnPoint1, pipePrefabs1);
            else
                SpawnPipe(spawnPoint2, pipePrefabs2);

            toggleSpawn = !toggleSpawn;  // меняем очередь
            timer = 0f;
        }
    }

    void SpawnPipe(RectTransform spawnPoint, GameObject[] pipePrefabs)
    {
        if (pipePrefabs.Length == 0 || spawnPoint == null) return;

        // Случайная вертикальная позиция
        float y = Random.Range(-gapOffset, gapOffset);

        // Выбираем случайный префаб
        int index = Random.Range(0, pipePrefabs.Length);
        GameObject pipePrefab = pipePrefabs[index];

        // Спавним как UI элемент
        GameObject pipe = Instantiate(pipePrefab, spawnPoint.parent);
        RectTransform pipeRect = pipe.GetComponent<RectTransform>();

        // Устанавливаем позицию относительно Canvas
        pipeRect.anchoredPosition = new Vector2(spawnPoint.anchoredPosition.x, spawnPoint.anchoredPosition.y + y);
    }

    public void StopSpawning()
    {
        canSpawn = false;
    }
}
