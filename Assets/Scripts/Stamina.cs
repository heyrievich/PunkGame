using UnityEngine;
using UnityEngine.UI;

public class StaminaUI_Rect : MonoBehaviour
{
    [Header("UI Elements (RectTransforms)")]
    public Image content;
    public Image bg;

    [Header("Player Reference")]
    public CharacterMovement player;

    [Header("Smooth")]
    public bool smooth = true;
    public float smoothSpeed = 8f;

    private RectTransform contentRect;
    private RectTransform bgRect;
    private float targetWidth;
    private float currentWidth;

    private void Awake()
    {
        contentRect = content.rectTransform;
        bgRect = bg.rectTransform;

        // 🔹 Жёстко фиксируем якоря и pivot для уменьшения справа налево
        contentRect.anchorMin = new Vector2(0f, contentRect.anchorMin.y);
        contentRect.anchorMax = new Vector2(0f, contentRect.anchorMax.y);
        contentRect.pivot = new Vector2(0f, contentRect.pivot.y);

        if (player == null)
            player = FindObjectOfType<CharacterMovement>();
    }

    private void Update()
    {
        if (player == null) return;

        float percent = player.GetStamina(); // 0..1
        float bgWidth = bgRect.rect.width;

        targetWidth = bgWidth * percent;

        if (smooth)
            currentWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * smoothSpeed);
        else
            currentWidth = targetWidth;

        contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth);
    }
}
