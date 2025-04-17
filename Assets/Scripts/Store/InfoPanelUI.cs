using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelUI : MonoBehaviour
{
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI descriptionText;
    private bool setup = false;

    void Setup()
    {
        nameText = transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        descriptionText = transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
        if (nameText == null || descriptionText == null)
        {
            Debug.LogWarning("InfoPanelUI: Some UI elements are missing!");
        }
        setup = true;
    }

    public void Show(ItemInfo info)
    {
        if (!setup)
        {
            Setup();
        }
        nameText.text = info.Name;
        descriptionText.text = info.Description;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }
}
