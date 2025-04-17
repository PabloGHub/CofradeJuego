using System.Collections.Generic;
using UnityEngine;

public class AnimacionesMovimiento : MaquinaDeEstados
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
            CrearEstado<EstadoArriba, AnimacionesMovimiento>(this),
            CrearEstado<EstadoAbajo, AnimacionesMovimiento>(this),
            CrearEstado<EstadoIzquierda, AnimacionesMovimiento>(this),
            CrearEstado<EstadoDerecha, AnimacionesMovimiento>(this)
        };
        CambiarEstado(1);
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
    // ***********************( Funciones Propias )*********************** //
    private bool f_ieneParametro_b(Animator animator, string nombre)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == nombre)
                return true;
        }
        return false;
    }


    // ***********************( ESTADOS DE LA MAQUINA DE ESTADOS )*********************** //
    class EstadoArriba : EstadoBase
    {
        AnimacionesMovimiento _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesMovimiento;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_ieneParametro_b(_animaciones._animator, "Subiendo"))
                _animaciones._animator.SetBool("Subiendo", true);

            if (_animaciones.Sprite != null)
                _animaciones.Sprite.flipX = false;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_ieneParametro_b(_animaciones._animator, "Subiendo"))
                _animaciones._animator.SetBool("Subiendo", false);
        }
    }
    class EstadoAbajo : EstadoBase
    {
        AnimacionesMovimiento _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesMovimiento;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_ieneParametro_b(_animaciones._animator, "Bajando"))
                _animaciones._animator.SetBool("Bajando", true);

            if (_animaciones.Sprite != null)
                _animaciones.Sprite.flipX = false;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_ieneParametro_b(_animaciones._animator, "Bajando"))
                _animaciones._animator.SetBool("Bajando", false);
        }
    }
    class EstadoIzquierda : EstadoBase
    {
        AnimacionesMovimiento _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesMovimiento;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_ieneParametro_b(_animaciones._animator, "Lateralmente"))
                _animaciones._animator.SetBool("Lateralmente", true);

            if (_animaciones.Sprite != null && _animaciones.Flipear)
                _animaciones.Sprite.flipX = true;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_ieneParametro_b(_animaciones._animator, "Lateralmente"))
                _animaciones._animator.SetBool("Lateralmente", false);
        }
    }
    class EstadoDerecha : EstadoBase
    {
        AnimacionesMovimiento _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesMovimiento;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_ieneParametro_b(_animaciones._animator, "Lateralmente"))
                _animaciones._animator.SetBool("Lateralmente", true);

            if (_animaciones.f_ieneParametro_b(_animaciones._animator, "Derecha"))
                _animaciones._animator.SetBool("Derecha", true);
                

            if (_animaciones.Sprite != null)
                _animaciones.Sprite.flipX = false;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_ieneParametro_b(_animaciones._animator, "Lateralmente"))
                _animaciones._animator.SetBool("Lateralmente", false);

            if (_animaciones.f_ieneParametro_b(_animaciones._animator, "Derecha"))
                _animaciones._animator.SetBool("Derecha", false);
            
        }
    }
}
