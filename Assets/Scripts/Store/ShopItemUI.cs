using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private TextMeshProUGUI nameText;
    //private TextMeshProUGUI descriptionText;
    private Image image;
    private Button button;
    private TextMeshProUGUI priceText;
    private ScrollRect scrollRect;
    private DraggingBehavior draggingBehavior;
    private InfoPanelUI infoPanel;

    private ItemInfo itemInfo;

    void Awake()
    {
        nameText = transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
        //descriptionText = transform.Find("ItemDesc")?.GetComponent<TextMeshProUGUI>();
        priceText = transform.Find("ItemPrice")?.GetComponent<TextMeshProUGUI>();
        image = transform.Find("Image")?.GetComponent<Image>();
        scrollRect = GetComponentInParent<ScrollRect>();
        button = transform.Find("plus")?.GetComponent<Button>();

        if (nameText == null || image == null || priceText == null || scrollRect == null || button == null)
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
        // descriptionText.text = item.Description;
        image.sprite = item.sprite;
        priceText.text = "$" + item.Price.ToString("F2");
        itemInfo = item;

        draggingBehavior = ShopManager.instance.dragElement.GetComponent<DraggingBehavior>();
        if (draggingBehavior == null)
        {
            Debug.LogWarning("ShopEntryUI: No draggingBehavior found");
        }
    }

    void Buy()
    {
        draggingBehavior.Drag(itemInfo);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (ControladorPPAL.ppal.EnCurso_f) return;

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
            priceText.color = Color.white;
        }
    }

    public void ShowInfo()
    {
        if (infoPanel == null)
        {
            infoPanel = Resources.FindObjectsOfTypeAll<InfoPanelUI>().First();
        }

        if (infoPanel != null)
        {
            infoPanel.Show(itemInfo);
        }
    }
}
