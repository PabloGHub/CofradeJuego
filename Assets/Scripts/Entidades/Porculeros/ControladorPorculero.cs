using System;
using System.Collections.Generic;
using UnityEngine;

public class ControladorPorculero : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
     [HideInInspector] public Movimiento Movimiento_s;
     private Transform _objetivo_t;

    [Header("*--- Atributos ---*")]
    [SerializeField]
    private LayerMask _mascaraObjetivo;
    public bool EsDistancia;
    public float RangoVisibliidad;

    // Ataque
    private Ataque _ataque_s;

    // --- Maquina de Estados --- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }


    // ***********************( Metodos UNITY )*********************** //
    private void Awake()
    {
        // Movimiento
        Movimiento_s = GetComponent<Movimiento>();
        if (Movimiento_s == null)
        {
            Debug.LogError($"****** Porculero: {gameObject.name} NO tiene componente (Movimiento) ******");
            return;
        }

        // Maquina de Estados
        Inicializar(gameObject);
        estadosPosibles = new List<EstadoBase>
        { 
            CrearEstado<EstadoParado, ControladorPorculero>(this),
            CrearEstado<EstadoPersiguiendo, ControladorPorculero>(this),
            CrearEstado<EstadoAtacando, ControladorPorculero>(this)
        };
        CambiarEstado(0);

        // Ataque
        _ataque_s = GetComponent<Ataque>();
        if (_ataque_s == null)
        {
            Debug.LogError($"****** Porculero: {gameObject.name} NO tiene componente (Ataque) ******");
            return;
        }
    }

    private void FixedUpdate()
    {
        if (ControladorPPAL.v_pausado_b)
            return;

        if (Vector3.Distance(transform.position, Peloton.peloton.transform.position) <= Peloton.peloton.v_distanciaAlPelotonReal_f)
        {
            CambiarEstado(1);
        }

        Movimiento_s.v_objetivo_t = _objetivo_t;
    }

    // ***********************( Metodos NUESTROS )*********************** //


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
            _controladorPorculero_s._objetivo_t = null;
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
            _controladorPorculero_s.Movimiento_s.v_esperando_b = false;
            _controladorPorculero_s.Movimiento_s.v_exodia_b = false;
        }
        public override void Salir()
        { }

        public override void MiUpdate()
        {
            _controladorPorculero_s._objetivo_t = Peloton.peloton.transform;
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
            _controladorPorculero_s.Movimiento_s.v_esperando_b = false;
            _controladorPorculero_s.Movimiento_s.v_exodia_b = true;
        }
        public override void Salir()
        {
            _controladorPorculero_s.Movimiento_s.v_esperando_b = false;
            _controladorPorculero_s.Movimiento_s.v_exodia_b = false;
        }

        public override void MiUpdate()
        {
            Transform _nuevoObjetivo_t = v_controladorNazareno_s.v_ataque_s.EnemigoObjetivo_go.transform;
            if (_nuevoObjetivo_t != null)
                v_controladorNazareno_s.v_objetivo_t = _nuevoObjetivo_t;
            else
                Debug.LogWarning("--- No hay enemigo objetivo ---");
        }
    }
}

