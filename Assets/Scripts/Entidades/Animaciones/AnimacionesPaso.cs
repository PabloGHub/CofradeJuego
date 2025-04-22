using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Animations;
#endif

public class AnimacionesPaso : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
    // ----( Componentes )---- //
    private Animator _animator;
    private Movimiento _movimiento;
    [HideInInspector] public SpriteRenderer Sprite;
    private Vector3 _posicionInicial;

    // --- Animaciones --- //
    [Header("****( ¿Tiene 8 direciones? )****")]
    [SerializeField] private bool _8direcciones_b = false;
    private float _angulo_f = 0f;

    // ----( Maquina de estados )---- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }

    // ***********************( Metodos UNITY )*********************** //
    private void Awake()
    {
        //Maquina de Estados
        Inicializar(gameObject);
        estadosPosibles = new List<EstadoBase>
        {
            CrearEstado<EstadoNorte, AnimacionesPaso>(this),
            CrearEstado<EstadoSur, AnimacionesPaso>(this),
            CrearEstado<EstadoOeste, AnimacionesPaso>(this),
            CrearEstado<EstadoEste, AnimacionesPaso>(this),

            CrearEstado<EstadoNorOeste, AnimacionesPaso>(this),
            CrearEstado<EstadoNorEste, AnimacionesPaso>(this),
            CrearEstado<EstadoSurOeste, AnimacionesPaso>(this),
            CrearEstado<EstadoSurEste, AnimacionesPaso>(this),

            CrearEstado<EstadoGolpeado, AnimacionesPaso>(this)
        };

        // Posicion Inicial
        _posicionInicial = transform.localPosition;
    }

    private void OnEnable()
    {
        obtenerComponentes();
        CambiarEstado(1);
    }
    private void Start()
    {
        obtenerComponentes();

        if (_animator == null)
            return;

        AgregarTransicion(() => ControladorPPAL.V_pausado_b, 1);
        //AgregarTransicion(() => /*TODO: terminar.*/, 8);
        if (_8direcciones_b)
        {
            AgregarTransicion(() => (_angulo_f > -22.5f && _angulo_f <= 22.5f), 0);
            AgregarTransicion(() => (_angulo_f > 22.5f && _angulo_f <= 67.5f), 1);
            AgregarTransicion(() => (_angulo_f > 67.5f && _angulo_f <= 112.5), 2);
            AgregarTransicion(() => (_angulo_f > 112.5f && _angulo_f <= 157.5f), 3);
            AgregarTransicion(() => (_angulo_f > 157.5f || _angulo_f <= -157.5f), 4);
            AgregarTransicion(() => (_angulo_f > -157.5f && _angulo_f <= -112.5f), 5);
            AgregarTransicion(() => (_angulo_f > -112.5f && _angulo_f <= -67.5f), 6);
            AgregarTransicion(() => (_angulo_f > -67.5f && _angulo_f <= -22.5f), 7);
        }
        else
        {
            AgregarTransicion(() => (_angulo_f > -45f && _angulo_f <= 45f), 0);
            AgregarTransicion(() => (_angulo_f > 45f && _angulo_f <= 135f), 1);
            AgregarTransicion(() => (_angulo_f > 135f || _angulo_f <= -135f), 2);
            AgregarTransicion(() => (_angulo_f > -135f && _angulo_f <= -45f), 3);
        }
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

        _angulo_f = _movimiento.Angulo_f;
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





    // Nombres de los estados del animator.
    // Norte = N, NorEste = NE, Este = E, SurEste = SE, Sur = S, SurOeste = SO, Oeste = O, NorOeste = NO
    // ***********************( ESTADOS DE LA MAQUINA DE ESTADOS )*********************** //
    class EstadoNorte : EstadoBase
    {
        AnimacionesPaso _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesPaso;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.Play("N", 0, 0f);
        }
        public override void Salir()
        { }
    }
    class EstadoSur : EstadoBase
    {
        AnimacionesPaso _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesPaso;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.Play("S", 0, 0f);
        }
        public override void Salir()
        { }
    }
    class EstadoOeste : EstadoBase
    {
        AnimacionesPaso _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesPaso;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.Play("O", 0, 0f);
        }
        public override void Salir()
        { }
    }
    class EstadoEste : EstadoBase
    {
        AnimacionesPaso _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesPaso;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.Play("E", 0, 0f);
        }
        public override void Salir()
        { }
    }

    class EstadoNorOeste : EstadoBase
    {
        AnimacionesPaso _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesPaso;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.Play("NO", 0, 0f);
        }
        public override void Salir()
        { }
    }
    class EstadoNorEste : EstadoBase
    {
        AnimacionesPaso _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesPaso;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.Play("NE", 0, 0f);
        }
        public override void Salir()
        { }
    }
    class EstadoSurOeste : EstadoBase
    {
        AnimacionesPaso _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesPaso;
        }

        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.Play("SO", 0, 0f);
        }
        public override void Salir()
        { }
    }
    class EstadoSurEste : EstadoBase
    {
        AnimacionesPaso _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesPaso;
        }
        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;

            _animaciones._animator.Play("SE", 0, 0f);
        }
        public override void Salir()
        { }
    }

    class EstadoGolpeado : EstadoBase
    {
        AnimacionesPaso _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as AnimacionesPaso;
        }
        public override void Entrar()
        {
            if (_animaciones._animator == null)
                return;
        }
        public override void Salir()
        { }
    }

    // ***********************( Herramietas para Unity )*********************** //

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





/*
switch (_movimiento.Direcion)
{
    case Movimiento.Direcion_e.ARRIBA:

        if (_animator == null)
            return;

        if (_8direcciones_b)
        {

        }
        else
        {
            CambiarEstado(0);
        }

    break;


    case Movimiento.Direcion_e.DERECHA:

        if (_animator == null)
            return;

        if (_8direcciones_b)
        {

        }
        else
        {
            CambiarEstado(3);
        }

    break;


    case Movimiento.Direcion_e.IZQUIERDA:

        if (_animator == null)
            return;

        if (_8direcciones_b)
        {

        }
        else
        {
            CambiarEstado(2);
        }

    break;


    case Movimiento.Direcion_e.ABAJO: 

        if (_animator == null)
            return;

        if (_8direcciones_b)
        {

        }
        else
        {
            CambiarEstado(1);
        }

    break;

    case Movimiento.Direcion_e.NULO:
        Debug.LogWarning($"****** Entidad: {gameObject.name} NO tiene Direccion (NULO) ******");
    break;
}
*/