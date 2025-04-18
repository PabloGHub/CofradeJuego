using System;
using UnityEngine;

public class MoviCamara : MonoBehaviour
{
    // TODO: Que la camara se pueda alejar y acercar con la rueda del raton.
    // ***********************( Declaraciones )*********************** //
    [Header("*--- Atributos ---*")]
    [SerializeField]
    private float v_velocidad_f = 2f;

    // --- Teclas --- //
    private KeyCode v_arriba_kc;
    private KeyCode v_abajo_kc;
    private KeyCode v_izquierda_kc;
    private KeyCode v_derecha_kc;

    // ***********************( Metodos de UNITY )*********************** //
    private void Start()
    {
        v_arriba_kc = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("arriba", KeyCode.W.ToString()));
        v_abajo_kc = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("abajo", KeyCode.S.ToString()));
        v_izquierda_kc = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("izquierda", KeyCode.A.ToString()));
        v_derecha_kc = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("derecha", KeyCode.D.ToString()));
    }

    private void Update()
    {
        if (ControladorPPAL.V_pausado_b && ControladorPPAL.ppal.EnCurso_f)
            return;

        Vector3 nuevaPosicion = transform.position;

        if (Input.GetKey(v_arriba_kc))
        {
            nuevaPosicion += (Vector3.up * v_velocidad_f) * Time.deltaTime;
        }
        else if (Input.GetKey(v_abajo_kc))
        {
            nuevaPosicion += (Vector3.down * v_velocidad_f) * Time.deltaTime;
        }
        if (Input.GetKey(v_izquierda_kc))
        {
            nuevaPosicion += (Vector3.left * v_velocidad_f) * Time.deltaTime;
        }
        else if (Input.GetKey(v_derecha_kc))
        {
            nuevaPosicion += (Vector3.right * v_velocidad_f) * Time.deltaTime;
        }

        // Limitar la posición dentro de los límites definidos en ControladorPPAL
        nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, ControladorPPAL.ppal.Esquina1_v2.x, ControladorPPAL.ppal.Esquina2_v2.x);
        nuevaPosicion.y = Mathf.Clamp(nuevaPosicion.y, ControladorPPAL.ppal.Esquina1_v2.y, ControladorPPAL.ppal.Esquina2_v2.y);

        transform.position = nuevaPosicion;
    }
    // ***********************( Metodos NUESTROS )*********************** //
}
