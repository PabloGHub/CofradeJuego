using UnityEngine;

public class NazarenoClickable : Clickable
{
    private ControladorNazareno nazareno;

    public override void OnClick(Vector2 mousePos) 
    {
        if (nazareno == null)
        {
            nazareno = GetComponent<ControladorNazareno>();
            if (nazareno == null)
            {
                return;
            }
        }

        ShopManager.instance.dragElement.GetComponent<DraggingBehavior>().Drag(ShopManager.instance.Data.Items[nazareno.nombre], gameObject);
    }
}
