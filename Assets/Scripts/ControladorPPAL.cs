using CommandTerminal;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class ControladorPPAL : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
    [Header("**---- Niveles ----**")]
    public int NivelActual_i = 0;

    [Header("**---- Limites de la camara ----**")]
    public Vector2 Esquina1_v2;
    public Vector2 Esquina2_v2;

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

            if (!ppal.EnCurso_f && !_pausado_b)
            {
                ppal.EnCurso_f = true;
                OnIniciar?.Invoke();
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

    public bool EnCurso_f = false;

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

        if (!EnCurso_f && !_pausado_b)
        {
            EnCurso_f = true;
            OnIniciar?.Invoke();
        }

        Terminal.Log("Pausado: " + _pausado_b);
    }

    public static void Reiniciar()
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
        //ControladorPPAL.ppal.reiniciar();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(Esquina1_v2.x, Esquina1_v2.y, 0), new Vector3(Esquina1_v2.x, Esquina2_v2.y, 0));
        Gizmos.DrawLine(new Vector3(Esquina1_v2.x, Esquina1_v2.y, 0), new Vector3(Esquina2_v2.x, Esquina1_v2.y, 0));
        Gizmos.DrawLine(new Vector3(Esquina2_v2.x, Esquina1_v2.y, 0), new Vector3(Esquina2_v2.x, Esquina2_v2.y, 0));
        Gizmos.DrawLine(new Vector3(Esquina2_v2.x, Esquina2_v2.y, 0), new Vector3(Esquina1_v2.x, Esquina2_v2.y, 0));
    }

    // ***********************( Clases de Estados )*********************** //
}
