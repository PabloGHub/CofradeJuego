using System;
using UnityEngine;

public class Salud : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    [Header("*--- Atribustos ---*")]
    [SerializeField]
    private float saludMaxima = 100f;

    [SerializeField] [Range(0.0f, 1.0f)]
    private float resistencia = 0f;

    [SerializeField] [Range(0.0f, 1.0f)]
    private float reflejoDanno = 0.0f;

    [SerializeField]
    private float tiempoInmunidad = 0.0f;

    [SerializeField]
    private GameObject[] prefabsMorir;

    private float v_saludActual_f = 1f;
    private float v_tiempoInmunidadActual_f = 0f;

    // ----( Componentes )---- //
    private Rigidbody2D v_rb_c;
    private Movimiento v_movimiento_c;

    // ***********************( Metodos UNITY )*********************** //
    private void Start()
    {
        v_saludActual_f = saludMaxima;

        v_rb_c = GetComponent<Rigidbody2D>();
        v_movimiento_c = GetComponent<Movimiento>();
    }

    private void Update()
    {
        if (v_tiempoInmunidadActual_f > 0)
            v_tiempoInmunidadActual_f -= Time.deltaTime;

        if (saludMaxima <= 0)
            gestionarMuerte();
    }

    // ***********************( Metodos NUESTROS )*********************** //
    public float? RecibirDano(float v_danno_f, float v_fuerzaRetroceso_f = 0f)
    {
        if (v_tiempoInmunidadActual_f > 0)
            return null;

        v_saludActual_f -= v_danno_f * (1 - resistencia);
        v_tiempoInmunidadActual_f = tiempoInmunidad;

        v_movimiento_c.Empujar(v_fuerzaRetroceso_f);

        return v_danno_f * (1 - reflejoDanno);
    }

    private void gestionarMuerte()
    {
        // TODO: Implementar la lógica para manejar la muerte del objeto.
        gameObject.SetActive(false); // Termporral/PlaceHolder.
    }

    // ***********************( Getters Y Setters )*********************** //
    // TODO: Implementarlos para acceder a traves de la consola.
}
