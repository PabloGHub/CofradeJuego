namespace ejemplo_pruebas
{
    using SuiMachine;
    using UnityEngine;

    public class Ejemplo : MonoBehaviour
    {
        private MachineState _maquina;
        private void Start()
        {
            _maquina = new MachineState(gameObject);
            _maquina.CrearEstado<Estado1, Ejemplo>(this);
            _maquina.CrearEstado<Estado2, Ejemplo>(this);
        }
    }

    public class Estado1 : StateBase
    {
        Ejemplo dependencia;
        public override void Init<T>(T dependencia)
        {
            this.dependencia = dependencia as Ejemplo;
        }

        public override void Enter()
        { }
        public override void Exit()
        {
            Debug.Log("(Estado1): Tecnicamente mientras solo haya estos 2 estados nunca deberia aparecer.");
        }

        public override void MiAwake()
        {
            OnExitTo<Estado2>(siSaleHacia2);
        }

        public void siSaleHacia2()
        {
            Debug.Log("Salio hacia el estado 2");
        }
    }

    public class Estado2 : StateBase
    {
        Ejemplo dependencia;
        public override void Init<T>(T dependencia)
        {
            this.dependencia = dependencia as Ejemplo;
        }

        public override void Enter()
        {
            Debug.Log("(Estado2): Tecnicamente mientras solo aya estos 2 estados nunca deberia aparecer.");
        }
        public override void Exit()
        { }


        public override void MiAwake()
        {
            //OnEnterFrom<Estado1>(siEntrarDesde1);
        }

        [OnEnterFrom(typeof(Estado1))]
        public void siEntrarDesde1()
        {
            Debug.Log("Entrando desde el estado 1");
        }
    }
}