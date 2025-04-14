using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{
    static public ShopManager instance;

    public ShopData Data;
    public GameObject shopEntryPrefab;
    public Transform shopListContainer;
    public GameObject dragElement;
    public GameObject trashElement;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private string MoneyTextStart;

    public float startingMoney;
    private float moneyAvailable;
    public float MoneyAvailable => moneyAvailable;

    public static event Action OnMoneyUpdated;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        SetMoney(startingMoney);
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

    public void SetMoney(float amount)
    {
        moneyAvailable = amount;
        moneyText.text = MoneyTextStart + moneyAvailable.ToString();
        OnMoneyUpdated?.Invoke();
    }

    public void AddMoney(float amount)
    {
        moneyAvailable += amount;
        moneyText.text = MoneyTextStart + moneyAvailable.ToString();
        OnMoneyUpdated?.Invoke();
    }

}
