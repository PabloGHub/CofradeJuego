using UnityEngine;

public class Explosion : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    private float v_cuantraAtras_f = 0.25f;

    // ----( Entrantes Y Atributos )---- //
    [Header("*-- Atributos --*")]
    [SerializeField] private float fuerza = 10f;
    [SerializeField] private float tamanno = 5f;
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
        }
    }

    // ***********************( Metodos NUESTROS )*********************** //
    public void Personalizar(float v_fuerza_f, float v_tamanno_f, LayerMask v_mascaraAfectas_lm)
    {
        this.fuerza = v_fuerza_f;
        this.tamanno = v_tamanno_f;
        this.mascarasAfectadas = v_mascaraAfectas_lm;
    }

    private void detonar(float v_fuerza_f, float v_tamanno_f)
    {
        Collider2D[] _colisao = Physics2D.OverlapCircleAll(transform.position, v_tamanno_f);

        foreach (Collider2D _c in _colisao)
        {
            if (_c.gameObject.layer == mascarasAfectadas)
                continue;

            Salud _salud = _c.GetComponent<Salud>();
            if (_salud != null)
            {
                float _distancia = Vector2.Distance(_c.transform.position, transform.position);
                float _forca = Mathf.Clamp(v_fuerza_f / _distancia, 0f, v_fuerza_f);
                _salud.RecibirDano(_forca);
            }

            Rigidbody2D _rb = _c.GetComponent<Rigidbody2D>();
            if (_rb != null)
            {
                Vector2 _direccion = _c.transform.position - transform.position;
                _rb.AddForce(_direccion.normalized * v_fuerza_f, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }
}
