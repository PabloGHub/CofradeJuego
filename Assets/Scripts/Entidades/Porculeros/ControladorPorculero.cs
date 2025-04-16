using UnityEngine;

public class ControladorPorculero : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
    // private float cercaniaAlObjetivo = 2.5f;
    // public Movimiento v_movimiento;
    // private Transform v_objetivo_Transform;


    // --- Maquina de Estados --- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }


    // ***********************( Metodos UNITY )*********************** //
    // ***********************( Metodos NUESTROS )*********************** //

    // ***********************( ESTADOS DE LA MAQUINA DE ESTADOS )*********************** //
    class EstadoParado : EstadoBase
    {
        private ControladorPorculero _controladorPorculero_s;
        public override void Init<T>(T dependencia)
        {
            _controladorPorculero_s = dependencia as ControladorPorculero;
        }

        public override void Entrar()
        {}
        public override void Salir()
        {}
    }
    class EstadoPersiguiendo : EstadoBase
    {
        private ControladorPorculero _controladorPorculero_s;
        public override void Init<T>(T dependencia)
        {
            _controladorPorculero_s = dependencia as ControladorPorculero;
        }

        public override void Entrar()
        { }
        public override void Salir()
        { }
    }
    class EstadoAtacando : EstadoBase
    {
        private ControladorPorculero _controladorPorculero_s;
        public override void Init<T>(T dependencia)
        {
            _controladorPorculero_s = dependencia as ControladorPorculero;
        }
        public override void Entrar()
        { }
        public override void Salir()
        { }
    }
}

