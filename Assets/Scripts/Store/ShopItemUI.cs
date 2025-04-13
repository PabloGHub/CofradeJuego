using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI priceText;
    private ScrollRect scrollRect;
    private DraggingBehavior draggingBehavior;

    private ItemInfo itemInfo;

    void Awake()
    {
        nameText = transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
        descriptionText = transform.Find("ItemDesc")?.GetComponent<TextMeshProUGUI>();
        priceText = transform.Find("ItemPrice")?.GetComponent<TextMeshProUGUI>();
        scrollRect = GetComponentInParent<ScrollRect>();

        if (nameText == null || descriptionText == null || priceText == null || scrollRect == null)
        {
            Debug.LogWarning("ShopEntryUI: Some UI elements are missing!");
        }
    }

    public void Setup(ItemInfo item)
    {
        nameText.text = item.Name;
        descriptionText.text = item.Description;
        priceText.text = "$" + item.Price.ToString("F2");
        itemInfo = item;

        Transform draggerContainer = GameObject.Find("DraggerContainer").transform;
        if (draggerContainer != null)
        {
            GameObject dragger = Instantiate(itemInfo.dragObject, draggerContainer);
            dragger.SetActive(false);
            draggingBehavior = dragger.GetComponent<DraggingBehavior>();
        }
        else
        {
            Debug.LogWarning("ShopEntryUI: No DraggerContainer found");
        }
    }

    void Buy()
    {
        draggingBehavior.Drag(itemInfo.dropObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        EnableScrolling(false);
        Buy();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EnableScrolling(true);
    }

    public void EnableScrolling(bool enable)
    {
        if (scrollRect != null)
        {
            scrollRect.enabled = enable;
        }
    }

}
