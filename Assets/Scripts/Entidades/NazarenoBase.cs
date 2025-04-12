using UnityEngine;

public class NazarenoBase : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    private float cercaniaAlObjetivo = 3f;
    private int v_objetivoIndex_i = 0;
    private Movimiento v_movimiento;

    // ***********************( Funciones Unity )*********************** //
    private void Start()
    {
        v_movimiento = GetComponent<Movimiento>();
        if (v_movimiento == null)
        {
            Debug.LogError("El objeto no tiene un componente Movimiento.");
            return;
        }

        v_movimiento.v_objetivo_Transform = Navegacion.nav.trayectoria[v_objetivoIndex_i];
    }

    private void Update()
    {
        if (ControladorPPAL.v_pausado_b)
            return;

        if (Vector3.Distance(transform.position, v_movimiento.v_objetivo_Transform.position) < cercaniaAlObjetivo)
        {
            Debug.Log("PuntoControl Alcanzado");

            v_objetivoIndex_i++;

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
            

            if (v_objetivoIndex_i >= Navegacion.nav.trayectoria.Length)
                v_objetivoIndex_i = 0; // Creara un bucle.

            v_movimiento.v_objetivo_Transform = Navegacion.nav.trayectoria[v_objetivoIndex_i];
        }
    }
    // ***********************( Funciones Nuestras )*********************** //
}
