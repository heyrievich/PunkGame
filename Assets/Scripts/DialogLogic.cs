using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    [Header("Dialogs")]
    public Animator dialog1Animator;
    public Animator dialog2Animator;

    [Header("Text UI")]
    public TMP_Text dialog1Text;
    public TMP_Text dialog2Text;

    [Header("Dialogue Text Lists")]
    public List<string> dialog1Lines;
    public List<string> dialog2Lines;

    [Header("Player Reference")]
    public CharacterMovement playerMovement;
    public GameObject uiInterface;      // интерфейс игрока
    public GameObject dialogOverlay;    // UI, который показываем во время диалога

    [Header("Transition Animator")]
    public Animator transitionAnimator; // аниматор для перехода на следующий уровень

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip nextLineClip;      // звук при переходе на следующую реплику

    [Header("Next Level Settings")]
    public bool isNextLevel = false;
    public string nextSceneName;

    [Tooltip("Общее количество реплик (если 0 — берётся автоматически)")]
    public int dialogMax = 0;

    private int nextIndex1 = 0;
    private int nextIndex2 = 0;
    private int shownCount = 0;
    private bool showingDialog1 = true;
    private bool isDialogAnimating = false;
    private bool isDialogueActive = false;

    public void StartDialogue()
    {
        if (isDialogueActive) return;

        if (dialogMax <= 0)
            dialogMax = dialog1Lines.Count + dialog2Lines.Count;

        isDialogueActive = true;

        // Отключаем управление игроком
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Отключаем основной UI
        if (uiInterface != null)
            uiInterface.SetActive(false);

        // Включаем диалоговый UI/оверлей
        if (dialogOverlay != null)
            dialogOverlay.SetActive(true);

        nextIndex1 = 0;
        nextIndex2 = 0;
        shownCount = 0;
        showingDialog1 = true;
        isDialogAnimating = false;

        if (dialog1Lines.Count > 0)
        {
            dialog1Text.text = dialog1Lines[0];
            nextIndex1 = 1;
            shownCount = 1;
        }
        else
        {
            EndDialogue();
            return;
        }

        if (dialog1Animator != null)
            dialog1Animator.Play("DialogPanel1Open");
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextDialogue();
        }
    }

    void NextDialogue()
    {
        if (isDialogAnimating) return;

        if (shownCount >= dialogMax)
        {
            EndDialogue();
            return;
        }

        // Проигрываем звук листания диалога
        if (audioSource != null && nextLineClip != null)
            audioSource.PlayOneShot(nextLineClip);

        if (showingDialog1)
        {
            if (nextIndex2 < dialog2Lines.Count)
            {
                string line = dialog2Lines[nextIndex2];
                nextIndex2++;
                shownCount++;

                StartCoroutine(SwitchDialog(
                    dialog1Animator, "DialogPanel1Close",
                    dialog2Animator, "DialogPanel2Open",
                    dialog2Text, line,
                    false
                ));
            }
            else EndDialogue();
        }
        else
        {
            if (nextIndex1 < dialog1Lines.Count)
            {
                string line = dialog1Lines[nextIndex1];
                nextIndex1++;
                shownCount++;

                StartCoroutine(SwitchDialog(
                    dialog2Animator, "DialogPanel2Close",
                    dialog1Animator, "DialogPanel1Open",
                    dialog1Text, line,
                    true
                ));
            }
            else EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;

        if (dialog1Animator != null) dialog1Animator.Play("DialogPanel1Close");
        if (dialog2Animator != null) dialog2Animator.Play("DialogPanel2Close");

        // Вернуть управление игроку только если не переключаем сцену
        if (!isNextLevel && playerMovement != null)
            playerMovement.enabled = true;

        // Включить основной UI
        if (uiInterface != null)
            uiInterface.SetActive(true);

        // Отключить диалоговый UI/оверлей
        if (dialogOverlay != null)
            dialogOverlay.SetActive(false);

        isDialogAnimating = false;

        // Если нужно перейти на новый уровень
        if (isNextLevel && !string.IsNullOrEmpty(nextSceneName))
        {
            if (transitionAnimator != null)
            {
                transitionAnimator.Play("toPerehod");
                StartCoroutine(LoadSceneWithDelay(1.5f));
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private IEnumerator LoadSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextSceneName);
    }

    private System.Collections.IEnumerator SwitchDialog(
        Animator closingAnimator, string closeTrigger,
        Animator openingAnimator, string openTrigger,
        TMP_Text nextText, string nextLine,
        bool nextDialogIs1)
    {
        isDialogAnimating = true;

        if (closingAnimator != null) closingAnimator.Play(closeTrigger);
        yield return new WaitForSeconds(0.3f);

        showingDialog1 = nextDialogIs1;

        if (nextText != null) nextText.text = nextLine;
        if (openingAnimator != null) openingAnimator.Play(openTrigger);

        yield return new WaitForSeconds(0.3f);
        isDialogAnimating = false;
    }
}
