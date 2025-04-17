using System;
using UnityEngine;

public class Clickable : MonoBehaviour
{
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
                    clickable.OnClick(Input.mousePosition);
                    break;
                }
            }
        }
    }

    public virtual void OnClick(Vector2 mousePos)
    {

    }
}
