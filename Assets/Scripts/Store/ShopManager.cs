using UnityEngine;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{
    public ShopData Data;
    public GameObject shopEntryPrefab;
    public Transform shopListContainer;

    void Start()
    {
        GenerateShop();
    }

    void GenerateShop()
    {
        foreach (var item in Data.Items)
        {
            GameObject entry = Instantiate(shopEntryPrefab, shopListContainer);
            ShopItemUI entryUI = entry.GetComponent<ShopItemUI>();
            entryUI.Setup(item.Value);
        }
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 1;
    }


}
