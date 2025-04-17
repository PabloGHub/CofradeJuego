using System.Collections.Generic;
using UnityEngine;

public class Animaciones : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
    [SerializeField]
    private bool Flipear;

    // ----( Componentes )---- //
    private Animator _animator;
    private Movimiento _movimiento;
    [HideInInspector] public SpriteRenderer Sprite;

    // ----( Maquina de estados )---- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }

    // ***********************( Metodos UNITY )*********************** //
    private void Awake()
    {
        // Maquina de Estados
        Inicializar(gameObject);
        estadosPosibles = new List<EstadoBase>
        {
            CrearEstado<EstadoArriba, Animaciones>(this),
            CrearEstado<EstadoAbajo, Animaciones>(this),
            CrearEstado<EstadoIzquierda, Animaciones>(this),
            CrearEstado<EstadoDerecha, Animaciones>(this)
        };
    }

    private void OnEnable()
    {
        // Movimiento del padre
        _movimiento = GetComponentInParent<Movimiento>();
        if (_movimiento == null)
            Debug.LogError($"****** Entidad: {gameObject.name} NO tiene componente (Movimiento) ******");

        // SpriteRenderer
        Sprite = GetComponent<SpriteRenderer>();
        if (Sprite == null)
            Debug.LogError($"****** Entidad: {gameObject.name} NO tiene componente (SpriteRenderer) ******");

        // Animator
        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError($"****** Entidad: {gameObject.name} NO tiene componente (Animator) ******");
    }

    private void Update()
    {
        if (_animator == null || _movimiento == null || Sprite == null)
        {
            Debug.LogError($"****** Entidad: {gameObject.name} NO tiene componente (_animator, _movimiento, Sprite) ******");
            return;
        }

        transform.rotation = Quaternion.identity;
        _animator.SetBool("pausa", ControladorPPAL.v_pausado_b);

        switch (_movimiento.Direcion)
        {
            case Movimiento.Direcion_e.ARRIBA:
                CambiarEstado(0);
            break;


            case Movimiento.Direcion_e.DERECHA:
                CambiarEstado(3);
            break;


            case Movimiento.Direcion_e.IZQUIERDA:
                CambiarEstado(2);
            break;


            case Movimiento.Direcion_e.ABAJO:
                CambiarEstado(1);
            break;

            case Movimiento.Direcion_e.NULO:
                CambiarEstado(1);
            break;
        }
    }


    // ***********************( ESTADOS DE LA MAQUINA DE ESTADOS )*********************** //
    class EstadoArriba : EstadoBase
    {
        Animaciones _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as Animaciones;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.SetBool("Subiendo", true);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", false);

            if (_animaciones.Sprite != null)
                _animaciones.Sprite.flipX = false;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", false);
        }
    }
    class EstadoAbajo : EstadoBase
    {
        Animaciones _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as Animaciones;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", true);
            _animaciones._animator.SetBool("Lateralmente", false);

            if (_animaciones.Sprite != null)
                _animaciones.Sprite.flipX = false;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", false);
        }
    }
    class EstadoIzquierda : EstadoBase
    {
        Animaciones _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as Animaciones;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", true);

            if (_animaciones.Sprite != null && _animaciones.Flipear)
                _animaciones.Sprite.flipX = true;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", false);
        }
    }
    class EstadoDerecha : EstadoBase
    {
        Animaciones _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as Animaciones;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", true);

            if (_animaciones.Sprite != null)
                _animaciones.Sprite.flipX = false;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", false);
        }
    }
}
