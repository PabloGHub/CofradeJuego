using System.Linq;
using UnityEngine;

public class PuntoClickable : Clickable
{
    private Punto punto;

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

        punto.ToggleChosen();

    }
}
