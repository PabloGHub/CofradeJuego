using UnityEngine;

public class NPCInfo : MonoBehaviour
{
    public Transform target;
    private Camera mainCamera;
    private RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (target == null || mainCamera == null)
            return;

        Vector3 worldPos = target.position;
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPos);

        // Optional: hide UI if behind camera
        if (screenPoint.z < 0)
        {
            rectTransform.gameObject.SetActive(false);
        }
        else
        {
            rectTransform.gameObject.SetActive(true);
            rectTransform.anchoredPosition = screenPoint;
        }
    }
}
