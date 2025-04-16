using System;
using System.Collections.Generic;
using UnityEngine;

public class Ataque : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //x
    // ----( Atribustos )---- //
    [Header("**---- Atribustos ----**")]
    [SerializeField] private float danno = 1f;
    [SerializeField] private float alcance = 1f;
    [SerializeField] private float tiempoRecarga = 1f;
    [SerializeField] private float fuerzaEmpuje = 5f;
    [SerializeField] public bool EsDistancia = false;

    [Header("**---- Caracteristicas ----**")]
    public Vector2? _direcion_v2;
    public LayerMask? v_capaAtacado_LM;
    public Vector3? v_inicio_V3;
    public GameObject prefabExplosion;
    public float RangoVisibliidad;


    private float v_tiempoDeRecargaAtual_f;
    private GameObject _enemigoObjetivo_go;
    private CircleCollider2D _circle_coll;
    private List<GameObject> _listaNazarenos;
    private Dictionary<GameObject, Action> _nazarenosDelegados = new Dictionary<GameObject, Action>();


    // ----( Entrantes )---- //
    [HideInInspector] public bool _atacar_b;

    // ----( Componentes )---- //
    private Salud v_salud_s;

    // ***********************( Metodos UNITY )*********************** //
    private void Start()
    {
        v_tiempoDeRecargaAtual_f = 0f;
        fuerzaEmpuje = (EsDistancia == true) ? 0f : fuerzaEmpuje;


        v_salud_s = GetComponent<Salud>();
    }

    private void Update()
    {
        if (v_tiempoDeRecargaAtual_f > 0)
            v_tiempoDeRecargaAtual_f -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Nazarenos"))
        {
            if (!_listaNazarenos.Contains(collision.gameObject))
            {
                _listaNazarenos.Add(collision.gameObject);
                Salud v_salud_s = collision.gameObject.GetComponent<Salud>();
                if (v_salud_s != null)
                {
                    Action v_delegado = () => eliminarNazareno(collision.gameObject);
                    _nazarenosDelegados[collision.gameObject] = v_delegado;
                    v_salud_s.OnMuerto += v_delegado;
                }

            }

            comprobarListaObjetivos();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Nazarenos"))
        {
            if (_listaNazarenos.Contains(collision.gameObject))
            {
                _listaNazarenos.Remove(collision.gameObject);
                Salud v_salud_s = collision.gameObject.GetComponent<Salud>();
                if (v_salud_s != null && _nazarenosDelegados.ContainsKey(collision.gameObject))
                {
                    v_salud_s.OnMuerto -= _nazarenosDelegados[collision.gameObject];
                    _nazarenosDelegados.Remove(collision.gameObject);
                }
            }

            comprobarListaObjetivos();
        }
    }

    // ***********************( Metodos NUESTROS )*********************** //
    void comprobarListaObjetivos()
    {
        if (_listaNazarenos.Count <= 0)
        {
            CambiarEstado(1);
            _enemigoObjetivo_go = null;
            return;
        }

        float _distanciaMinima = float.MaxValue;
        foreach (GameObject v_porculero_go in _listaNazarenos)
        {
            float _distancia = Vector3.Distance(transform.position, v_porculero_go.transform.position);
            if (_distancia < _distanciaMinima)
            {
                _distanciaMinima = _distancia;
                _enemigoObjetivo_go = v_porculero_go;
            }
        }

        if (_enemigoObjetivo_go != null)
            CambiarEstado(2);
    }
    private void eliminarNazareno(GameObject _objetivo_go)
    {
        if (_listaNazarenos.Contains(_objetivo_go))
        {
            _listaNazarenos.Remove(_objetivo_go);
        }
    }




    public void Atacar()
    {
        if (v_tiempoDeRecargaAtual_f > 0)
            return;

        if (_direcion_v2 == null || v_capaAtacado_LM == null)
        {
            Debug.LogError
            (
                "Error: Ataque.cs - Atacar() - Alguno de los atributos es null.\n" +
                "v_direcion_f: " + ((_direcion_v2 == null) ? "null" : _direcion_v2) + "\n" + 
                "v_capaAtacado_LM: " + ((v_capaAtacado_LM == null) ? "null" : v_capaAtacado_LM) + "\n" +
                "v_inicio_V3: " + ((v_inicio_V3 == null) ? "null" : v_inicio_V3)
            );
            return;
        }


        Vector3 _inicio = v_inicio_V3 ?? transform.position;

        RaycastHit2D _golpe = Physics2D.Raycast(_inicio, (Vector2)_direcion_v2, alcance, v_capaAtacado_LM.Value);
        if (_golpe)
        {
            if (v_tiempoDeRecargaAtual_f <= 0)
            {
                Debug.Log("Golpeado: " + _golpe.collider.name);

                Salud _salud = _golpe.collider.GetComponent<Salud>();
                // TODO: Que instancie Bala en vez de explosion.
                if (EsDistancia) 
                {
                    GameObject v_objetoExplosion_go = Instantiate(prefabExplosion, _golpe.point, Quaternion.identity);
                    Explosion v_explosion_s = v_objetoExplosion_go.GetComponent<Explosion>();
                    if (v_explosion_s != null)
                        v_explosion_s.Personalizar(fuerzaEmpuje, alcance, v_capaAtacado_LM.Value);
                    
                }
                else if (_salud != null)
                {
                    float? _ref = _salud.RecibirDano(danno, _direcion_v2.Value, fuerzaEmpuje);

                    if (_ref != null && v_salud_s != null)
                        v_salud_s.RecibirDano((float)_ref, Vector3.one);
                }
            }
        }

        v_tiempoDeRecargaAtual_f = tiempoRecarga;
    }
}