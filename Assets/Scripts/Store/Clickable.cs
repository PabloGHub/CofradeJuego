using System;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Clickable clickable = hit.collider.GetComponent<Clickable>();
                if (clickable != null)
                {
                    clickable.OnClick(Input.mousePosition); // or pass world pos if needed
                }
            }
        }
    }

    public virtual void OnClick(Vector2 mousePos)
    {

    }
}
