using CommandTerminal;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Peloton : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    [HideInInspector] public static Peloton peloton;
    [HideInInspector] public float v_distanciaAlPeloton_f = 0f;
    [HideInInspector] public float v_distanciaAlPelotonReal_f = 0f;

    [SerializeField] private List<Transform> integrantes;
    [SerializeField] private float tamannoNazareno = 1.1f;

    [SerializeField] private Transform AreaDespliegue;

    // --- Control de puntosControl --- //
    public int v_objetivoIndex_i = 0; // [HideInInspector]
    private float cercaniaAlObjetivo = 2.5f;
    private Transform v_objetivo_Transform;


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
        AreaDespliegue.gameObject.SetActive(false);
    }

    private void Update()
    {
        v_distanciaAlPeloton_f = (integrantes.Count * tamannoNazareno) * 0.5f;
        v_distanciaAlPelotonReal_f = (integrantes.Count * tamannoNazareno);
        transform.position = F_calcularCentro_Vector3(integrantes.ToArray());

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
        List<Transform> v_integranteLejosAtrasado = new List<Transform>();
        List<Transform> v_integranteLejosMedio = new List<Transform>();
        List<Transform> v_integranteLejosAdelantado = new List<Transform>();
        List<Transform> v_integranteCerca = new List<Transform>();

        bool v_alguienAtrasado_b = false;
        int? v_max_i = null;
        int? v_min_i = null;


        foreach (Transform v_integrante in integrantes)
        {
            ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
            if (v_nazareno == null)
                return;

            v_max_i = (v_nazareno.v_objetivoIndex_i > v_max_i || v_max_i == null) ? v_nazareno.v_objetivoIndex_i : v_max_i;
            v_min_i = (v_nazareno.v_objetivoIndex_i < v_min_i || v_min_i == null) ? v_nazareno.v_objetivoIndex_i : v_min_i;
        }

        foreach (Transform v_integrante in integrantes)
        {
            // El integrante esta lejos del peloton.
            if (Vector3.Distance(v_integrante.position, transform.position) > v_distanciaAlPeloton_f)
            {
                ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();

                if (v_nazareno == null)
                    return;

                // El integrante esta delante.
                if (v_nazareno.v_objetivoIndex_i > (v_max_i * 0.66f))
                {
                    v_integranteLejosAdelantado.Add(v_integrante);
                }

                // El integrante esta atrasado.
                else if (v_nazareno.v_objetivoIndex_i < (v_min_i * 0.66f))
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


        // LEJOS ADELANTADOS
        foreach (Transform v_integrante in v_integranteLejosAdelantado)
        {
            ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
            if (v_nazareno == null)
                return;
            v_nazareno.v_movimiento.v_esperando_b = true;
            v_nazareno.v_movimiento.v_exodia_b = false;
        }


        // LEJOS ATRASADOS
        foreach (Transform v_integrante in v_integranteLejosAtrasado)
        {
            ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
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
                ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
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
                ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
                if (v_nazareno == null)
                    return;
                v_nazareno.v_movimiento.v_esperando_b = false;
                v_nazareno.v_movimiento.v_exodia_b = false;
            }
        }


        // LEJOS MEDIO
        foreach (Transform v_integrante in v_integranteLejosMedio)
        {
            ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
            if (v_nazareno == null)
                return;
            v_nazareno.v_movimiento.v_esperando_b = false;
            v_nazareno.v_movimiento.v_exodia_b = false;
        }
    }


    // Solo instanciar Miembro si no está colisionando con ningún otro miembro del Peloton (opcional, desactivado)
    // Solo instanciar Miembro si está en la Navmesh
    // Solo instanciar Miembro si está en Area de Despliegue
    public bool TryToDropMember(ItemInfo memberInfo, Vector3 position)
    {
        GameObject member = memberInfo.dropObject;
        float memberRadius = member.transform.GetComponent<CircleCollider2D>().radius;
        Transform memberTransform = member.transform;

        NavMeshHit hitNav;
        float maxDistance = 1.0f;
        if (!NavMesh.SamplePosition(position, out hitNav, maxDistance, NavMesh.AllAreas))
        {
            return false;
        }

        AreaDespliegue.gameObject.SetActive(true);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hitRay = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hitRay.collider != null)
        {
            if (!hitRay.collider.CompareTag("Despliegue"))
            {
                AreaDespliegue.gameObject.SetActive(false);
                return false;
            }
        }
        else
        {
            AreaDespliegue.gameObject.SetActive(false);
            return false;
        }
        AreaDespliegue.gameObject.SetActive(false);

        //foreach (Transform transform in integrantes)
        //{
        //    float radius = transform.GetComponent<CircleCollider2D>().radius;
        //    float distanceSQ = (transform.position - position).sqrMagnitude;
        //    if (distanceSQ < (radius + memberRadius) * (radius + memberRadius))
        //    {
        //        return false;
        //    }
        //}
        GameObject droppedMember = Instantiate(member, position, Quaternion.identity);
        droppedMember.GetComponent<ControladorNazareno>().nombre = memberInfo.Name;
        integrantes.Add(droppedMember.transform);
        return true;
    }

    public void EliminarIntegrante(GameObject integrante)
    {
        integrantes.Remove(integrante.transform);
        Destroy(integrante);
    }



    // ***********************( Metodos Comandos )*********************** //
    [RegisterCommand(Help = "Muestra el estado de los integrantes")]
    static void CommandEstados(CommandArg[] args)
    {
        foreach (Transform v_integrante in Peloton.peloton.integrantes)
        {
            ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
            if (v_nazareno == null)
                return;
            Terminal.Log(v_integrante.name + " - " + v_nazareno.v_movimiento.v_estado.ToSafeString());
        }
    }
}
