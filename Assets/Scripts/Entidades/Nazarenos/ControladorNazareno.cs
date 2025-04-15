using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControladorNazareno : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
    private float cercaniaAlObjetivo = 2.5f;
    public int v_objetivoIndex_i = 0;
    [HideInInspector] public Movimiento v_movimiento;
    private Transform v_objetivo_Transform;

    // Datos
    public string nombre;
    [HideInInspector] public int id;
    [SerializeField]
    private LayerMask mascaraEnemigo;
    public bool EsDistancia;
    public float RangoVisibliidad;

    // Ataque
    private Vector2 v_45_v2 = new Vector2(0, 45);
    private Vector2 v_pos1_v2;
    private Vector2 v_pos2_v2;
    private Vector2 v_pos3_v2;
    private Vector2 v_pos4_v2;
    private GameObject v_enemigoObjetivo_go;
    private Ataque v_ataque_s;
    private CircleCollider2D v_circle_coll;
    private List<GameObject> v_listaPorculeros;
    private Dictionary<GameObject, Action> v_porculeroDelegados = new Dictionary<GameObject, Action>();

    // --- Maquina de Estados --- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }

    // ***********************( Funciones Unity )*********************** //
    private void Start()
    {
        // Movimiento
        v_movimiento = GetComponent<Movimiento>();
        if (v_movimiento == null)
        {
            Debug.LogError("El Nazareno no tiene un componente Movimiento.");
            return;
        }
        v_objetivo_Transform = Navegacion.nav.trayectoria[v_objetivoIndex_i];
        v_movimiento.v_objetivo_Transform = v_objetivo_Transform;


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

        // Ataque
        RangoVisibliidad += 1;
        v_pos1_v2 = transform.up;
        v_pos2_v2 = transform.right;
        v_pos3_v2 = -transform.up;
        v_pos4_v2 = -transform.right;
        v_ataque_s = GetComponent<Ataque>();
        v_ataque_s.v_capaAtacado_LM = mascaraEnemigo;
        v_circle_coll = gameObject.AddComponent<CircleCollider2D>();
        v_circle_coll.isTrigger = true;
        v_circle_coll.radius = RangoVisibliidad;
        v_listaPorculeros = new List<GameObject>();
        v_porculeroDelegados = new Dictionary<GameObject, Action>();
    }

    private void Update()
    {
        if (ControladorPPAL.v_pausado_b)
            return;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("puntoControl") && collision.gameObject == v_objetivo_Transform.gameObject)
        {
            if (Vector3.Distance(transform.position, v_movimiento.v_objetivo_Transform.position) < cercaniaAlObjetivo)
            {
                actualizarObjetivo();
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
        if (collision.CompareTag("Porculeros"))
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
            v_enemigoObjetivo_go = null;
            return;
        }

        float _distanciaMinima = float.MaxValue;
        foreach (GameObject v_porculero_go in v_listaPorculeros)
        {
            float _distancia = Vector3.Distance(transform.position, v_porculero_go.transform.position);
            if (_distancia < _distanciaMinima)
            {
                _distanciaMinima = _distancia;
                v_enemigoObjetivo_go = v_porculero_go;
            }
        }

        if (v_enemigoObjetivo_go != null)
            CambiarEstado(1);
    }

    private void actualizarObjetivo()
    {
        v_objetivoIndex_i++;

        if (v_objetivoIndex_i >= Navegacion.nav.trayectoria.Length)
            extracion();

        while (true)
        {
            Punto punto = Navegacion.nav.trayectoria[v_objetivoIndex_i].GetComponent<Punto>();

            if (!punto.difurcacion)
            {
                break;
            }
            else if (!punto.v_elegido_b)
            {
                v_objetivoIndex_i++;
            }
            else
            {
                break;
            }
        }

        v_objetivo_Transform = Navegacion.nav.trayectoria[v_objetivoIndex_i];
        v_movimiento.v_objetivo_Transform = v_objetivo_Transform;
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
        {
            
        }
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
            v_controladorNazareno_s.v_movimiento.v_exodia_b = false;
        }
        public override void Salir() { }

        public override void MiUpdate()
        {
            if (v_controladorNazareno_s.v_enemigoObjetivo_go == null)
            {
                Debug.LogWarning("No hay enemigo objetivo.");
                return;
            }

            Transform objetivoTransform = v_controladorNazareno_s.v_enemigoObjetivo_go.transform;
            if (objetivoTransform == null)
            {
                Debug.LogWarning("El Transform del enemigo objetivo es null.");
                return;
            }

            v_controladorNazareno_s.v_objetivo_Transform = objetivoTransform;


            Vector3 direccion = (objetivoTransform.position - v_controladorNazareno_s.transform.position).normalized;

            v_controladorNazareno_s.v_ataque_s.v_direcion_f = direccion.x; 

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