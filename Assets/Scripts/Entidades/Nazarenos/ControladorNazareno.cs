using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ControladorNazareno : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
    private float _cercaniaAlObjetivo = 3f;
    public int ObjetivoIndex_i = 0;
    [HideInInspector] public Movimiento v_movimiento;
    [HideInInspector] public Transform v_objetivo_t;
    [HideInInspector] public Transform v_puntoObjetivo_t;

    // Datos
    public string nombre;
    [HideInInspector] public int id;

    // Ataque
    private Ataque v_ataque_s;

    // Animaciones
    private AnimacionesMovimiento _animaciones_s;

    // --- Maquina de Estados --- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }

    // ***********************( Funciones Unity )*********************** //
    private void Start()
    {
        // Ataqie
        v_ataque_s = GetComponent<Ataque>();
        if (v_ataque_s != null)
        {
            v_ataque_s.OnSinEnemigos += () => CambiarEstado(0);
            v_ataque_s.OnEnemigosCerca += () => CambiarEstado(1);
        }
        else
            Debug.LogError($"****** Nazareno: {gameObject.name} NO tiene componente (Ataque) ******");

        
        v_puntoObjetivo_t = Navegacion.nav.trayectoria[ObjetivoIndex_i];
        v_objetivo_t = v_puntoObjetivo_t;

        // Movimiento
        v_movimiento = GetComponent<Movimiento>();
        if (v_movimiento != null)
            v_movimiento.v_objetivo_t = v_objetivo_t; // Puede que si esta atacando, gire raro.
        else
            Debug.LogError($"****** Nazareno: {gameObject.name} NO tiene componente (Movimiento) ******");
        

        // Inicializar Maquina de Estados
        Inicializar(gameObject);
        estadosPosibles = new List<EstadoBase>
        {
            CrearEstado<EstadoNada, ControladorNazareno>(this),
            CrearEstado<EstadoAtacar, ControladorNazareno>(this)
        };
        subEstadosPosibles = new List<EstadoBase>
        {
            CrearEstado<EstadoLejosAdelantado, ControladorNazareno>(this),
            CrearEstado<EstadoLejosMedio, ControladorNazareno>(this),
            CrearEstado<EstadoLejosAtrasado, ControladorNazareno>(this),
            CrearEstado<EstadoCerca, ControladorNazareno>(this)
        };
        CambiarEstado(0);
        CambiarSubEstado(3);

        // Animaciones
        for (int i = 0; i < transform.childCount; i++)
        {
            AnimacionesMovimiento _anim = transform.GetChild(i).GetComponent<AnimacionesMovimiento>();
            if (_anim != null)
                _animaciones_s = _anim;
        }
        if (_animaciones_s == null)
            Debug.LogError($"****** Nazareno: {gameObject.name} NO tiene componente (Animaciones) ******");
    }

    private void Update()
    {
        if (ControladorPPAL.V_pausado_b)
            return;

        if (v_movimiento != null)
            v_movimiento.v_objetivo_t = v_objetivo_t;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("puntoControl") && collision.gameObject == v_puntoObjetivo_t.gameObject)
        {
            if (v_movimiento.v_objetivo_t == null)
                return;

            if (Vector3.Distance(transform.position, v_movimiento.v_objetivo_t.position) < _cercaniaAlObjetivo)
            {
                //Debug.Log("Llegamos al punto de control: " + v_puntoObjetivo_t.name);
                actualizarPunto();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("puntoControl") && collision.gameObject == v_puntoObjetivo_t.gameObject)
        {
            if (v_movimiento.v_objetivo_t == null)
                return;

            if (Vector3.Distance(transform.position, v_movimiento.v_objetivo_t.position) < _cercaniaAlObjetivo)
            {
                //Debug.Log("Llegamos al punto de control: " + v_puntoObjetivo_t.name);
                actualizarPunto();
            }
        }
    }



    // ***********************( Funciones Nuestras )*********************** //
    void extracion()
    {
        // TODO: cuando llega a carrera destuir o ocular al nazareno.
    }

    private void actualizarPunto()
    {
        ObjetivoIndex_i++;

        if (ObjetivoIndex_i >= Navegacion.nav.trayectoria.Length)
            extracion();

        while (true)
        {
            Punto punto = Navegacion.nav.trayectoria[ObjetivoIndex_i].GetComponent<Punto>();

            if (!punto.Difurcacion)
            {
                break;
            }
            else if (!punto.Elegido_b)
            {
                ObjetivoIndex_i++;
            }
            else
            {
                break;
            }
        }

        v_puntoObjetivo_t = Navegacion.nav.trayectoria[ObjetivoIndex_i];
        v_objetivo_t = v_puntoObjetivo_t;
        v_movimiento.v_objetivo_t = v_objetivo_t; // Puede que si esta atacando, gire raro.
    }

    // ***********************( Estados de la MAQUINA DE ESTADOS )*********************** //
    // ************ Estado de Movimiento ************ //
    // --- LEJOS --- //
    class EstadoLejosAdelantado : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }


        public override void Entrar()
        {
            v_controladorNazareno_s.v_movimiento.v_esperando_b = true;
            v_controladorNazareno_s.v_movimiento.v_exodia_b = false;
        }
        public override void Salir() { }
    }
    class EstadoLejosMedio : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }


        public override void Entrar()
        {
            v_controladorNazareno_s.v_movimiento.v_esperando_b = false;
            v_controladorNazareno_s.v_movimiento.v_exodia_b = false;
        }
        public override void Salir() { }
    }
    class EstadoLejosAtrasado : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }


        public override void Entrar()
        {
            v_controladorNazareno_s.v_movimiento.v_esperando_b = false;
            v_controladorNazareno_s.v_movimiento.v_exodia_b = true;
        }
        public override void Salir() { }
    }


    // --- CERCA --- //
    class EstadoCerca : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }

        public override void Entrar()
        {
            v_controladorNazareno_s.v_movimiento.v_esperando_b = false;
            v_controladorNazareno_s.v_movimiento.v_exodia_b = false;
        }
        public override void Salir() { }
    }


    // ************ Estado de ATACAR ************ //
    // --- ATACANDO --- //
    class EstadoNada : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }

        public override void Entrar() 
        {
            v_controladorNazareno_s.v_objetivo_t = v_controladorNazareno_s.v_puntoObjetivo_t;

            if (v_controladorNazareno_s.v_ataque_s == null)
                return;

            v_controladorNazareno_s.v_ataque_s._atacar_b = false;
        }
        public override void Salir() { }
    }
    class EstadoAtacar : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }

        public override void Entrar()
        {
            if (v_controladorNazareno_s.v_movimiento == null)
                return;

            v_controladorNazareno_s.v_movimiento.v_esperando_b = false;
            v_controladorNazareno_s.v_movimiento.v_exodia_b = true;
        }
        public override void Salir()
        {
            if (v_controladorNazareno_s.v_movimiento == null)
                return;

            v_controladorNazareno_s.v_movimiento.v_esperando_b = false;
            v_controladorNazareno_s.v_movimiento.v_exodia_b = false;
        }

        public override void MiFixedUpdate()
        {
            try
            {
                if (v_controladorNazareno_s.v_ataque_s == null)
                    return;

                v_controladorNazareno_s.v_ataque_s._atacar_b = true;
                Transform _nuevoObjetivo_t = v_controladorNazareno_s.v_ataque_s.EnemigoObjetivo_go.transform;
                if (_nuevoObjetivo_t != null)
                    v_controladorNazareno_s.v_objetivo_t = _nuevoObjetivo_t;
                else
                    Debug.LogWarning("--- No hay enemigo objetivo ---");
            }
            catch (Exception e)
            {
                Debug.LogError($"****( OBJETO: {gameObject.name} -> {e} )****");
            }
        }
    }


    // ----------( Funciones de Debug )---------- //
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        {
            Handles.color = Color.red;
            Handles.Label(transform.position + Vector3.up * 0.3f,
                                    $"CONTROLADOR :  id -> {id} | nombre -> {nombre} | " +
                                    $"estado -> {EstadoActual} | subEstado -> {SubEstadoActual}");
        }
        #endif
    }

}