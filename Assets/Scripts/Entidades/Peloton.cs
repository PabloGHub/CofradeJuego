using System.Collections.Generic;
using UnityEngine;

public class Peloton : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    [HideInInspector] public static Peloton peloton;
    [HideInInspector] public float v_distanciaAlPeloton_f = 0f;
    [HideInInspector] public float v_distanciaAlPelotonReal_f = 0f;

    [SerializeField] private Transform[] integrantes;
    [SerializeField] private float tamannoNazareno = 1.1f;

    // --- Control de puntosControl --- //
    public int v_objetivoIndex_i = 0; // [HideInInspector]
    private float cercaniaAlObjetivo = 2.5f;
    private Transform v_objetivo_Transform;

    // --- Control de distancia --- //
    private List<Transform> v_integranteLejosAtrasado;
    private List<Transform> v_integranteLejosMedio;
    private List<Transform> v_integranteLejosAdelantado;
    private List<Transform> v_integranteCerca;

    // ***********************( Metodos UNITY )*********************** //
    private void Awake()
    {
        if (peloton != this)
        {
            peloton = this;
        }
    }

    private void Start()
    {
        v_objetivo_Transform = Navegacion.nav.trayectoria[v_objetivoIndex_i];

        v_integranteLejosAtrasado = new List<Transform>();
        v_integranteLejosMedio = new List<Transform>();
        v_integranteLejosAdelantado = new List<Transform>();
        v_integranteCerca = new List<Transform>();
    }

    private void Update()
    {
        v_distanciaAlPeloton_f = (integrantes.Length * tamannoNazareno) * 0.5f;
        v_distanciaAlPelotonReal_f = (integrantes.Length * tamannoNazareno);
        transform.position = F_calcularCentro_Vector3(integrantes);

        gestionarIntegrantes();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("puntoControl") && collision.gameObject == v_objetivo_Transform.gameObject)
        {
            if (Vector3.Distance(transform.position, v_objetivo_Transform.position) < cercaniaAlObjetivo)
            {
                actualizarObjetivo();
            }
        }
    }

    // ***********************( Metodos NUESTROS )*********************** //
    void extracion()
    {
        Debug.Log("Llegamos a carrera");
        // TODO: Llegamos a carrera.
    }


    private Vector3 F_calcularCentro_Vector3(Transform[] v_cantidad_t)
    {
        Vector3 v_sumaPosiciones_v3 = Vector3.zero;

        for (int i = 0; i < v_cantidad_t.Length; i++)
        {
            if (v_cantidad_t[i] != null)
                v_sumaPosiciones_v3 += v_cantidad_t[i].position;
        }

        return v_sumaPosiciones_v3 / v_cantidad_t.Length;
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
    }


    private void gestionarIntegrantes()
    {
        bool v_alguienAtrasado_b = false;

        foreach (Transform v_integrante in integrantes)
        {
            // El integrante esta lejos del peloton.
            if (Vector3.Distance(v_integrante.position, transform.position) > v_distanciaAlPeloton_f)
            {
                NazarenoBase v_nazareno = v_integrante.GetComponent<NazarenoBase>();

                if (v_nazareno == null)
                    return;

                // El integrante esta adelante.
                if (v_nazareno.v_objetivoIndex_i > v_objetivoIndex_i)
                {
                    v_integranteLejosAdelantado.Add(v_integrante);
                }

                // El integrante esta atrasado.
                else if (v_nazareno.v_objetivoIndex_i < v_objetivoIndex_i)
                {
                     v_integranteLejosAtrasado.Add(v_integrante);
                }

                // El integrante esta en el medio.
                else
                {
                    v_integranteLejosMedio.Add(v_integrante);
                }

            }
            // El integrante esta cerca del peloton.
            else
            {
                v_integranteCerca.Add(v_integrante);
            }
        }

        if (v_integranteLejosAdelantado.Count < (integrantes.Length * 0.4f))
        {
            // LEJOS ADELANTADOS
            foreach (Transform v_integrante in v_integranteLejosAdelantado)
            {
                NazarenoBase v_nazareno = v_integrante.GetComponent<NazarenoBase>();
                if (v_nazareno == null)
                    return;
                v_nazareno.v_movimiento.v_esperando_b = true;
                v_nazareno.v_movimiento.v_exodia_b = false;
            }
        }
        // Peloton se a saltado un punto de control.
        else // No se como es de probable.
        {
            v_objetivoIndex_i++;

            // LEJOS ADELANTADOS
            foreach (Transform v_integrante in v_integranteLejosAdelantado)
            {
                NazarenoBase v_nazareno = v_integrante.GetComponent<NazarenoBase>();
                if (v_nazareno == null)
                    return;
                v_nazareno.v_movimiento.v_esperando_b = false;
                v_nazareno.v_movimiento.v_exodia_b = false;
            }
        }

        // LEJOS ATRASADOS
        foreach (Transform v_integrante in v_integranteLejosAtrasado)
        {
            NazarenoBase v_nazareno = v_integrante.GetComponent<NazarenoBase>();
            if (v_nazareno == null)
                return;
            v_nazareno.v_movimiento.v_esperando_b = false;
            v_nazareno.v_movimiento.v_exodia_b = true;
            v_alguienAtrasado_b = true;
        }

        if (v_alguienAtrasado_b)
        {
            // CERCA
            foreach (Transform v_integrante in v_integranteCerca)
            {
                NazarenoBase v_nazareno = v_integrante.GetComponent<NazarenoBase>();
                if (v_nazareno == null)
                    return;
                v_nazareno.v_movimiento.v_esperando_b = true;
                v_nazareno.v_movimiento.v_exodia_b = false;
            }
        }
        else
        {
            // CERCA
            foreach (Transform v_integrante in v_integranteCerca)
            {
                NazarenoBase v_nazareno = v_integrante.GetComponent<NazarenoBase>();
                if (v_nazareno == null)
                    return;
                v_nazareno.v_movimiento.v_esperando_b = false;
                v_nazareno.v_movimiento.v_exodia_b = false;
            }
        }

        // LEJOS MEDIO
        foreach (Transform v_integrante in v_integranteLejosMedio)
        {
            NazarenoBase v_nazareno = v_integrante.GetComponent<NazarenoBase>();
            if (v_nazareno == null)
                return;
            v_nazareno.v_movimiento.v_esperando_b = false;
            v_nazareno.v_movimiento.v_exodia_b = false;
        }

        // Limpiar listas
        v_integranteLejosAtrasado.Clear();
        v_integranteLejosMedio.Clear();
        v_integranteLejosAdelantado.Clear();
        v_integranteCerca.Clear();
    }
}
