using System.Linq;
using UnityEngine;

public class NazarenoClickable : Clickable
{
    private ControladorNazareno nazareno;
    private InfoPanelUI infoPanel;

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


    public override void OnShortClick(Vector2 mousePos)
    {
        if (nazareno == null)
        {
            nazareno = GetComponent<ControladorNazareno>();
            if (nazareno == null)
            {
                return;
            }
        }

        ShowInfo(ShopManager.instance.Data.Items[nazareno.nombre]);

    }

    public void ShowInfo(ItemInfo itemInfo)
    {
        if (infoPanel == null)
        {
            infoPanel = Resources.FindObjectsOfTypeAll<InfoPanelUI>().First();
        }

        if (infoPanel != null)
        {
            infoPanel.Show(itemInfo);
        }
    }
}
