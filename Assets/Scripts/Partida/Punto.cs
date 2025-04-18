using UnityEngine;
using UnityEngine.UI;

public class Punto : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    public bool Difurcacion = false;

    //[HideInInspector]
    public bool Elegido_b = false;

    public float DistanciaAlSiguiente_f = 0f;

    public int Indice_i = -1;

    private SpriteRenderer image;
    private SpriteRenderer selectorImage;

    // ***********************( Funciones Unity )*********************** //

    private void Start()
    {
        image = GetComponent<SpriteRenderer>();
        selectorImage = transform.Find("Selector")?.GetComponent<SpriteRenderer>();
        selectorImage.gameObject.SetActive(false);
        if (Difurcacion)
        {
            image.gameObject.SetActive(true);
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }

    // ***********************( Funciones Nuestras )*********************** //

    public void ToggleChosen()
    {
        Elegido_b = !Elegido_b;
        selectorImage.gameObject.SetActive(Elegido_b);
    }
}
