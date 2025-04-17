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
        transform.position = f_calcularCentro_Vector3(integrantes.ToArray());

        gestionarIntegrantes();
    }

    // ***********************( Metodos NUESTROS )*********************** //
    /// <summary>
    /// Calcula el centro de un array de transform.
    /// </summary>
    /// <param name="cantidad_t">Array de transorm</param>
    /// <returns>Vector3 de la posicion entre todos los transform</returns>
    private Vector3 f_calcularCentro_Vector3(Transform[] cantidad_t)
    {
        Vector3 v_sumaPosiciones_v3 = Vector3.zero;
        int v_contador_i = 0;

        for (int i = 0; i < cantidad_t.Length; i++)
        {
            if (cantidad_t[i] != null)
            {
                v_sumaPosiciones_v3 += cantidad_t[i].position;
                v_contador_i++;
            }
        }

        return v_contador_i > 0 ? v_sumaPosiciones_v3 / v_contador_i : transform.position;
    }

    /// <summary>
    /// Gestiona el estado de los integrantes del peloton.
    /// Mucho sufrimiento y dolor para tan pocas palabras.
    /// </summary>
    private void gestionarIntegrantes()
    {
        int _suma_i = 0;
        int _conteo_i = 0;

        foreach (Transform v_integrante in integrantes)
        {
            // TODO: Comprobar si no esta asignado.
            ControladorNazareno v_nazareno = v_integrante.GetComponent<ControladorNazareno>();
            if (v_nazareno == null) continue;

            _suma_i += v_nazareno.v_objetivoIndex_i;
            _conteo_i++;
        }
        float _promedio_i = _conteo_i > 0 ? (float)_suma_i / _conteo_i : 0f;

        float _margen_f = 0.3f;
        float _limiteAdelantado_f = _promedio_i + _margen_f;
        float _limiteAtrasado_f = _promedio_i - _margen_f;

        foreach (Transform v_integrante in integrantes)
        {
            ControladorNazareno _nazareno = v_integrante.GetComponent<ControladorNazareno>();

            if (_nazareno == null) continue;
            if (_nazareno.EstadoActual == null) continue;
            if (_nazareno.ObtenerIndice(_nazareno.EstadoActual) > 0) continue;


            // El integrante esta lejos del peloton.
            if (Vector3.Distance(v_integrante.position, transform.position) > v_distanciaAlPeloton_f)
            {
                float _avance_f = Vector3.Distance(_nazareno.v_objetivo_t.position, v_integrante.position);
                float _distanciaAlsiguiente_f = Navegacion.nav.trayectoria[_nazareno.v_objetivoIndex_i].gameObject.GetComponent<Punto>().DistanciaAlSiguiente_f;
                float _progresoPorcentual_f = _distanciaAlsiguiente_f > 0 ? _avance_f / _distanciaAlsiguiente_f : 0f;

                if (_nazareno.v_objetivoIndex_i < _limiteAtrasado_f && _progresoPorcentual_f > 0.3f)
                    _nazareno.CambiarSubEstado(2); // Atrasado

                else if(_nazareno.v_objetivoIndex_i > _limiteAdelantado_f && _progresoPorcentual_f < 0.7f)
                    _nazareno.CambiarSubEstado(0); // Adelantado

                else
                    _nazareno.CambiarSubEstado(1); // Medio
            }
            // El integrante esta cerca del peloton.
            else
                _nazareno.CambiarSubEstado(3);
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

    public float DevolverIntegrantesTotal()
    {
        float amount = 0;
        var integrantesCopy = new List<Transform>(integrantes);
        foreach (var integrante in integrantesCopy)
        {
            amount += ShopManager.instance.Data.Items.ContainsKey(integrante.name) ? ShopManager.instance.Data.Items[integrante.name].Price : 0;
            EliminarIntegrante(integrante.gameObject);
        }
        return amount;
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



    // ***********************( Debug/Gizmos )*********************** //
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, v_distanciaAlPeloton_f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, v_distanciaAlPelotonReal_f);
    }
}
