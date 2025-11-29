using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;
    public bool moveX;
    public bool moveY;
    public bool moveZ;

    [Header("Inventory Key ID")]
    public string keyItemId = "Key";

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip openSound;      // звук успешного открытия
    public AudioClip noKeySound;     // звук ошибки (нет ключа)

    private bool isPlayerInside = false;
    private bool isOpen = false;
    private Vector3 initialPos;
    private Vector3 targetPos;

    private void Start()
    {
        initialPos = door.position;
        targetPos = initialPos + new Vector3(
            moveX ? moveDistance : 0f,
            moveY ? moveDistance : 0f,
            moveZ ? moveDistance : 0f
        );
    }

    private void Update()
    {
        if (!isPlayerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }

        if (isOpen)
        {
            door.position = Vector3.Lerp(
                door.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void TryOpenDoor()
    {
        if (InventoryManager.Instance.HasItem(keyItemId))
        {
            isOpen = true;
            InventoryManager.Instance.RemoveItem(keyItemId, 1);

            if (audioSource && openSound)
                audioSource.PlayOneShot(openSound);

            Debug.Log("Дверь открыта!");
        }
        else
        {
            if (audioSource && noKeySound)
                audioSource.PlayOneShot(noKeySound);

            Debug.Log("У вас нет ключа!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInside = false;
    }
}
