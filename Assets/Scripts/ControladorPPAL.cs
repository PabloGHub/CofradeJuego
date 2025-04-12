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

    [RegisterCommand(Help = "pausa/desapausa")]
    static void CommandPausa(CommandArg[] args)
    {
        v_pausado_b = !v_pausado_b;
        Terminal.Log("Pausado: " + v_pausado_b);
    }
}
