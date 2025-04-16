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
    private GameObject _enemigoObjetivo_go;
    private Ataque _ataque_s;
    private CircleCollider2D _circle_coll;
    private List<GameObject> _listaNazarenos;
    private Dictionary<GameObject, Action> _nazarenosDelegados = new Dictionary<GameObject, Action>();

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
            Debug.LogError("El Porculero no tiene un componente Movimiento.");
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
        RangoVisibliidad += 1;
        _ataque_s = GetComponent<Ataque>();
        if (_ataque_s == null)
        {
            Debug.LogError("El Porculero no tiene un componente Ataque.");
            return;
        }
        _ataque_s.v_capaAtacado_LM = _mascaraObjetivo;
        _circle_coll = gameObject.AddComponent<CircleCollider2D>();
        _circle_coll.isTrigger = true;
        _circle_coll.radius = RangoVisibliidad;
        _listaNazarenos = new List<GameObject>();
        _nazarenosDelegados = new Dictionary<GameObject, Action>();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Nazarenos"))
        {
            if (!_listaNazarenos.Contains(collision.gameObject))
            {
                _listaNazarenos.Add(collision.gameObject);
                Salud v_salud_s = collision.gameObject.GetComponent<Salud>();
                if (v_salud_s != null)
                {
                    Action v_delegado = () => eliminarNazareno(collision.gameObject);
                    _nazarenosDelegados[collision.gameObject] = v_delegado;
                    v_salud_s.OnMuerto += v_delegado;
                }

            }

            comprobarListaObjetivos();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Nazarenos"))
        {
            if (_listaNazarenos.Contains(collision.gameObject))
            {
                _listaNazarenos.Remove(collision.gameObject);
                Salud v_salud_s = collision.gameObject.GetComponent<Salud>();
                if (v_salud_s != null && _nazarenosDelegados.ContainsKey(collision.gameObject))
                {
                    v_salud_s.OnMuerto -= _nazarenosDelegados[collision.gameObject];
                    _nazarenosDelegados.Remove(collision.gameObject);
                }
            }

            comprobarListaObjetivos();
        }
    }

    // ***********************( Metodos NUESTROS )*********************** //
    void comprobarListaObjetivos()
    {
        if (_listaNazarenos.Count <= 0)
        {
            CambiarEstado(1);
            _enemigoObjetivo_go = null;
            return;
        }

        float _distanciaMinima = float.MaxValue;
        foreach (GameObject v_porculero_go in _listaNazarenos)
        {
            float _distancia = Vector3.Distance(transform.position, v_porculero_go.transform.position);
            if (_distancia < _distanciaMinima)
            {
                _distanciaMinima = _distancia;
                _enemigoObjetivo_go = v_porculero_go;
            }
        }

        if (_enemigoObjetivo_go != null)
            CambiarEstado(2);
    }
    private void eliminarNazareno(GameObject _objetivo_go)
    {
        if (_listaNazarenos.Contains(_objetivo_go))
        {
            _listaNazarenos.Remove(_objetivo_go);
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
            if (_controladorPorculero_s._enemigoObjetivo_go == null)
            {
                Debug.LogWarning("No hay enemigo objetivo.");
                return;
            }

            Transform _objetivoTransform_t = _controladorPorculero_s._enemigoObjetivo_go.transform;
            if (_objetivoTransform_t == null)
            {
                Debug.LogWarning("El Transform del enemigo objetivo es null.");
                return;
            }

            _controladorPorculero_s._objetivo_t = _objetivoTransform_t;

            Vector3 _direccion_v3 = (_objetivoTransform_t.position - _controladorPorculero_s.transform.position).normalized;
            _controladorPorculero_s._ataque_s._direcion_v2 = _direccion_v3;
            _controladorPorculero_s._ataque_s.Atacar();
        }
    }
}

