using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// NOTA: Copilot el proyecto es 2D.
public class Proyectil : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    [Header("*-- Atributos --*")]
    [SerializeField] private float danno = 10f;
    [SerializeField] private float fuerzaEmpuje = 5f;
    [SerializeField] private float _velocidad_f = 10f;
    [SerializeField] private float _maxTiempoVuelo_f = 10f;

    [Header("*-- Prefab de Explosion --*")]
    [SerializeField] private bool _explosionar = false;
    [SerializeField] private GameObject _prefabExplosion_go;

    [Header("*-- Lista de capas Colisionar --*")]
    [SerializeField] private LayerMask _capas;

    private float _tiempoRestante_f = 0f;
    private RaycastHit2D _anteriorJit;

    // ***********************( Metodos Unity )*********************** //
    private void Awake()
    {
        _tiempoRestante_f = _maxTiempoVuelo_f;
    }

    private void Update()
    {
        _tiempoRestante_f -= Time.deltaTime;
        if (_tiempoRestante_f <= 0f)
        {
            if (_explosionar)
            {
                colisionar();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        transform.Translate(Vector2.up * _velocidad_f * Time.deltaTime);

        RaycastHit2D _hit = Physics2D.Raycast
        (
            transform.position,
            transform.up,
            0.15f,
            _capas
        );
        if (_hit)
        {
            _anteriorJit = _hit;

            if (_explosionar)
            {
                colisionar();
            }
            else
            {
                Salud _salud = _hit.collider.GetComponent<Salud>();
                if (_salud != null)
                {
                    _salud.RecibirDano(danno, transform.position, fuerzaEmpuje);
                }
                Destroy(gameObject);
            }
        }
    }

    // ***********************( Metodos Nuestros )*********************** //
    private void colisionar()
    {
        GameObject _explosion = Instantiate(_prefabExplosion_go, transform.position, Quaternion.identity);
        _explosion.transform.localScale = new Vector3(1f, 1f, 1f);
        Destroy(gameObject);
    }

    public LayerMask F_CombinarCapas_LM(List<LayerMask> _capas)
    {
        int _combinadas = 0;
        foreach (LayerMask _capa in _capas)
        {
            _combinadas |= _capa.value;
        }
        return _combinadas;
    }

    // ***********************( Gizmos )*********************** //
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.15f);

        {
        #if UNITY_EDITOR
            Handles.Label(transform.position + Vector3.up * 0.5f,
                            $"PROYECTIL : tiempoRestante -> {_tiempoRestante_f} | hitAnterior -> {_anteriorJit}");
        #endif
        }

    }
}
