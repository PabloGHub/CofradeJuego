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

    // Control
    List<List<GameObject>> grupos;
    List<int> indicesNoVacios;

    // ***********************( Metodos de Unity )*********************** //
    private void Awake()
    {
        grupos = new List<List<GameObject>> { _grupo1, _grupo2, _grupo3 };
        indicesNoVacios = new List<int>();

        for (int i = 0; i < grupos.Count; i++)
        {
            if (grupos[i].Count > 0)
            {
                indicesNoVacios.Add(i);
            }
        }

        ControladorPPAL.IntanciarEnemigos += instanciar;
    }

    private void Start()
    {
        instanciar();
    }

    // ***********************( Metodos Nuestros )*********************** //
    private void instanciar()
    {
        Debug.Log($"---{gameObject.name} Instanciando---");
        if (indicesNoVacios.Count > 0)
        {
            int indiceAleatorio = indicesNoVacios[Random.Range(0, indicesNoVacios.Count)];
            if (_cantidad >= 1)
            {
                for (int i = 0; i < _cantidad; i++)
                {
                    Vector3 posicionAleatoria = new Vector3(
                        Random.Range(-_largo * 0.5f, _largo * 0.5f),
                        Random.Range(-_ancho * 0.5f, _ancho * 0.5f),
                        0f
                    );
                    GameObject prefabAleatorio = grupos[indiceAleatorio][Random.Range(0, grupos[indiceAleatorio].Count)];
                    GameObject _instanciado = Instantiate(prefabAleatorio, transform.position + posicionAleatoria, Quaternion.identity);
                    _instanciado.GetComponent<Salud>().OnMuerto += () => ControladorPPAL.ppal.EliminarDeLaLista(_instanciado);
                    ControladorPPAL.ppal.Porculeros.Add(_instanciado);
                }
            }
            else
            {
                Debug.LogWarning($"--- {gameObject.name} La cantidad A instanciar es menor a 1 ---");
            }
        }
        else
        {
            Debug.LogWarning($"--- {gameObject.name} Todos los grupos están vacíos ---");
        }
    }


    // ***********************( Metodos Debug )*********************** //
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(_largo, _ancho, 0f));
    }
}
