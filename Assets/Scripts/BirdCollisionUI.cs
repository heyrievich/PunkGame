using UnityEngine;

public class BirdCollisionUI : MonoBehaviour
{
    RectTransform bird;

    void Start()
    {
        bird = GetComponent<RectTransform>();
    }

    void Update()
    {
        var pipes = FindObjectsOfType<PipeUI>();
        foreach (var pipe in pipes)
        {
            foreach (RectTransform r in pipe.GetComponentsInChildren<RectTransform>())
            {
                if (r == pipe.transform) continue;

                if (RectOverlap(bird, r))
                {
                    GetComponent<BirdUI>().Die();
                    return;
                }
            }
        }
    }

    bool RectOverlap(RectTransform a, RectTransform b)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(a, b.position) &&
               RectTransformUtility.RectangleContainsScreenPoint(b, a.position);
    }
}
