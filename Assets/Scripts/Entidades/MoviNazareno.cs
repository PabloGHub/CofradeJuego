using UnityEngine;
using UnityEngine.AI;

public class MoviNazareno : MonoBehaviour
{
    // TODO: Al pausar se detiene y al despausar aplicar fuerza para compensar.
    // ***********************( Declaraciones )*********************** //
    [Header("*-- Atributos --*")]
    public Transform objetivo;
    [SerializeField]
    private float aceleracion = 10f;
    [SerializeField]
    private float fuerzaRotacion = 5f;


    private NavMeshAgent v_agente_NavMeshAgent;
    private Rigidbody2D v_rb_rb2D;

    // ***********************( Funciones Unity )*********************** //
    private void Awake()
    {
        if (v_agente_NavMeshAgent == null)
        {
            v_agente_NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        if (v_rb_rb2D == null)
        {
            v_rb_rb2D = GetComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        v_agente_NavMeshAgent.updatePosition = false;
        v_agente_NavMeshAgent.updateRotation = false;
        v_agente_NavMeshAgent.updateUpAxis = false;
    }

    private void Update()
    {
        if (ControladorPPAL.v_pausado_b)
            return;

        if (v_agente_NavMeshAgent != null)
        {
            // Actualizar el destino del agente
            v_agente_NavMeshAgent.SetDestination(objetivo.position);
        }
    }

    private void FixedUpdate()
    {
        if (ControladorPPAL.v_pausado_b)
            return;

        if (v_agente_NavMeshAgent != null)
        {
            Vector3 v_direccion_v3 = v_agente_NavMeshAgent.desiredVelocity.normalized;

            float v_anguloActual_f = Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg;
            float v_anguloObjetivo_f = Mathf.Atan2(v_direccion_v3.y, v_direccion_v3.x) * Mathf.Rad2Deg;

            float v_diferenciaAngulo_f = Mathf.DeltaAngle(v_anguloActual_f, v_anguloObjetivo_f);

            float v_torque_f;// = v_diferenciaAngulo_f * fuerzaRotacion * Time.fixedDeltaTime;
            if (v_diferenciaAngulo_f > 0)
                v_torque_f = fuerzaRotacion * Time.fixedDeltaTime;
            else
                v_torque_f = -fuerzaRotacion * Time.fixedDeltaTime;

            v_rb_rb2D.AddTorque(v_torque_f);
            // TODO: Que no acelere si tiene una pared delante.
            v_rb_rb2D.AddForce(transform.up * aceleracion * Time.fixedDeltaTime);

            v_agente_NavMeshAgent.nextPosition = transform.position;
        }
    }

    // ***********************( Funciones Nuestras )*********************** //
}