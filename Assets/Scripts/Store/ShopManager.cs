using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{
    static public ShopManager instance;

    public ShopData Data;
    public GameObject shopEntryPrefab;
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
        if (Data == null)
        {
            Debug.LogError("ShopData est� null en ShopManager");
            return;
        }

        if (Data.Items == null || Data.Items.Count == 0)
        {
            Debug.LogWarning("Items no estaba inicializado, forzando OnEnable...");
            Data.GetType().GetMethod("OnEnable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(Data, null);
        }

        SetMoney(startingMoney);
        GenerateShop();
    }

    void GenerateShop()
    {
        foreach (var item in Data.Items)
        {
            GameObject entry = Instantiate(shopEntryPrefab, transform);
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

    public void ResetPreparations()
    {
        //if (ControladorPPAL.ppal.EnCurso_f)
        //{
        //    ControladorPPAL.Reiniciar();
        //    return;
        //}
        float moneyRecovered = Peloton.peloton.DevolverIntegrantesTotal();
        AddMoney(moneyRecovered);
    }

}
