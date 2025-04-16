using System.Collections.Generic;
using UnityEngine;

public class Animaciones : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
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
        // Animator
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("El Nazareno no tiene un componente Animator.");
            return;
        }

        // Maquina de Estados
        Inicializar(gameObject);
        estadosPosibles = new List<EstadoBase>
        {
            CrearEstado<EstadoArriba, Animaciones>(this),
            CrearEstado<EstadoAbajo, Animaciones>(this),
            CrearEstado<EstadoLateral, Animaciones>(this)
        };

        // Movimiento del padre
        _movimiento = GetComponentInParent<Movimiento>();

        // SpriteRenderer
        Sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;

        switch (_movimiento.Direcion)
        {
            case Movimiento.Direcion_e.ARRIBA:
                Sprite.flipX = false;
                CambiarEstado(0);
            break;


            case Movimiento.Direcion_e.DERECHA:
                Sprite.flipX = false;
                CambiarEstado(2);
            break;


            case Movimiento.Direcion_e.IZQUIERDA:
                CambiarEstado(2);
                Sprite.flipX = true;
            break;


            case Movimiento.Direcion_e.ABAJO:
                Sprite.flipX = false;
                CambiarEstado(1);
            break;

            case Movimiento.Direcion_e.NULO:
                Sprite.flipX = false;
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
            _animaciones._animator.SetBool("Subiendo", true);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", false);
        }
        public override void Salir()
        {
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
            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", true);
            _animaciones._animator.SetBool("Lateralmente", false);
        }
        public override void Salir()
        {
            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", false);
        }
    }
    class EstadoLateral : EstadoBase
    {
        Animaciones _animaciones;
        public override void Init<T>(T dependencia)
        {
            _animaciones = dependencia as Animaciones;
        }

        public override void Entrar()
        {
            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", true);
        }
        public override void Salir()
        {
            _animaciones._animator.SetBool("Subiendo", false);
            _animaciones._animator.SetBool("Bajando", false);
            _animaciones._animator.SetBool("Lateralmente", false);
        }
    }
}
