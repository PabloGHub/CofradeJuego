using System;
using UnityEngine;

public class MoviCamara : MonoBehaviour
{
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
        if (ControladorPPAL.v_pausado_b)
            return;

        if (Input.GetKey(v_arriba_kc))
        {
            transform.position += (Vector3.up * v_velocidad_f) * Time.deltaTime;
        }
        else if (Input.GetKey(v_abajo_kc))
        {
            transform.position += (Vector3.down * v_velocidad_f) * Time.deltaTime;
        }
        if (Input.GetKey(v_izquierda_kc))
        {
            transform.position += (Vector3.left * v_velocidad_f) * Time.deltaTime;
        }
        else if (Input.GetKey(v_derecha_kc))
        {
            transform.position += (Vector3.right * v_velocidad_f) * Time.deltaTime;
        }
    }
    // ***********************( Metodos NUESTROS )*********************** //
}
