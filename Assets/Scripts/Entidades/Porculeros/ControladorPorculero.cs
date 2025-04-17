using System;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class ControladorPorculero : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
     [HideInInspector] public Movimiento Movimiento_s;
     private Transform v_objetivo_t;

    // Ataque
    private Ataque v_ataque_s;

    // --- Maquina de Estados --- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }


    // ***********************( Metodos UNITY )*********************** //
    private void Awake()
    {
        // Maquina de Estados
        Inicializar(gameObject);
        estadosPosibles = new List<EstadoBase>
        {
            CrearEstado<EstadoParado, ControladorPorculero>(this),
            CrearEstado<EstadoPersiguiendo, ControladorPorculero>(this),
            CrearEstado<EstadoAtacando, ControladorPorculero>(this)
        };
    }

    private void Start()
    {
        // Movimiento
        Movimiento_s = GetComponent<Movimiento>();
        if (Movimiento_s == null)
            Debug.LogError($"****** Porculero: {gameObject.name} NO tiene componente (Movimiento) ******");
          

        // Ataque
        v_ataque_s = GetComponent<Ataque>();
        if (v_ataque_s != null)
        {
            v_ataque_s.OnSinEnemigos += sinEnemigos;
            v_ataque_s.OnEnemigosCerca += () => CambiarEstado(2);
        }
        else
            Debug.LogError($"****** Porculero: {gameObject.name} NO tiene componente (Ataque) ******");

        CambiarEstado(0);
    }

    private void FixedUpdate()
    {
        if (ControladorPPAL.v_pausado_b)
            return;

        if (Vector3.Distance(transform.position, Peloton.peloton.transform.position) <= Peloton.peloton.v_distanciaAlPelotonReal_f)
        {
            CambiarEstado(1);
        }

        Movimiento_s.v_objetivo_t = v_objetivo_t;
    }

    // ***********************( Metodos NUESTROS )*********************** //
    private void sinEnemigos()
    {
        if (ObtenerIndice(EstadoActual) == 2)
        {
            CambiarEstado(1);
        }
    }


    // ***********************( ESTADOS DE LA MAQUINA DE ESTADOS )*********************** //
    class EstadoParado : EstadoBase
    {
        private ControladorPorculero _controladorPorculero_s;
        public override void Init<T>(T dependencia)
        {
            _controladorPorculero_s = dependencia as ControladorPorculero;
        }

        public override void Entrar()
        {
            _controladorPorculero_s.v_objetivo_t = null;
            _controladorPorculero_s.Movimiento_s.v_esperando_b = true;
            _controladorPorculero_s.Movimiento_s.v_exodia_b = false;
        }
        public override void Salir()
        {}
    }
    class EstadoPersiguiendo : EstadoBase
    {
        private ControladorPorculero _controladorPorculero_s;
        public override void Init<T>(T dependencia)
        {
            _controladorPorculero_s = dependencia as ControladorPorculero;
        }

        public override void Entrar()
        {
            if (_controladorPorculero_s.Movimiento_s == null)
                return;

            _controladorPorculero_s.Movimiento_s.v_esperando_b = false;
            _controladorPorculero_s.Movimiento_s.v_exodia_b = false;
        }
        public override void Salir()
        { }

        public override void MiUpdate()
        {
            _controladorPorculero_s.v_objetivo_t = Peloton.peloton.transform;
        }
    }
    class EstadoAtacando : EstadoBase
    {
        private ControladorPorculero _controladorPorculero_s;
        public override void Init<T>(T dependencia)
        {
            _controladorPorculero_s = dependencia as ControladorPorculero;
        }

        public override void Entrar()
        {
            if (_controladorPorculero_s.Movimiento_s == null)
                return;

            _controladorPorculero_s.Movimiento_s.v_esperando_b = false;
            _controladorPorculero_s.Movimiento_s.v_exodia_b = true;
        }
        public override void Salir()
        {
            if (_controladorPorculero_s.Movimiento_s == null)
                return;

            _controladorPorculero_s.Movimiento_s.v_esperando_b = false;
            _controladorPorculero_s.Movimiento_s.v_exodia_b = false;
        }

        public override void MiUpdate()
        {
            if (_controladorPorculero_s.v_ataque_s == null)
                return;

            _controladorPorculero_s.v_ataque_s._atacar_b = true;
            Transform _nuevoObjetivo_t = _controladorPorculero_s.v_ataque_s.EnemigoObjetivo_go.transform;
            if (_nuevoObjetivo_t != null)
                _controladorPorculero_s.v_objetivo_t = _nuevoObjetivo_t;
            else
                Debug.LogWarning("--- No hay enemigo objetivo ---");
        }
    }


    // ***********************( Gizmos )*********************** //
    private void OnDrawGizmos()
    {
        if  (v_objetivo_t == null)
            return;

        Gizmos.color = Color.green;
        #if UNITY_EDITOR
                Handles.Label(transform.position + Vector3.up * 0.5f, "PORCULERO : objetivo -> " + v_objetivo_t.name);
        #endif
    }
}

