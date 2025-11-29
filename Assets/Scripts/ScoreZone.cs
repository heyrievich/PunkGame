using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    private bool counted = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (counted) return;
        if (other.CompareTag("Player"))
        {
            counted = true;
        }
    }
}
