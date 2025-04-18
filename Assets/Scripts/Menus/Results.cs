using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Results : MonoBehaviour
{
    private TextMeshProUGUI ResultText;
    private TextMeshProUGUI DetailsText;

    private Button MainMenu;
    private Button Retry;
    private Button NextLevel;
    private Image image;


    void Setup()
    {
        ResultText = transform.Find("ResultText")?.GetComponent<TextMeshProUGUI>();
        DetailsText = transform.Find("DetailsText")?.GetComponent<TextMeshProUGUI>();
        MainMenu = transform.Find("MenuButton")?.GetComponent<Button>();
        Retry = transform.Find("RestartButton")?.GetComponent<Button>();
        NextLevel = transform.Find("NextButton")?.GetComponent<Button>();
        image = transform.Find("Image")?.GetComponent<Image>();

        if (ResultText == null || DetailsText == null)
        {
            Debug.LogWarning("Results: Some UI elements are missing!");
        }
    }

    public void ShowResults(bool hasWon, float score, float time, float wax)
    {
        if (ResultText == null) Setup();

        ResultText.text = hasWon ? "YOU WON" : "YOU LOST";
        DetailsText.text = "Time: " + time + "\nWax: " + wax + "\nScore: " + score;

        MainMenu.gameObject.SetActive(true);
        Retry.gameObject.SetActive(true);
        NextLevel.gameObject.SetActive(true);
        image.gameObject.SetActive(false);

        gameObject.SetActive(true);
    }

    public void ShowPause()
    {
        if (ResultText == null) Setup();

        ResultText.text = "PAUSE";
        DetailsText.text = "";

        MainMenu.gameObject.SetActive(true);
        Retry.gameObject.SetActive(true);
        NextLevel.gameObject.SetActive(false);
        image.gameObject.SetActive(true);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
