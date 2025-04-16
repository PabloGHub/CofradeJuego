using UnityEngine;

public class Ataque : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //x
    // ----( Atribustos )---- //
    [Header("*-- Atribustos --*")]
    [SerializeField] private float danno = 1f;
    [SerializeField] private float alcance = 1f;
    [SerializeField] private float tiempoRecarga = 1f;
    [SerializeField] private float fuerzaEmpuje = 5f;
    [SerializeField] public bool explosion = false;
    public GameObject prefabExplosion;

    private float v_tiempoDeRecargaAtual_f;

    // ----( Entrantrantes )---- //
    [HideInInspector] public Vector2? _direcion_v2 = null;
    [HideInInspector] public LayerMask? v_capaAtacado_LM = null;
    [HideInInspector] public Vector3? v_inicio_V3 = null;
    //[HideInInspector] public float v_alcance_f;

    // ----( Componentes )---- //
    private Salud v_salud_s;

    // ***********************( Metodos UNITY )*********************** //
    private void Start()
    {
        v_tiempoDeRecargaAtual_f = 0f;
        fuerzaEmpuje = (explosion == true) ? 0f : fuerzaEmpuje;


        v_salud_s = GetComponent<Salud>();
    }

    private void Update()
    {
        if (v_tiempoDeRecargaAtual_f > 0)
            v_tiempoDeRecargaAtual_f -= Time.deltaTime;
    }

    // ***********************( Metodos NUESTROS )*********************** //
    public void Atacar()
    {
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


        Vector3 _inicio = (v_inicio_V3 != null) ? v_inicio_V3.Value : transform.position;

        RaycastHit2D _golpe = Physics2D.Raycast(_inicio, (Vector2)_direcion_v2, alcance, v_capaAtacado_LM.Value);
        if (_golpe)
        {
            if (v_tiempoDeRecargaAtual_f <= 0)
            {
                Debug.Log("Golpeado: " + _golpe.collider.name);

                Salud _salud = _golpe.collider.GetComponent<Salud>();
                // TODO: Que instancie Bala en vez de explosion.
                if (explosion) 
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