using UnityEngine;

public class Ataque : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //x
    // ----( Atribustos )---- //
    [Header("*-- Atribustos --*")]
    [SerializeField] private float danno = 1;
    [SerializeField] private float alcance = 1;
    [SerializeField] private float tiempoRecarga = 1;

    private float v_tiempoDeRecargaAtual_f;

    // ----( Entrantrantes )---- //
    [HideInInspector] public float? v_direcion_f = null;
    [HideInInspector] public LayerMask? v_capaAtacado_LM = null;
    [HideInInspector] public Vector3? v_inicio_V3 = null;
    //[HideInInspector] public float v_alcance_f;

    // ***********************( Metodos UNITY )*********************** //
    private void Start()
    {
        v_tiempoDeRecargaAtual_f = 0f;
    }

    private void Update()
    {
        if (v_tiempoDeRecargaAtual_f > 0)
        {
            v_tiempoDeRecargaAtual_f -= Time.deltaTime;
        }
    }

    // ***********************( Metodos NUESTROS )*********************** //
    public void Atacar()
    {
        if (v_direcion_f == null || v_capaAtacado_LM == null || v_inicio_V3 == null)
        {
            Debug.LogError
            (
                "v_direcion_f: " + ((v_direcion_f == null) ? "null" : v_direcion_f) +
                "v_capaAtacado_LM: " + ((v_capaAtacado_LM == null) ? "null" : v_capaAtacado_LM) +
                "v_inicio_V3: " + ((v_inicio_V3 == null) ? "null" : v_inicio_V3)
            );
            return;
        }

        if (v_tiempoDeRecargaAtual_f <= 0)
        {
            Vector2 _direccion = new Vector2(v_direcion_f.Value, 0);
            Vector3 _inicio = (v_inicio_V3 != null) ? v_inicio_V3.Value : transform.position;

            RaycastHit2D v_golpe = Physics2D.Raycast(_inicio, _direccion, alcance, v_capaAtacado_LM.Value);
            if (v_golpe)
            {
                Debug.Log("Golpeado: " + v_golpe.collider.name);
                // Aqui se puede aplicar el daño al objeto golpeado
                // v_golpe.collider.GetComponent<Salud>().RecibirDano(danno);
            }

            v_tiempoDeRecargaAtual_f = tiempoRecarga;
        }
        else
        {
            Debug.Log("Ataque en recarga. Tiempo restante: " + v_tiempoDeRecargaAtual_f);
        }
    }
}