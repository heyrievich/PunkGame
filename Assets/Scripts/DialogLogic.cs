using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Tooltip("Общее количество реплик (если 0, будет установлено автоматически как dialog1Lines.Count + dialog2Lines.Count)")]
    public int dialogMax = 0;

    private int nextIndex1 = 0; // следующий индекс для dialog1 (уже показанные: 0 .. nextIndex1-1)
    private int nextIndex2 = 0; // следующий индекс для dialog2
    private int shownCount = 0; // сколько реплик уже показано (включая первую)
    private bool showingDialog1 = true;
    private bool isDialogAnimating = false;
    private bool isDialogueActive = false;

    public void StartDialogue()
    {
        if (isDialogueActive) return;

        // если dialogMax не задан - посчитает автоматически
        if (dialogMax <= 0)
            dialogMax = dialog1Lines.Count + dialog2Lines.Count;

        isDialogueActive = true;

        // Блокируем движение игрока
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Инициализация индексов
        nextIndex1 = 0;
        nextIndex2 = 0;
        shownCount = 0;
        showingDialog1 = true;
        isDialogAnimating = false;

        // Показываем первую реплику dialog1 (если есть)
        if (dialog1Lines.Count > 0)
        {
            dialog1Text.text = dialog1Lines[0];
            nextIndex1 = 1; // следующая для dialog1 будет 1
            shownCount = 1;
        }
        else
        {
            // если первой строки нет — сразу завершаем
            EndDialogue();
            return;
        }

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

        // Если уже показано достаточно реплик — завершаем
        if (shownCount >= dialogMax)
        {
            EndDialogue();
            return;
        }

        // Определяем, какая панель сейчас открыта и какую реплику показывать следующей
        if (showingDialog1)
        {
            // Сейчас открыт dialog1 — нужно показать dialog2[nextIndex2]
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
            else
            {
                // Нет больше реплик для dialog2 — завершаем
                EndDialogue();
            }
        }
        else
        {
            // Сейчас открыт dialog2 — нужно показать dialog1[nextIndex1]
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
            else
            {
                // Нет больше реплик для dialog1 — завершаем
                EndDialogue();
            }
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;

        // Закрываем панели
        if (dialog1Animator != null) dialog1Animator.Play("DialogPanel1Close");
        if (dialog2Animator != null) dialog2Animator.Play("DialogPanel2Close");

        // Возвращаем управление игроку
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Сброс флагов
        isDialogAnimating = false;
    }

    private System.Collections.IEnumerator SwitchDialog(
        Animator closingAnimator, string closeTrigger,
        Animator openingAnimator, string openTrigger,
        TMP_Text nextText, string nextLine,
        bool nextDialogIs1)
    {
        isDialogAnimating = true;

        if (closingAnimator != null) closingAnimator.Play(closeTrigger);
        // Подстраховка: ждём, но лучше синхронизировать с длиной анимации
        yield return new WaitForSeconds(0.3f);

        showingDialog1 = nextDialogIs1;

        if (nextText != null) nextText.text = nextLine;
        if (openingAnimator != null) openingAnimator.Play(openTrigger);

        yield return new WaitForSeconds(0.3f);
        isDialogAnimating = false;
    }
}
