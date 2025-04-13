using UnityEngine;
using UnityEngine.UI;

public class DraggingBehavior : MonoBehaviour
{
    public Image dragIcon;
    private Camera worldCamera;
    private GameObject itemToPlace;

    private bool isDragging = false;


    void Awake()
    {
        dragIcon = transform.Find("Image")?.GetComponent<Image>();
        if (dragIcon == null )
        {
            Debug.LogWarning("DraggingBehavior: Some UI elements are missing!");
        }
        worldCamera = Camera.main;
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

    public void Drag(GameObject item)
    {
        isDragging = true;
        itemToPlace = item;
        SetPosition();
        gameObject.SetActive(true);
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

        if (itemToPlace != null && Peloton.peloton != null)
        {
            Peloton.peloton.TryToDropMember(itemToPlace, mouseWorldPos);
        }
    }

}
