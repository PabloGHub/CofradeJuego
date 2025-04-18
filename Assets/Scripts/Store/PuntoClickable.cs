using System.Linq;
using UnityEngine;

public class PuntoClickable : Clickable
{
    private Punto punto;
    public float radius = 0.5f;

    public override void OnShortClick(Vector2 mousePos)
    {
        if (punto == null)
        {
            punto = GetComponent<Punto>();
            if (punto == null)
            {
                return;
            }
        }
        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseInWorld.z = 0;
        float dist = Vector4.Distance(transform.position, mouseInWorld);
        if (dist <= radius)
        {
            punto.ToggleChosen();
        }
    }
}
