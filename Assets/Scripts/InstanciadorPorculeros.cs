using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InstanciadorPorculeros : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    [Header("**---- Grupos ----**")]
    [SerializeField] private List<GameObject> _grupo1;
    [SerializeField] private List<GameObject> _grupo2;
    [SerializeField] private List<GameObject> _grupo3;

    [Header("**---- Atribustos ----**")]
    [SerializeField] private int _cantidad = 1;
    [SerializeField] private float _largo = 1f;
    [SerializeField] private float _ancho = 1f;

    // ***********************( Metodos de Unity )*********************** //
    private void Awake()
    {
        List<List<GameObject>> grupos = new List<List<GameObject>> { _grupo1, _grupo2, _grupo3 };
        List<int> indicesNoVacios = new List<int>();

        for (int i = 0; i < grupos.Count; i++)
        {
            if (grupos[i].Count > 0)
            {
                indicesNoVacios.Add(i);
            }
        }

        
        if (indicesNoVacios.Count > 0)
        {
            int indiceAleatorio = indicesNoVacios[Random.Range(0, indicesNoVacios.Count)];
            if (_cantidad >= 1)
            {
                for (int i = 0; i < _cantidad; i++)
                {
                    Vector3 posicionAleatoria = new Vector3(
                        Random.Range(-_largo / 2, _largo / 2),
                        0,
                        Random.Range(-_ancho / 2, _ancho / 2)
                    );
                    GameObject prefabAleatorio = grupos[indiceAleatorio][Random.Range(0, grupos[indiceAleatorio].Count)];
                    Instantiate(prefabAleatorio, transform.position + posicionAleatoria, Quaternion.identity);
                }
            }
            else
            {
                Debug.LogWarning($"---{gameObject.name} La cantidad de instancias es menor a 1---");
            }
        }
        else
        {
            Debug.LogWarning($"---{gameObject.name} Todos los grupos están vacíos---");
        }
    }

    // ***********************( Metodos Nuestros )*********************** //
    // ***********************( Metodos Debug )*********************** //
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(_largo, _ancho, 0f));
    }
}
