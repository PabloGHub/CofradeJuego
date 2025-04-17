using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ataque : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    // ----( Unity )---- //
    [Header("**---- Atribustos ----**")]
    [SerializeField] private float alcance = 1f;
    public float Alcance => alcance;
    [SerializeField] private float RangoVisibliidad;
    [SerializeField] private float tiempoRecarga = 1f;
    public float TiempoRecarga => tiempoRecarga;
    [SerializeField] private float fuerzaEmpuje = 5f;
    [SerializeField] private LayerMask capaAtacado;

    [Header("**---- Danno Mele ----**")]
    [SerializeField] private float danno = 1f;
    public float Danno => danno;

    [Header("**---- Danno Distancia ----**")]
    [SerializeField] private bool EsDistancia = false;
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private float _alcanceExplosion_f;


    [Header("**---- Caracteristicas ----**")]
    [SerializeField] private float _inicio = 0;
    private Vector3 _inicioR_v3;
    // ----( Fin Unity )---- //


    private float v_tiempoDeRecargaAtual_f;
    private HashSet<GameObject> _listaEnemigos;

    [HideInInspector]
    public GameObject EnemigoObjetivo_go;


    [HideInInspector] public bool _atacar_b;
    // ----( Eventos )---- //
    public event Action OnEnemigosCerca;
    public event Action OnSinEnemigos;

    // ----( Componentes )---- //
    private Salud v_salud_s;

    // ***********************( Metodos UNITY )*********************** //
    private void Start()
    {
        v_tiempoDeRecargaAtual_f = 0f;

        v_salud_s = GetComponent<Salud>();
        if (v_salud_s == null)
            Debug.LogError($"****** Entidad: {gameObject.name} NO tiene componente (Salud) ******");

        StartCoroutine(f_detectarPeriodicamente());
    }

    private void Update()
    {
        if (ControladorPPAL.v_pausado_b)
            return;

        if (v_tiempoDeRecargaAtual_f > 0)
            v_tiempoDeRecargaAtual_f -= Time.deltaTime;

        if (_atacar_b)
            Atacar();
    }


    // ***********************( IEnumetaros )*********************** //
    private IEnumerator f_detectarPeriodicamente()
    {
        while (true)
        {
            detectarEnemigos();
            yield return new WaitForSeconds(0.25f);
        }
    }

    // ***********************( Metodos NUESTROS )*********************** //
    private void detectarEnemigos()
    {
        Collider2D[] _eemigos_Array = Physics2D.OverlapCircleAll(transform.position, RangoVisibliidad, capaAtacado);
        _listaEnemigos = new HashSet<GameObject>(_eemigos_Array.Select(e => e.gameObject));

        comprobarListaObjetivos();
    }
    void comprobarListaObjetivos()
    {
        _listaEnemigos.RemoveWhere(e => e == null || !e.activeInHierarchy || e.GetComponent<Salud>()?.SaludActual <= 0);
        EnemigoObjetivo_go = _listaEnemigos
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .ThenBy(e => e.GetComponent<Salud>().SaludActual)
            .FirstOrDefault();

        if (EnemigoObjetivo_go != null)
            OnEnemigosCerca?.Invoke();
        else
            OnSinEnemigos?.Invoke();
    }


    public void Atacar()
    {
        if (v_tiempoDeRecargaAtual_f > 0)
            return;


        if (EsDistancia)
        {
            // TODO: Instanciar un proyectil.
        }
        else
        {
            _inicioR_v3 = transform.position + transform.up * _inicio;
            //Debug.Log($"Intentando Atacar:{gameObject.name}; _inicioR_v3:{_inicioR_v3}; transform.up:{transform.up}; alcance:{alcance}; capaAtacado:{capaAtacado.ToString()}");
            RaycastHit2D _golpe = Physics2D.Raycast(_inicioR_v3, transform.up, alcance, capaAtacado);
            if (_golpe)
            {
                Debug.Log(gameObject.name + " golpeo: " + _golpe.collider.name);

                Salud _salud = _golpe.collider.GetComponent<Salud>();
                if (_salud != null)
                {
                    float? _ref = _salud.RecibirDano(danno, transform.position, fuerzaEmpuje);

                    if (_ref != null && v_salud_s != null)
                        v_salud_s.RecibirDano((float)_ref, Vector3.one);
                }
            }
            else
            {
                //Debug.Log("Fallo: " + gameObject.name);
            }
        }
        

        v_tiempoDeRecargaAtual_f = tiempoRecarga;
    }

    // ----------( Funciones Funcionales )---------- //
    Vector3 f_predecirPosicion_v3(Transform _enemigo, float _tiempo)
    {
        Rigidbody2D _rb = _enemigo.GetComponent<Rigidbody2D>();
        return (_rb != null) ? _enemigo.position + (Vector3)(_rb.linearVelocity * _tiempo) : _enemigo.position;
    }

    // ----------( Funciones de Debug )---------- //
    private void OnDrawGizmos()
    {
        // Visibilidad
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, RangoVisibliidad);

        // Alcance
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + transform.up * _inicio, transform.position + transform.up * (_inicio + alcance));
    }
}