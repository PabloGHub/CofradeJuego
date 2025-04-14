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

    private void Start()
    {
        ShopManager.OnMoneyUpdated += UpdateMoneyState;
    }

    public void Setup(ItemInfo item)
    {
        nameText.text = item.Name;
        descriptionText.text = item.Description;
        priceText.text = "$" + item.Price.ToString("F2");
        itemInfo = item;

        draggingBehavior = ShopManager.instance.dragElement.GetComponent<DraggingBehavior>();
        if (draggingBehavior == null)
        {
            Debug.LogWarning("ShopEntryUI: No draggingBehavior found");
        }

        //Transform draggerContainer = GameObject.Find("DraggerContainer").transform;
        //if (draggerContainer != null)
        //{
        //    GameObject dragger = Instantiate(itemInfo.dragObject, draggerContainer);
        //    dragger.SetActive(false);
        //    draggingBehavior = dragger.GetComponent<DraggingBehavior>();
        //    draggingBehavior.SetItemInfo(itemInfo);
        //}
    }

    void Buy()
    {
        draggingBehavior.Drag(itemInfo);
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

    private void UpdateMoneyState()
    {
        if (ShopManager.instance.MoneyAvailable < itemInfo.Price)
        {
            priceText.color = Color.red;
        }
        else
        {
            priceText.color = Color.black;
        }
    }
}
