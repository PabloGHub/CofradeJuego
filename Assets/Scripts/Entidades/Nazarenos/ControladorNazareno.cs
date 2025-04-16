using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControladorNazareno : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
    private float cercaniaAlObjetivo = 3f;
    public int v_objetivoIndex_i = 0;
    [HideInInspector] public Movimiento v_movimiento;
    [HideInInspector] public Transform v_objetivo_t;
    [HideInInspector] public Transform v_puntoObjetivo_t;

    // Datos
    public string nombre;
    [HideInInspector] public int id;
    [SerializeField]
    private LayerMask mascaraEnemigo;
    public bool EsDistancia;
    public float RangoVisibliidad;

    // Ataque
    private GameObject _enemigoObjetivo_go;
    private Ataque v_ataque_s;
    private CircleCollider2D v_circle_coll;
    private List<GameObject> v_listaPorculeros;
    private Dictionary<GameObject, Action> v_porculeroDelegados = new Dictionary<GameObject, Action>();

    // Animaciones
    private Animaciones _animaciones_s;

    // --- Maquina de Estados --- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }

    // ***********************( Funciones Unity )*********************** //
    private void Awake()
    {
        // Ataque
        RangoVisibliidad += 1;
        v_ataque_s = GetComponent<Ataque>();
        v_ataque_s.v_capaAtacado_LM = mascaraEnemigo;
        v_circle_coll = gameObject.AddComponent<CircleCollider2D>();
        v_circle_coll.isTrigger = true;
        v_circle_coll.radius = RangoVisibliidad;
        v_listaPorculeros = new List<GameObject>();
        v_porculeroDelegados = new Dictionary<GameObject, Action>();
    }

    private void Start()
    {
        // Movimiento
        v_movimiento = GetComponent<Movimiento>();
        if (v_movimiento == null)
        {
            Debug.LogError("El Nazareno no tiene un componente Movimiento.");
            return;
        }
        v_puntoObjetivo_t = Navegacion.nav.trayectoria[v_objetivoIndex_i];
        v_objetivo_t = v_puntoObjetivo_t;
        v_movimiento.v_objetivo_t = v_objetivo_t; // Puede que si esta atacando, gire raro.

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
            Debug.Log("i: " + i);
            Animaciones _anim = transform.GetChild(i).GetComponent<Animaciones>();
            if (_anim != null)
                _animaciones_s = _anim;
        }
        if (_animaciones_s == null)
            Debug.LogError("El Nazareno no tiene un componente Animaciones.");
        
    }

    private void FixedUpdate()
    {
        switch (v_movimiento.Direcion)
        {
            case Movimiento.Direcion_e.ARRIBA:
                _animaciones_s.Sprite.flipX = false;
                _animaciones_s.CambiarEstado(0);
            break;


            case Movimiento.Direcion_e.DERECHA:
                _animaciones_s.Sprite.flipX = false;
                _animaciones_s.CambiarEstado(3);
            break;


            case Movimiento.Direcion_e.IZQUIERDA:
                _animaciones_s.CambiarEstado(3);
                _animaciones_s.Sprite.flipX = true;
            break;


            case Movimiento.Direcion_e.ABAJO:
                _animaciones_s.Sprite.flipX = false;
                _animaciones_s.CambiarEstado(1);
            break;

            case Movimiento.Direcion_e.NULO:
                _animaciones_s.Sprite.flipX = false;
                _animaciones_s.CambiarEstado(1);
            break;
        }

        if (ControladorPPAL.v_pausado_b)
            return;

        v_movimiento.v_objetivo_t = v_objetivo_t;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("puntoControl") && collision.gameObject == v_puntoObjetivo_t.gameObject)
        {
            if (v_movimiento.v_objetivo_t == null)
                return;

            if (Vector3.Distance(transform.position, v_movimiento.v_objetivo_t.position) < cercaniaAlObjetivo)
            {
                //Debug.Log("Llegamos al punto de control: " + v_puntoObjetivo_t.name);
                actualizarPunto();
            }
        }
        else if (collision.CompareTag("Porculeros"))
        {
            if (!v_listaPorculeros.Contains(collision.gameObject))
            {
                v_listaPorculeros.Add(collision.gameObject);
                Salud v_salud_s = collision.gameObject.GetComponent<Salud>();
                if (v_salud_s != null)
                {
                    Action v_delegado = () => eliminarPorculero(collision.gameObject);
                    v_porculeroDelegados[collision.gameObject] = v_delegado;
                    v_salud_s.OnMuerto += v_delegado;
                }
                    
            }
        }

        comprobarPorculeros();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("puntoControl") && collision.gameObject == v_puntoObjetivo_t.gameObject)
        {
            if (v_movimiento.v_objetivo_t == null)
                return;

            if (Vector3.Distance(transform.position, v_movimiento.v_objetivo_t.position) < cercaniaAlObjetivo)
            {
                //Debug.Log("Llegamos al punto de control: " + v_puntoObjetivo_t.name);
                actualizarPunto();
            }
        }
        else if (collision.CompareTag("Porculeros"))
        {
            if (v_listaPorculeros.Contains(collision.gameObject))
            {
                v_listaPorculeros.Remove(collision.gameObject);
                Salud v_salud_s = collision.gameObject.GetComponent<Salud>();
                if (v_salud_s != null && v_porculeroDelegados.ContainsKey(collision.gameObject))
                {
                    v_salud_s.OnMuerto -= v_porculeroDelegados[collision.gameObject];
                    v_porculeroDelegados.Remove(collision.gameObject);
                }
            }
        }

        comprobarPorculeros();
    }



    // ***********************( Funciones Nuestras )*********************** //
    void extracion()
    {
        // TODO: cuando llega a carrera destuir o ocular al nazareno.
    }

    private void eliminarPorculero(GameObject v_elPorculero_go)
    {
        if (v_listaPorculeros.Contains(v_elPorculero_go))
        {
            v_listaPorculeros.Remove(v_elPorculero_go);
        }
    }

    void comprobarPorculeros()
    {
        if (v_listaPorculeros.Count <= 0)
        {
            CambiarEstado(0);
            _enemigoObjetivo_go = null;
            v_objetivo_t = v_puntoObjetivo_t;
            return;
        }

        float _distanciaMinima = float.MaxValue;
        foreach (GameObject v_porculero_go in v_listaPorculeros)
        {
            float _distancia = Vector3.Distance(transform.position, v_porculero_go.transform.position);
            if (_distancia < _distanciaMinima)
            {
                _distanciaMinima = _distancia;
                _enemigoObjetivo_go = v_porculero_go;
            }
        }

        if (_enemigoObjetivo_go != null)
            CambiarEstado(1);
    }

    private void actualizarPunto()
    {
        v_objetivoIndex_i++;

        if (v_objetivoIndex_i >= Navegacion.nav.trayectoria.Length)
            extracion();

        while (true)
        {
            Punto punto = Navegacion.nav.trayectoria[v_objetivoIndex_i].GetComponent<Punto>();

            if (!punto.Difurcacion)
            {
                break;
            }
            else if (!punto.Elegido_b)
            {
                v_objetivoIndex_i++;
            }
            else
            {
                break;
            }
        }

        v_puntoObjetivo_t = Navegacion.nav.trayectoria[v_objetivoIndex_i];
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

        public override void Entrar() { }
        public override void Salir() { }

        public override void MiUpdate()
        {}
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
            v_controladorNazareno_s.v_movimiento.v_esperando_b = false;
            v_controladorNazareno_s.v_movimiento.v_exodia_b = true;
        }
        public override void Salir()
        {
            v_controladorNazareno_s.v_movimiento.v_esperando_b = false;
            v_controladorNazareno_s.v_movimiento.v_exodia_b = false;
        }

        public override void MiUpdate()
        {
            if (v_controladorNazareno_s._enemigoObjetivo_go == null)
            {
                Debug.LogWarning("No hay enemigo objetivo.");
                return;
            }

            Transform _objetivoTransform_t = v_controladorNazareno_s._enemigoObjetivo_go.transform;
            if (_objetivoTransform_t == null)
            {
                Debug.LogWarning("El Transform del enemigo objetivo es null.");
                return;
            }

            v_controladorNazareno_s.v_objetivo_t = _objetivoTransform_t;


            Vector3 _direccion_v3 = (_objetivoTransform_t.position - v_controladorNazareno_s.transform.position).normalized;
            v_controladorNazareno_s.v_ataque_s._direcion_v2 = _direccion_v3; 
            v_controladorNazareno_s.v_ataque_s.Atacar();
        }
    }

}

/*
    bool v_lejosPeloton_b = Vector3.Distance(transform.position, Peloton.peloton.transform.position) > Peloton.peloton.v_distanciaAlPeloton_f;
    if (v_lejosPeloton_b && v_objetivoIndex_i < Peloton.peloton.v_objetivoIndex_i)
    {

    }
    else if (v_lejosPeloton_b)
    {
        v_movimiento.v_esperando_b = true;
    }
    else
    {
        v_movimiento.v_esperando_b = false;
    }
*/