using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Results : MonoBehaviour
{
    private TextMeshProUGUI ResultText;
    private TextMeshProUGUI DetailsText;


    void Start()
    {
        ResultText = transform.Find("ResultText")?.GetComponent<TextMeshProUGUI>();
        DetailsText = transform.Find("DetailsText")?.GetComponent<TextMeshProUGUI>();

        if (ResultText == null || DetailsText == null)
        {
            Debug.LogWarning("Results: Some UI elements are missing!");
        }
    }

    public void ShowResults(bool hasWon, float score, float time, float wax)
    {
        ResultText.text = hasWon ? "YOU WON" : "YOU LOST";
        DetailsText.text = "Time: " + time + "\nWax: " + wax + "\nScore: " + score;
    }
}
