using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class MovimientoCopia : MonoBehaviour
{
    // TODO: Al pausar se detiene y al despausar aplicar fuerza para compensar.
    // ***********************( Declaraciones )*********************** //
    [Header("*-- Atributos --*")]
    [SerializeField] private float aceleracion = 10f;
    [SerializeField] private float fuerzaRotacion = 5f;

    [HideInInspector] private float v_aceleracionExodia_f;

    // --- Estados --- //
    //HideInInspector]
    public bool v_esperando_b = false;
    public bool v_exodia_b = false;
    public bool QuedarteQuieto = false;

    // --- Componentes --- //
    [HideInInspector] 
    public Transform v_objetivo_t = null;
    private NavMeshAgent v_agente_NavMeshAgent;
    private Rigidbody2D v_rb_rb2D;

    // --- Direcion --- //
    [HideInInspector]
    public float Angulo_f = 0f;
    public enum Direcion_e
    {
        ARRIBA,
        DERECHA,
        ABAJO,
        IZQUIERDA,
        NULO
    }
    public Direcion_e Direcion = Direcion_e.NULO;

    // --- Maquina de Estados --- //

    // ***********************( Funciones Unity )*********************** //
    private void Awake()
    {
        Direcion = f_obtenerDirecion_e(transform);

        v_aceleracionExodia_f = aceleracion * 1.5f;


        v_agente_NavMeshAgent = GetComponent<NavMeshAgent>();
        if (v_agente_NavMeshAgent == null)
            Debug.LogError($"****** Entidad: {gameObject.name} NO tiene componente (NavMeshAgent) ******");

        if (v_agente_NavMeshAgent != null)
        {
            v_agente_NavMeshAgent.updatePosition = false;
            v_agente_NavMeshAgent.updateRotation = false;
            v_agente_NavMeshAgent.updateUpAxis = false;
        }

        v_rb_rb2D = GetComponent<Rigidbody2D>();
        if (v_rb_rb2D == null)
            Debug.LogError($"****** Entidad: {gameObject.name} NO tiene componente (Rigidbody2D) ******");


        Inicializar(gameObject);
        estadosPosibles = new List<EstadoBase>
        {
            CrearEstado<EstadoMoviendose, MovimientoCopia>(this),
            CrearEstado<EstadoQuieto, MovimientoCopia>(this)
        };
        AgregarTransicion(() => v_esperando_b == true, 1);
        AgregarTransicion(() => v_esperando_b == false, 0);
    }


    private void FixedUpdate()
    {
        if (v_agente_NavMeshAgent != null)
        {
            v_agente_NavMeshAgent.updatePosition = false;
            v_agente_NavMeshAgent.updateRotation = false;
            v_agente_NavMeshAgent.updateUpAxis = false;
        }

        ActualizarTransiciones();
        establecerDestino();

        v_agente_NavMeshAgent.nextPosition = transform.position;

        calcularAngulo();
    }

    private void LateUpdate()
    {
        ActualizarTransiciones();
        establecerDestino();

        v_agente_NavMeshAgent.nextPosition = transform.position;
    }

    // ***********************( Funciones Nuestras )*********************** //
    protected void establecerDestino()
    {
        if (ControladorPPAL.V_pausado_b)
            return;

        if (QuedarteQuieto)
            v_objetivo_t = null;

        if (v_objetivo_t == null)
        {
            if (v_agente_NavMeshAgent != null)
            {
                v_agente_NavMeshAgent.isStopped = true;
                v_agente_NavMeshAgent.ResetPath();
            }
        }
        else
        {
            if (v_agente_NavMeshAgent != null)
            {
                v_agente_NavMeshAgent.isStopped = false;
                v_agente_NavMeshAgent.SetDestination(v_objetivo_t.position);
            }
        }

        if (v_agente_NavMeshAgent != null)
        {
            v_agente_NavMeshAgent.nextPosition = transform.position;

            if (!QuedarteQuieto && v_objetivo_t != null)
                v_agente_NavMeshAgent.SetDestination(v_objetivo_t.position);

            if (v_agente_NavMeshAgent != null)
                v_agente_NavMeshAgent.nextPosition = transform.position;
        }
    }

    protected void calcularAngulo()
    {
        //Angulo_f = Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg;
        //if (Angulo_f < -180f) Angulo_f += 360f;
        //if (Angulo_f > 180f) Angulo_f -= 360f;

        Vector2 direccion = transform.up.normalized;
        Angulo_f = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        if (Angulo_f < 0f)
            Angulo_f += 360f;
    }

    protected void irAlDestino()
    {
        if (v_objetivo_t != null)
        {
            dirigirirAlDestino();
            Avanzar();
        }
    }

    protected void dirigirirAlDestino()
    {
        if (v_objetivo_t != null)
        {
            Vector3 v_direccion_v3 = v_agente_NavMeshAgent.desiredVelocity.normalized;
            Rotar(v_direccion_v3);
        }
    }

    protected void RedirigirHaciaNavMesh()
    {
        if (v_objetivo_t == null)
            return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            // Calcular la dirección hacia el punto más cercano en el NavMesh
            Vector3 direccionHaciaNavMesh = (hit.position - transform.position).normalized;

            Rotar(direccionHaciaNavMesh);
            Avanzar();
        }
        else
        {
            Debug.LogWarning("No se pudo encontrar un punto válido en el NavMesh.");
        }
    }

    public void Rotar(Vector3 v_direccion_v3)
    {
        if (ControladorPPAL.V_pausado_b)
            return;

        if (v_objetivo_t == null)
            return;

        float v_anguloActual_f = Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg;
        float v_anguloObjetivo_f = Mathf.Atan2(v_direccion_v3.y, v_direccion_v3.x) * Mathf.Rad2Deg;

        float v_diferenciaAngulo_f = Mathf.DeltaAngle(v_anguloActual_f, v_anguloObjetivo_f);

        float v_torque_f;
        if (v_exodia_b)
        {
            v_torque_f = v_diferenciaAngulo_f * (fuerzaRotacion / 2) * Time.fixedDeltaTime;
        }
        else
        {
            if (v_diferenciaAngulo_f > 0)
                v_torque_f = fuerzaRotacion * Time.fixedDeltaTime;
            else
                v_torque_f = -fuerzaRotacion * Time.fixedDeltaTime;
        }

        v_rb_rb2D.AddTorque(v_torque_f);

        Direcion = f_obtenerDirecion_e(transform);
    }

    public void Avanzar()
    {
        if (ControladorPPAL.V_pausado_b)
            return;

        if (v_objetivo_t == null)
            return;

        RaycastHit2D v_hit = Physics2D.Raycast(transform.position, transform.up, 1.15f, 12);
        if (!v_hit)
        {
            if (!v_exodia_b)
                v_rb_rb2D.AddForce(transform.up * aceleracion * Time.fixedDeltaTime);
            else
                v_rb_rb2D.AddForce(transform.up * v_aceleracionExodia_f * Time.fixedDeltaTime);
        }

        v_agente_NavMeshAgent.nextPosition = transform.position;
    }

    
    public void Empujar(float v_fuerza_f, Vector3 v_direccion_v3 = default)
    {
        if (v_direccion_v3 == default)
            v_direccion_v3 = -Vector3.up;

        v_rb_rb2D.AddForce
        (
            (v_direccion_v3 * v_fuerza_f),
            ForceMode2D.Impulse
        );
    }



    // ***********************( Funciones Funciononales )*********************** //
    protected Direcion_e f_obtenerDirecion_e(Transform _transform)
    {
        if (_transform == null)
            return Direcion_e.NULO;

        Vector3 _direcionActual_v3 = _transform.up.normalized;

        if (Mathf.Abs(_direcionActual_v3.x) > Mathf.Abs(_direcionActual_v3.y))
            return (_direcionActual_v3.x > 0) ? Direcion_e.DERECHA : Direcion_e.IZQUIERDA;
        else
            return (_direcionActual_v3.y > 0) ? Direcion_e.ARRIBA : Direcion_e.ABAJO;
    }



    // ***********************( MAQUINA DE ESTADOS )*********************** //
    class EstadoMoviendose : EstadoBase
    {
        protected MovimientoCopia v_movimiento;
        public override void Init<T>(T dependencia)
        {
            v_movimiento = dependencia as MovimientoCopia;
        }


        public override void Entrar()
        {
            v_movimiento.Empujar(0.025f, transform.up);
        }
        public override void Salir()
        {

        }

        public override void MiFixedUpdate()
        {
            if (ControladorPPAL.V_pausado_b)
                return;

            if (v_movimiento.v_agente_NavMeshAgent != null)
            {
                if (!v_movimiento.v_agente_NavMeshAgent.isOnNavMesh)
                {
                    Debug.LogWarning("El agente está fuera del NavMesh. Redirigiendo...");
                    v_movimiento.RedirigirHaciaNavMesh();
                    return;
                }

                v_movimiento.irAlDestino();
            }
        }
    }

    class EstadoQuieto : EstadoBase
    {
        protected MovimientoCopia v_movimiento;
        public override void Init<T>(T dependencia)
        {
            v_movimiento = dependencia as MovimientoCopia;
        }


        public override void Entrar()
        {
            v_movimiento.Empujar(0f, -transform.up);
        }
        public override void Salir()
        { }

        public override void MiFixedUpdate()
        {
            v_movimiento.dirigirirAlDestino();
        }
    }


    // ***********************( Gizmos )*********************** //
    private void OnDrawGizmos()
    {
        if (v_objetivo_t == null)
            return;


        #if UNITY_EDITOR
            Handles.color = Color.red;
            Handles.Label(transform.position + Vector3.up * 0.4f,
                    "MOVIMIENTO : Angulo_f -> " + Angulo_f + " | " +
                    "objetivo -> " + v_objetivo_t.name);
        #endif
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

/* // MoviNazareno
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
 */