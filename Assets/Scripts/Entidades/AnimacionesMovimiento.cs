using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class AnimacionesMovimiento : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
    [SerializeField]
    private bool FlipearEnIzquierda;
    [SerializeField]
    private bool FlipearEnDerecha;

    // ----( Componentes )---- //
    private Animator _animator;
    private Movimiento _movimiento;
    [HideInInspector] public SpriteRenderer Sprite;
    private Vector3 _posicionInicial;

    // ----( Maquina de estados )---- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }

    // ***********************( Metodos UNITY )*********************** //
    private void Awake()
    {
        // Maquina de Estados
        // Inicializar(gameObject);
        // estadosPosibles = new List<EstadoBase>
        // {
        //     CrearEstado<EstadoArriba, AnimacionesMovimiento>(this),
        //     CrearEstado<EstadoAbajo, AnimacionesMovimiento>(this),
        //     CrearEstado<EstadoIzquierda, AnimacionesMovimiento>(this),
        //     CrearEstado<EstadoDerecha, AnimacionesMovimiento>(this)
        // };

        // Posicion Inicial
        Debug.Log($"Posicion Inicial: {transform.position}");
        _posicionInicial = transform.localPosition;
    }

    private void OnEnable()
    {
        obtenerComponentes();
        //CambiarEstado(1);
    }
    private void Start()
    {
        obtenerComponentes();
    }

    private void Update()
    {
        if (_animator == null || _movimiento == null || Sprite == null)
        {
            Debug.LogError($"****** Entidad: {gameObject.name} NO tiene componente (_animator, _movimiento, Sprite) ******");
            return;
        }

        transform.rotation = Quaternion.identity;
        transform.localPosition = _posicionInicial;


        if (_movimiento.ObtenerIndice(_movimiento.EstadoActual) == 1)
            _animator.SetBool("pausa", true);
        else
            _animator.SetBool("pausa", false);

        if (ControladorPPAL.v_pausado_b)
            _animator.SetBool("pausa", true);


        switch (_movimiento.Direcion)
        {
            case Movimiento.Direcion_e.ARRIBA:

                if (_animator == null)
                    return;

                if (f_tieneParametro_b(_animator, "Subiendo"))
                    _animator.SetBool("Subiendo", true);

                if (f_tieneParametro_b(_animator, "Lateralmente"))
                    _animator.SetBool("Lateralmente", false);

                if (f_tieneParametro_b(_animator, "Derecha"))
                    _animator.SetBool("Derecha", false);

                if (f_tieneParametro_b(_animator, "Bajando"))
                    _animator.SetBool("Bajando", false);

            break;


            case Movimiento.Direcion_e.DERECHA:

                if (_animator == null)
                    return;

                if (f_tieneParametro_b(_animator, "Lateralmente"))
                    _animator.SetBool("Lateralmente", true);

                if (f_tieneParametro_b(_animator, "Derecha"))
                    _animator.SetBool("Derecha", true);

                if (f_tieneParametro_b(_animator, "Bajando"))
                    _animator.SetBool("Bajando", false);

                if (f_tieneParametro_b(_animator, "Subiendo"))
                    _animator.SetBool("Subiendo", false);

                // Si solo tienes anim de iz, aqui flipeas.
                if (FlipearEnIzquierda || FlipearEnDerecha)
                {
                    if (FlipearEnDerecha)
                    {
                        Sprite.flipX = true;
                    }
                    else if (FlipearEnIzquierda)
                    {
                        Sprite.flipX = false;
                    }
                }

            break;


            case Movimiento.Direcion_e.IZQUIERDA:

                if (_animator == null)
                    return;

                if (f_tieneParametro_b(_animator, "Lateralmente"))
                    _animator.SetBool("Lateralmente", true);

                if (f_tieneParametro_b(_animator, "Subiendo"))
                    _animator.SetBool("Subiendo", false);

                if (f_tieneParametro_b(_animator, "Derecha"))
                    _animator.SetBool("Derecha", false);

                if (f_tieneParametro_b(_animator, "Bajando"))
                    _animator.SetBool("Bajando", false);

                if (FlipearEnIzquierda || FlipearEnDerecha)
                {
                    if (FlipearEnDerecha)
                    {
                        Sprite.flipX = false;
                    }
                    else if (FlipearEnIzquierda)
                    {
                        Sprite.flipX = true;
                    }
                }

                break;


            case Movimiento.Direcion_e.ABAJO: 

                if (_animator == null)
                    return;

                if (f_tieneParametro_b(_animator, "Bajando"))
                    _animator.SetBool("Bajando", true);

                if (f_tieneParametro_b(_animator, "Lateralmente"))
                    _animator.SetBool("Lateralmente", false);

                if (f_tieneParametro_b(_animator, "Subiendo"))
                    _animator.SetBool("Subiendo", false);

                if (f_tieneParametro_b(_animator, "Derecha"))
                    _animator.SetBool("Derecha", false);

            break;

            case Movimiento.Direcion_e.NULO:
                Debug.LogWarning($"****** Entidad: {gameObject.name} NO tiene Direccion (NULO) ******");
            break;
        }
    }
    // ***********************( Funciones Propias )*********************** //
    private bool f_tieneParametro_b(Animator animator, string nombre)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == nombre)
                return true;
        }
        return false;
    }

    private void obtenerComponentes()
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

            if (_animaciones.f_tieneParametro_b(_animaciones._animator, "Subiendo"))
                _animaciones._animator.SetBool("Subiendo", true);

            if (!_animaciones.FlipearEnDerecha)
            {
                if (_animaciones.Sprite != null)
                    _animaciones.Sprite.flipX = false;
            }
            else
                if (_animaciones.Sprite != null)
                    _animaciones.Sprite.flipX = true;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_tieneParametro_b(_animaciones._animator, "Subiendo"))
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

            if (_animaciones.f_tieneParametro_b(_animaciones._animator, "Bajando"))
                _animaciones._animator.SetBool("Bajando", true);

            if (!_animaciones.FlipearEnDerecha)
            {
                if (_animaciones.Sprite != null)
                    _animaciones.Sprite.flipX = false; 
            }
            else
                if (_animaciones.Sprite != null)
                    _animaciones.Sprite.flipX = true;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_tieneParametro_b(_animaciones._animator, "Bajando"))
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

            if (_animaciones.f_tieneParametro_b(_animaciones._animator, "Lateralmente"))
                _animaciones._animator.SetBool("Lateralmente", true);

            if (_animaciones.Sprite != null && _animaciones.FlipearEnIzquierda && !_animaciones.FlipearEnDerecha)
                _animaciones.Sprite.flipX = true;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_tieneParametro_b(_animaciones._animator, "Lateralmente"))
                _animaciones._animator.SetBool("Lateralmente", false);

            if (_animaciones.Sprite != null && _animaciones.FlipearEnIzquierda && !_animaciones.FlipearEnDerecha)
                _animaciones.Sprite.flipX = false;
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

            if (_animaciones.f_tieneParametro_b(_animaciones._animator, "Lateralmente"))
                _animaciones._animator.SetBool("Lateralmente", true);

            if (_animaciones.f_tieneParametro_b(_animaciones._animator, "Derecha"))
                _animaciones._animator.SetBool("Derecha", true);


            if (_animaciones.Sprite != null && _animaciones.FlipearEnIzquierda && _animaciones.FlipearEnDerecha)
                _animaciones.Sprite.flipX = true;
        }
        public override void Salir()
        {
            if (_animaciones._animator == null)
                return;

            if (_animaciones.f_tieneParametro_b(_animaciones._animator, "Lateralmente"))
                _animaciones._animator.SetBool("Lateralmente", false);

            if (_animaciones.f_tieneParametro_b(_animaciones._animator, "Derecha"))
                _animaciones._animator.SetBool("Derecha", false);


            if (_animaciones.Sprite != null && _animaciones.FlipearEnIzquierda && _animaciones.FlipearEnDerecha)
                _animaciones.Sprite.flipX = false;

        }
    }




    // ***********************( Gizmos )*********************** //
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (_animator != null)
        {
            #if UNITY_EDITOR
                string parametrosDisponibles = "Parámetros disponibles: ";
                foreach (AnimatorControllerParameter param in _animator.parameters)
                {
                    parametrosDisponibles += param.name + ", ";
                }

                Handles.Label(transform.position + Vector3.up * 0.6f,
                                $"ANIMACIONES : Estado -> {_movimiento.Direcion} | {parametrosDisponibles} | PosicionInicial -> {_posicionInicial}");
            #endif
        }

    }
}
