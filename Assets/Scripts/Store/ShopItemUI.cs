using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI priceText;
    private Button buyButton;

    void Awake()
    {
        nameText = transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
        descriptionText = transform.Find("ItemDesc")?.GetComponent<TextMeshProUGUI>();
        priceText = transform.Find("BuyButton/ItemPrice")?.GetComponent<TextMeshProUGUI>();
        buyButton = transform.Find("BuyButton")?.GetComponent<Button>();

        if (nameText == null || descriptionText == null || priceText == null || buyButton == null)
        {
            Debug.LogWarning("ShopEntryUI: Some UI elements are missing!");
        }
    }

    public void Setup(ItemInfo item)
    {
        nameText.text = item.Name;
        descriptionText.text = item.Description;
        priceText.text = "$" + item.Price.ToString("F2");
        buyButton.onClick.AddListener(() => Buy(item));
    }

    void Buy(ItemInfo item)
    {
        Debug.Log("Buying " + item.Name);
    }
}
