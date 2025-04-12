using UnityEngine;

public class Movimiento : MonoBehaviour
{
    // TODO: Al pausar se detiene y al despausar aplicar fuerza para compensar.
    // TODO: Que no acelere si tiene una pared delante.
    // TODO: Que calcule el tiempo apoximado al alza para llegar al objetivo y si lo supera que paso al siguiente.
    // ***********************( Declaraciones )*********************** //
    [Header("*-- Atributos --*")]

    [SerializeField]
    private float aceleracion = 2f;
    [SerializeField]
    private float fuerzaRotacion = 10f;
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

        v_rb_rb2D.AddForce(transform.up * aceleracion * Time.fixedDeltaTime);
    }

    // ***********************( Funciones Nuestras )*********************** //
}


