using CommandTerminal;
using UnityEngine;

public class ControladorPPAL : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    public static ControladorPPAL ppal;

    [HideInInspector]
    public static bool v_pausado_b = true;

    // ***********************( Funciones Unity )*********************** //
    private void Awake()
    {
        if (ppal != this)
        {
            ppal = this;
        }
    }
    // ***********************( Funciones Nuestras )*********************** //
    private void cabiarPausa()
    {
        if (Navegacion.nav.comprobarCaminos())
            v_pausado_b = !v_pausado_b;

        Terminal.Log("Pausado: " + v_pausado_b);
    }


    [RegisterCommand(Help = "pausa/desapausa")]
    static void CommandPausa(CommandArg[] args)
    {
        ControladorPPAL.ppal.cabiarPausa();
    }
}
