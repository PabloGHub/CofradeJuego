using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggingBehavior : MonoBehaviour
{
    private Image dragIcon;
    private Camera worldCamera;
    private ItemInfo itemInfo;
    private GameObject NPC;

    private bool isDragging = false;

    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;


    void Awake()
    {
        dragIcon = transform.Find("Image")?.GetComponent<Image>();
        if (dragIcon == null )
        {
            Debug.LogWarning("DraggingBehavior: Some UI elements are missing!");
        }
        worldCamera = Camera.main;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        raycaster = GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        if (isDragging)
        {
            SetPosition();

            if (Input.GetMouseButtonUp(0))
            {
                Drop();
            }
        }
    }

    public void Setup(ItemInfo info, GameObject go = null)
    {
        dragIcon.sprite = info.sprite;
        itemInfo = info;
        NPC = go;
    }

    public void Drag(ItemInfo info, GameObject go = null)
    {
        if (ShopManager.instance.MoneyAvailable >= itemInfo.Price)
        {
            Setup(info, go);
            isDragging = true;
            SetPosition();
            gameObject.SetActive(true);
        }
        if (go != null)
        {
            ShopManager.instance.trashElement.SetActive(true);
        }
    }

    void SetPosition()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragIcon.canvas.GetComponent<RectTransform>(),
            Input.mousePosition,
            null,
            out mousePos
        );
        dragIcon.rectTransform.anchoredPosition = mousePos;
    }

    void Drop()
    {
        isDragging = false;
        gameObject.SetActive(false);

        Vector3 mouseWorldPos = worldCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        if (NPC == null)
        {
            if (itemInfo.dropObject != null && Peloton.peloton != null)
            {
                bool success = Peloton.peloton.TryToDropMember(itemInfo, mouseWorldPos);
                if (success)
                {
                    ShopManager.instance.AddMoney(-itemInfo.Price);
                }
            }
        }
        else
        {
            //PointerEventData pointerEventData = new PointerEventData(eventSystem);
            //pointerEventData.position = Input.mousePosition;

            //List<RaycastResult> results = new List<RaycastResult>();
            //raycaster.Raycast(pointerEventData, results);

            //foreach (RaycastResult result in results)
            //{
            //    if (result.gameObject == ShopManager.instance.trashElement)
            //    {
            //        Peloton.peloton.EliminarIntegrante(NPC);
            //        ShopManager.instance.AddMoney(itemInfo.Price);
            //        break;
            //    }
            //}
            //ShopManager.instance.trashElement.SetActive(false);
        }
    }

}
