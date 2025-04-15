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
        //AreaDespliegue.gameObject.SetActive(false); //Me esta salntado un error
    }

    private void Update()
    {
        v_distanciaAlPeloton_f = (integrantes.Count * tamannoNazareno) * 0.5f;
        v_distanciaAlPelotonReal_f = (integrantes.Count * tamannoNazareno);
        transform.position = F_calcularCentro_Vector3(integrantes.ToArray());

        gestionarIntegrantes();
    }

    // ***********************( Metodos NUESTROS )*********************** //
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


    private void gestionarIntegrantes()
    {
        //List<Transform> v_integranteLejosAtrasado = new List<Transform>();
        //List<Transform> v_integranteLejosMedio = new List<Transform>();
        //List<Transform> v_integranteLejosAdelantado = new List<Transform>();
        //List<Transform> v_integranteCerca = new List<Transform>();

        //bool v_alguienAtrasado_b = false;
        //int? v_max_i = null;
        //int? v_min_i = null;


        //foreach (Transform v_integrante in integrantes)
        //{
        //    ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
        //    if (v_nazareno == null)
        //        return;

        //    v_max_i = (v_nazareno.v_objetivoIndex_i > v_max_i || v_max_i == null) ? v_nazareno.v_objetivoIndex_i : v_max_i;
        //    v_min_i = (v_nazareno.v_objetivoIndex_i < v_min_i || v_min_i == null) ? v_nazareno.v_objetivoIndex_i : v_min_i;
        //}

        //float v_limiteAdelantado_f = v_max_i.HasValue ? v_max_i.Value * 0.7f : 0;
        //float v_limiteAtrasado_f = v_min_i.HasValue && v_max_i.HasValue ? v_min_i.Value + (v_max_i.Value - v_min_i.Value) * 0.3f : 0;

        int _suma_i = 0;
        int _conteo_i = 0;

        foreach (Transform v_integrante in integrantes)
        {
            ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
            if (v_nazareno == null) continue;

            _suma_i += v_nazareno.v_objetivoIndex_i;
            _conteo_i++;
        }
        float _promedio_i = _conteo_i > 0 ? (float)_suma_i / _conteo_i : 0f;

        float _margen_f = 0.3f;
        float v_limiteAdelantado_f = _promedio_i + _margen_f;
        float v_limiteAtrasado_f = _promedio_i - _margen_f;

        foreach (Transform v_integrante in integrantes)
        {
            ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();

            if (v_nazareno == null) continue;
            if (v_nazareno.EstadoActual == null) continue;
            if (v_nazareno.ObtenerIndice(v_nazareno.EstadoActual) > 0) continue;


            // El integrante esta lejos del peloton.
            if (Vector3.Distance(v_integrante.position, transform.position) > v_distanciaAlPeloton_f)
            {
                if (v_nazareno.v_objetivoIndex_i > v_limiteAdelantado_f)
                    v_nazareno.CambiarSubEstado(0); // Adelantado

                else if (v_nazareno.v_objetivoIndex_i < v_limiteAtrasado_f)
                    v_nazareno.CambiarSubEstado(2); // Atrasado

                else
                    v_nazareno.CambiarSubEstado(1); // Medio
            }
            // El integrante esta cerca del peloton.
            else
                v_nazareno.CambiarSubEstado(3);
        }


        //// LEJOS ADELANTADOS
        //foreach (Transform v_integrante in v_integranteLejosAdelantado)
        //{
        //    ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
        //    if (v_nazareno == null)
        //        return;
        //    v_nazareno.v_movimiento.v_esperando_b = true;
        //    v_nazareno.v_movimiento.v_exodia_b = false;
        //}


        //// LEJOS ATRASADOS
        //foreach (Transform v_integrante in v_integranteLejosAtrasado)
        //{
        //    ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
        //    if (v_nazareno == null)
        //        return;
        //    v_nazareno.v_movimiento.v_esperando_b = false;
        //    v_nazareno.v_movimiento.v_exodia_b = true;
        //    v_alguienAtrasado_b = true;
        //}


        //if (v_alguienAtrasado_b)
        //{
        //    // CERCA
        //    foreach (Transform v_integrante in v_integranteCerca)
        //    {
        //        ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
        //        if (v_nazareno == null)
        //            return;
        //        v_nazareno.v_movimiento.v_esperando_b = true;
        //        v_nazareno.v_movimiento.v_exodia_b = false;
        //    }
        //}
        //else
        //{
        //    // CERCA
        //    foreach (Transform v_integrante in v_integranteCerca)
        //    {
        //        ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
        //        if (v_nazareno == null)
        //            return;
        //        v_nazareno.v_movimiento.v_esperando_b = false;
        //        v_nazareno.v_movimiento.v_exodia_b = false;
        //    }
        //}


        //// LEJOS MEDIO
        //foreach (Transform v_integrante in v_integranteLejosMedio)
        //{
        //    ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
        //    if (v_nazareno == null)
        //        return;
        //    v_nazareno.v_movimiento.v_esperando_b = false;
        //    v_nazareno.v_movimiento.v_exodia_b = false;
        //}
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
            Terminal.Log(v_integrante.name + " - " + v_nazareno.v_movimiento.Estado.ToSafeString());
            Debug.Log($"Estado: {v_nazareno.EstadoActual.GetType().Name}, Index: {v_nazareno.ObtenerIndice(v_nazareno.EstadoActual)}");
        }
    }
}
