using UnityEngine;

public class Navegacion : MonoBehaviour
{
    // TODO: Hacer herramienta para crear trayectorias.
    // ***********************( Declaraciones )*********************** //
    public static Navegacion nav;

    public Transform[] trayectoria;

    // ***********************( Funciones Unity )*********************** //
    private void Awake()
    {
        if (nav != this)
        {
            nav = this;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    // ***********************( Funciones Nuestras )*********************** //
}
