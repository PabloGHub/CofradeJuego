using UnityEngine;

public class Explosion : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    private float v_cuantraAtras_f = 0.25f;

    // ----( Entrantes Y Atributos )---- //
    [Header("*-- Atributos --*")]
    [SerializeField] private float fuerza = 10f;
    [SerializeField] private float tamanno = 5f;
    [SerializeField] private float upwardsModifier = 0.5f;
    [SerializeField] private LayerMask mascarasAfectadas;

    // ----( Componentes )---- //
    private Rigidbody2D v_rb_c;

    // ***********************( Metodos UNITY )*********************** //
    private void Start()
    {
        v_rb_c = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (v_cuantraAtras_f > 0)
            v_cuantraAtras_f -= Time.deltaTime;
        else
        {
            detonar(fuerza, tamanno);
            Destroy(gameObject);
        }
    }

    // ***********************( Metodos NUESTROS )*********************** //
    public void Personalizar(float v_fuerza_f, float v_tamanno_f, float upwardsModifier, LayerMask v_mascaraAfectas_lm)
    {
        this.fuerza = v_fuerza_f;
        this.tamanno = v_tamanno_f;
        this.upwardsModifier = upwardsModifier;
        this.mascarasAfectadas = v_mascaraAfectas_lm;
    }

    private void detonar(float v_fuerza_f, float v_tamanno_f)
    {
        Collider2D[] _colisao = Physics2D.OverlapCircleAll(transform.position, v_tamanno_f);

        foreach (Collider2D _c in _colisao)
        {
            Salud _salud = _c.GetComponent<Salud>();
            if (_salud != null)
            {
                float _distancia = Vector2.Distance(_c.transform.position, transform.position);
                float _forca = Mathf.Clamp(v_fuerza_f / _distancia, 0f, v_fuerza_f);
                _salud.RecibirDano(_forca);
                v_rb_c.AddExplosionForce
                (

                );
            }
        }
    }
}
