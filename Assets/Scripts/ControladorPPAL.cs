using CommandTerminal;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class ControladorPPAL : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    public static ControladorPPAL ppal;

    [HideInInspector]
    public static bool v_pausado_b = true;
    public static event Action<bool> OnCambioPausa;
    public static event Action OnReiniciar;
    public static event Action OnIniciar;
    public static event Action IntanciarEnemigos;

    [SerializeField] private TextMeshProUGUI PausaBotonTexto;

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
        {
            v_pausado_b = !v_pausado_b;
            OnCambioPausa?.Invoke(v_pausado_b);
        }

        Terminal.Log("Pausado: " + v_pausado_b);
    }
    private void reiniciar()
    {
        Navegacion.nav.Reiniciar(); // No hace nada
        // TODO: Devolver cuantia al jugador.
        OnReiniciar?.Invoke();
    }


    [RegisterCommand(Help = "pausa/desapausa")]
    static void CommandPausa(CommandArg[] args)
    {
        ControladorPPAL.ppal.cabiarPausa();
    }

    [RegisterCommand(Help = "Instanciar enemigos")]
    static void CommandInst(CommandArg[] args)
    {
        Debug.Log("Intentando Intanciar");
        ControladorPPAL.IntanciarEnemigos?.Invoke();
    }

    [RegisterCommand(Help = "Reiniciar Nivel")]
    static void CommandRei(CommandArg[] args)
    {
        Debug.Log("Reiniciando...");
        ControladorPPAL.ppal.reiniciar();
    }


    public void PauseFromUI()
    {
        cabiarPausa();
        PausaBotonTexto.text = v_pausado_b ? "RESUME" : "PAUSE";
    }
}
