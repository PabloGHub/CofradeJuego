using UnityEngine;
using UnityEngine.UI;

public class Lifebar : MonoBehaviour
{
    private RectTransform filler;

    public float objHP;
    public float maxHP;

    private float barHP;
    private float fullWidth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fullWidth = GetComponent<RectTransform>().rect.width;
        filler = transform.Find("Filler")?.GetComponent<RectTransform>();

        if (filler == null)
        {
            Debug.LogWarning("Lifebar: Some UI elements are missing!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (objHP != barHP)
        {
            if (objHP < barHP) barHP-=0.5f;
            else barHP+=0.5f;

            if (barHP > maxHP) barHP = maxHP;
            else if (barHP < 0) barHP = 0;

            float prct = barHP / maxHP; 
            float rightOffset = -(1f - prct) * fullWidth;

            filler.offsetMax = new Vector2(rightOffset, filler.offsetMax.y);

        }
    }
}
