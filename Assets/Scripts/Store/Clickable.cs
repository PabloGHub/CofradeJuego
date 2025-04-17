using System;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    private bool isPressed = false;
    private float pressTime = 0f;
    private float longPressThreshold = 0.3f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

            foreach (RaycastHit2D hit in hits)
            {
                Clickable clickable = hit.collider.GetComponent<Clickable>();
                if (clickable != null)
                {
                    clickable.isPressed = true;
                    clickable.pressTime = 0f;
                    break;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (isPressed)
            {
                pressTime += Time.deltaTime;
                if (pressTime >= longPressThreshold)
                {
                    isPressed = false; // prevent re-trigger
                    OnClick(Input.mousePosition);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isPressed && pressTime < longPressThreshold)
            {
                OnShortClick(Input.mousePosition);
            }
            isPressed = false;
        }
    }

    public virtual void OnClick(Vector2 mousePos)
    {

    }
    public virtual void OnShortClick(Vector2 mousePos)
    {

    }
}
