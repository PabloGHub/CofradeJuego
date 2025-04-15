using NUnit.Framework;
using System.Collections.Generic;
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
    }

    private void Update()
    {
        if (ControladorPPAL.v_pausado_b)
            return;
        if (!EsDistancia)
        {
            RaycastHit2D[] hits = new RaycastHit2D[4];
            hits[0] = Physics2D.Raycast
            (
                transform.position,
                v_pos1_v2 += v_45_v2,
                RangoVisibliidad,
                mascaraEnemigo
            );
            hits[1] = Physics2D.Raycast
            (
                transform.position,
                v_pos2_v2 += v_45_v2,
                RangoVisibliidad,
                mascaraEnemigo
            );
            hits[2] = Physics2D.Raycast
            (
                transform.position,
                v_pos3_v2 += v_45_v2,
                RangoVisibliidad,
                mascaraEnemigo
            );
            hits[3] = Physics2D.Raycast
            (
                transform.position,
                v_pos4_v2 += v_45_v2,
                RangoVisibliidad,
                mascaraEnemigo
            );

            RaycastHit2D hitMasCercano = default;
            float distanciaMinima = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    float distancia = Vector2.Distance(transform.position, hit.point);
                    if (distancia < distanciaMinima)
                    {
                        distanciaMinima = distancia;
                        hitMasCercano = hit;
                    }
                }
            }

            if (hitMasCercano.collider != null)
            {
                v_enemigoObjetivo_go = hitMasCercano.collider.gameObject;
                CambiarEstado(1);
            }
            else
            {
                v_enemigoObjetivo_go = null;
                CambiarEstado(0);
            }
        }

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
    }

    // ***********************( Funciones Nuestras )*********************** //
    void extracion()
    {
        // TODO: cuando llega a carrera destuir o ocular al nazareno.
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
            v_controladorNazareno_s.v_objetivo_Transform = v_controladorNazareno_s.v_enemigoObjetivo_go.transform;

            v_controladorNazareno_s.v_ataque_s.v_direcion_f = v_controladorNazareno_s.v_objetivo_Transform.position.x;
            v_controladorNazareno_s.v_ataque_s.Atacar();
        }
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        // Configurar el color de los Gizmos
        Gizmos.color = Color.red;

        // Dibujar los raycasts
        Vector3 origen = transform.position;

        // Raycast 1 (hacia arriba)
        Vector3 direccion1 = v_pos1_v2.normalized * RangoVisibliidad;
        Gizmos.DrawLine(origen, origen + direccion1);

        // Raycast 2 (hacia la derecha)
        Vector3 direccion2 = v_pos2_v2.normalized * RangoVisibliidad;
        Gizmos.DrawLine(origen, origen + direccion2);

        // Raycast 3 (hacia abajo)
        Vector3 direccion3 = v_pos3_v2.normalized * RangoVisibliidad;
        Gizmos.DrawLine(origen, origen + direccion3);

        // Raycast 4 (hacia la izquierda)
        Vector3 direccion4 = v_pos4_v2.normalized * RangoVisibliidad;
        Gizmos.DrawLine(origen, origen + direccion4);
    }

    // ************ Estado de Movimiento ************ //
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