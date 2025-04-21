using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{
    static public ShopManager instance;

    [HideInInspector] public ShopData Data;
    public GameObject shopEntryPrefab;
    public GameObject dragElement;
    public GameObject trashElement;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private string MoneyTextStart;

    public float startingMoney;
    private float moneyAvailable;
    public float MoneyAvailable => moneyAvailable;

    public static event Action OnMoneyUpdated;

    public GameObject amarillo;
    public GameObject azul;
    public GameObject rojo;
    public GameObject morado;
    public GameObject verde;

    public Sprite amarilloIm;
    public Sprite azulIm;
    public Sprite rojoIm;
    public Sprite moradoIm;
    public Sprite verdeIm;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Data = Resources.Load<ShopData>("ShopData");

        if (Data == null)
        {
            Data = new ShopData();
            ItemInfo ii = new ItemInfo();
            ii.Name = "Amarillo";
            ii.Price = 10;
            ii.sprite = amarilloIm;
            ii.dropObject = amarillo;
            Data.Items["Amarillo"] = ii;
            ii.Name = "Azul";
            ii.Price = 10;
            ii.sprite = azulIm;
            ii.dropObject = azul;
            Data.Items["Azul"] = ii;
            ii.Name = "Verde";
            ii.Price = 10;
            ii.sprite = verdeIm;
            ii.dropObject = verde;
            Data.Items["Verde"] = ii;
            ii.Name = "Rojo";
            ii.Price = 5;
            ii.sprite = rojoIm;
            ii.dropObject = rojo;
            Data.Items["Rojo"] = ii;
            ii.Name = "Morado";
            ii.Price = 15;
            ii.sprite = moradoIm;
            ii.dropObject = morado;
            Data.Items["Morado"] = ii;
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
