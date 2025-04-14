using UnityEngine;

public class NazarenoClickable : Clickable
{
    private NazarenoBase nazareno;

    public override void OnClick(Vector2 mousePos) 
    {
        if (nazareno == null)
        {
            nazareno = GetComponent<NazarenoBase>();
            if (nazareno == null)
            {
                return;
            }
        }

        ShopManager.instance.dragElement.GetComponent<DraggingBehavior>().Drag(ShopManager.instance.Data.Items[nazareno.name], gameObject);
    }
}
