using CommandTerminal;
using System;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class Salud : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    [Header("*--- Atribustos ---*")]
    [SerializeField]
    private float saludMaxima = 100f;
    public float SaludMaxima => saludMaxima;

    [SerializeField] [Range(0.0f, 1.0f)]
    private float resistencia = 0f;

    [SerializeField] [Range(0.0f, 1.0f)]
    private float reflejoDanno = 0.0f;

    [SerializeField]
    private float tiempoInmunidad = 0.0f;

    [SerializeField]
    private GameObject[] prefabsMorir;

    [SerializeField] private GameObject _fantasma_go;

    private float v_saludActual_f = 1f;
    private float v_tiempoInmunidadActual_f = 0f;

    // ----( Componentes )---- //
    private Rigidbody2D v_rb_c;
    private Movimiento v_movimiento_c;

    [SerializeField]
    private Lifebar lifeBar;

    // ----( Eventos )---- //
    public event Action OnMuerto;

    // ***********************( Metodos UNITY )*********************** //
    private void Start()
    {
        v_saludActual_f = saludMaxima;

        v_rb_c = GetComponent<Rigidbody2D>();
        if (v_rb_c == null)
            Debug.LogError($"****** Entidad: {gameObject.name} NO tiene componente (Rigidbody2D) ******");

        v_movimiento_c = GetComponent<Movimiento>();
        if (v_movimiento_c == null)
            Debug.LogError($"****** Entidad: {gameObject.name} NO tiene componente (Movimiento) ******");

        if (lifeBar != null)
        {
            lifeBar.maxHP = saludMaxima;
            lifeBar.objHP = v_saludActual_f;
        }
    }

    private void Update()
    {
        if (v_tiempoInmunidadActual_f > 0)
            v_tiempoInmunidadActual_f -= Time.deltaTime;

        if (v_saludActual_f <= 0)
            gestionarMuerte();
    }

    // ***********************( Metodos NUESTROS )*********************** //
    public float? RecibirDano(float v_danno_f, Vector3 v_direccion_v3 = default, float v_fuerzaRetroceso_f = 0f)
    {
        if (v_tiempoInmunidadActual_f > 0)
            return null;

        v_saludActual_f -= v_danno_f * (1 - resistencia);
        v_tiempoInmunidadActual_f = tiempoInmunidad;


        v_movimiento_c.Empujar(v_fuerzaRetroceso_f, v_direccion_v3);

        if (lifeBar != null)
        {
            lifeBar.objHP = v_saludActual_f;
        }

        Terminal.Log("Objeto: " + gameObject.name + ", SaludActual: " + v_saludActual_f);

        return v_danno_f * (1 - reflejoDanno);
    }

    private void gestionarMuerte()
    {
        Terminal.Log("MUERE: " + gameObject.name);

        if (_fantasma_go != null)
            Instantiate(_fantasma_go, transform.position, Quaternion.identity);

        OnMuerto?.Invoke();
        Peloton.peloton.EliminarIntegranteLista(gameObject);
        DestroyImmediate(gameObject);
    }

    // ***********************( Getters Y Setters )*********************** //
    // TODO: Implementarlos para acceder a traves de la consola.
    public float SaludActual { get => v_saludActual_f; }


    // ----------( Funciones de Debug )---------- //
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        {
            Handles.color = Color.red;
            Handles.Label(transform.position + Vector3.up * 0.7f,
                                    $"SALUD : SaludActual -> {SaludActual}");
        }
        #endif
    }
}
