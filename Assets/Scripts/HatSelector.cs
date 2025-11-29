using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HatSelector : MonoBehaviour
{
    [Header("Hats")]
    public List<GameObject> hats;
    private int currentIndex = 0;

    [Header("Sound Settings")]
    public AudioSource audioSource;   // Источник звука
    public AudioClip clickSound;      // Звук при нажатии кнопки

    private void Start()
    {
        currentIndex = PlayerPrefs.GetInt("SelectedHatID", 0);
        UpdateHats();
    }

    public void NextHat()
    {
        PlayClick();

        currentIndex++;
        if (currentIndex >= hats.Count)
            currentIndex = 0;

        UpdateHats();
    }

    public void PreviousHat()
    {
        PlayClick();

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = hats.Count - 1;

        UpdateHats();
    }

    public void SelectHat()
    {
        PlayClick();

        PlayerPrefs.SetInt("SelectedHatID", currentIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Level1");
    }

    private void UpdateHats()
    {
        for (int i = 0; i < hats.Count; i++)
            hats[i].SetActive(i == currentIndex);
    }

    // Воспроизведение звука
    private void PlayClick()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}
