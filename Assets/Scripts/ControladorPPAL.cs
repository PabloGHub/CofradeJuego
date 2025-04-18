using CommandTerminal;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class ControladorPPAL : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
    public static ControladorPPAL ppal;

    private static bool _pausado_b = true;
    public static bool V_pausado_b
    {
        get { return _pausado_b; }
        set
        {
            if (Navegacion.nav.comprobarCaminos())
            {
                _pausado_b = !_pausado_b;
                OnCambioPausa?.Invoke(_pausado_b);
            }

            Terminal.Log("Pausado: " + V_pausado_b);
        }
    }

    public static event Action<bool> OnCambioPausa;
    public static event Action OnReiniciar;
    public static event Action OnIniciar;
    public static event Action IntanciarEnemigos;

    public List<GameObject> Porculeros;

    [SerializeField] private TextMeshProUGUI PausaBotonTexto;

    // --- ( Estados ) --- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }

    // ***********************( Funciones Unity )*********************** //
    private void Awake()
    {
        ppal = this;
        _pausado_b = true;
        Porculeros = new List<GameObject>();
    }

    // ***********************( Metodos Nuestras )*********************** //
    private void cabiarPausa()
    {
        if (Navegacion.nav.comprobarCaminos())
        {
            _pausado_b = !_pausado_b;
            OnCambioPausa?.Invoke(_pausado_b);
        }

        Terminal.Log("Pausado: " + _pausado_b);
    }
    private void reiniciar()
    {
        //Navegacion.nav.Reiniciar(); // No hace nada
        //Peloton.peloton.Reiniciar(); 
        // TODO: Devolver cuantia al jugador.
        OnReiniciar?.Invoke();
    }


    public void EliminarDeLaLista(GameObject _objeto_go)
    {
        if (Porculeros.Contains(_objeto_go))
        {
            Porculeros.Remove(_objeto_go);
        }
    }


    public void PauseFromUI()
    {
        cabiarPausa();
        PausaBotonTexto.text = V_pausado_b ? "CONTINUAR" : "PAUSAR";
    }

    // ***********************( Comandos y Debug )*********************** //
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



    // ***********************( Clases de Estados )*********************** //
}
