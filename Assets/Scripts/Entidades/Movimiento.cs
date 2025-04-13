using UnityEngine;
using UnityEngine.AI;

public class Movimiento : MonoBehaviour
{
    // TODO: Al pausar se detiene y al despausar aplicar fuerza para compensar.
    // ***********************( Declaraciones )*********************** //
    [Header("*-- Atributos --*")]
    [SerializeField]
    private float aceleracion = 10f;
    private float v_aceleracionExodia_f;
    [SerializeField]
    private float fuerzaRotacion = 5f;

    //HideInInspector]
    public bool v_esperando_b = false;
    public bool v_exodia_b = false;

    [HideInInspector]
    public Transform v_objetivo_Transform;
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
        v_aceleracionExodia_f = aceleracion * 1.5f;

        if (v_agente_NavMeshAgent != null)
        {
            v_agente_NavMeshAgent.updatePosition = false;
            v_agente_NavMeshAgent.updateRotation = false;
            v_agente_NavMeshAgent.updateUpAxis = false;
        }
    }

    private void Update()
    {
        if (ControladorPPAL.v_pausado_b || v_esperando_b)
            return;

        if (v_agente_NavMeshAgent != null)
        {
            v_agente_NavMeshAgent.SetDestination(v_objetivo_Transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (ControladorPPAL.v_pausado_b || v_esperando_b)
            return;

        if (v_agente_NavMeshAgent != null)
        {
            if (!v_agente_NavMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning("El agente está fuera del NavMesh. Redirigiendo...");
                RedirigirHaciaNavMesh();
                return;
            }


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
            RaycastHit2D v_hit = Physics2D.Raycast(transform.position, transform.up, 1.15f);
            if (true)
            {
                if (!v_exodia_b)
                    v_rb_rb2D.AddForce(transform.up * aceleracion * Time.fixedDeltaTime);
                else
                    v_rb_rb2D.AddForce(transform.up * v_aceleracionExodia_f * Time.fixedDeltaTime);
            }

            v_agente_NavMeshAgent.nextPosition = transform.position;
        }
    }

    // ***********************( Funciones Nuestras )*********************** //
    private void RedirigirHaciaNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            // Calcular la dirección hacia el punto más cercano en el NavMesh
            Vector3 direccionHaciaNavMesh = (hit.position - transform.position).normalized;

            // Calcular el ángulo hacia el NavMesh
            float v_anguloActual_f = Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg;
            float v_anguloObjetivo_f = Mathf.Atan2(direccionHaciaNavMesh.y, direccionHaciaNavMesh.x) * Mathf.Rad2Deg;

            float v_diferenciaAngulo_f = Mathf.DeltaAngle(v_anguloActual_f, v_anguloObjetivo_f);

            // Aplicar torque para rotar hacia el NavMesh
            float v_torque_f;
            if (v_diferenciaAngulo_f > 0)
                v_torque_f = fuerzaRotacion * Time.fixedDeltaTime;
            else
                v_torque_f = -fuerzaRotacion * Time.fixedDeltaTime;

            v_rb_rb2D.AddTorque(v_torque_f);

            // Aplicar fuerza para mover hacia el NavMesh
            v_rb_rb2D.AddForce(transform.up * aceleracion * Time.fixedDeltaTime);
        }
    }
}


/* // Movimiento Antiguo
 // TODO: Al pausar se detiene y al despausar aplicar fuerza para compensar.
    // ***********************( Declaraciones )*********************** //
    [Header("*-- Atributos --*")]

    [SerializeField]
    private float aceleracion = 10f;
    [SerializeField]
    private float fuerzaRotacion = 5f;
    [SerializeField]
    private float cercaniaAlObjetivo = 0.1f;


    private Transform v_objetivo_transform;
    private int v_objetivoIndex_i = 0;
    private Rigidbody2D v_rb_rb2D;

    // ***********************( Funciones Unity )*********************** //
    private void Start()
    {
        v_objetivo_transform = Navegacion.nav.trayectoria[v_objetivoIndex_i];
        v_rb_rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (ControladorPPAL.v_pausado_b)
            return;
        
        if (Vector3.Distance(transform.position, v_objetivo_transform.position) < cercaniaAlObjetivo)
        {
            v_objetivoIndex_i++;

            if (v_objetivoIndex_i >= Navegacion.nav.trayectoria.Length)
                v_objetivoIndex_i = 0; // Creara un bucle.

            v_objetivo_transform = Navegacion.nav.trayectoria[v_objetivoIndex_i];
        }

        //transform.position = Vector3.MoveTowards(transform.position, v_objetivo_transform.position, velocidad * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (ControladorPPAL.v_pausado_b)
            return;

        Vector2 v_direccion_v2 = (v_objetivo_transform.position - transform.position).normalized;

        if (!(v_objetivoIndex_i++ >= Navegacion.nav.trayectoria.Length))
        { 
            Vector2 v_direcionPunto_v2 = (Navegacion.nav.trayectoria[v_objetivoIndex_i++].position - v_objetivo_transform.position).normalized;

            if (Vector2.Dot(v_direccion_v2, v_direcionPunto_v2) < 0)
            {
                v_objetivoIndex_i++;
            }
        }

        float v_anguloActual_f = Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg;
        float v_anguloObjetivo_f = Mathf.Atan2(v_direccion_v2.y, v_direccion_v2.x) * Mathf.Rad2Deg;

        float v_diferenciaAngulo_f = Mathf.DeltaAngle(v_anguloActual_f, v_anguloObjetivo_f);

        float v_torque_f;// = v_diferenciaAngulo_f * fuerzaRotacion * Time.fixedDeltaTime;
        if (v_diferenciaAngulo_f > 0)
            v_torque_f = fuerzaRotacion * Time.fixedDeltaTime;
        else
            v_torque_f = -fuerzaRotacion * Time.fixedDeltaTime;

        v_rb_rb2D.AddTorque(v_torque_f);

        // TODO: Que no acelere si tiene una pared delante.
        v_rb_rb2D.AddForce(transform.up * aceleracion * Time.fixedDeltaTime);
    }

    // ***********************( Funciones Nuestras )*********************** //
 */