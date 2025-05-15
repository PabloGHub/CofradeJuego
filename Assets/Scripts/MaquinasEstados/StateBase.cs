using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

namespace SuiMachine
{
    public abstract class StateBase : MonoBehaviour
    {
        // ***********************( Variables/Declaraciones )*********************** //
        public MachineState MachineState { get; set; }
        public object Source { get; set; } // Clase padre donde se se instancio la Maquina de Estados.

        public int MiIndex { get; set; }
        public Component ThisComponent { get; set; }

        private Dictionary<Type, Action> _entrarDesde { get; set; } = new();
        private Dictionary<Type, Action> _salirDesde { get; set; } = new();


        // --- Control
        private bool _primeraVez_bandera = true;
        public bool InFirstEnter
        {
            get
            {
                _primeraVez_bandera = false;
                return _primeraVez_bandera;
            }
            set
            {
                Debug.LogWarning($"(StateBase): Se esta forzando a cambiar 'InFirstEnter' a {value}, no recomendable.");
                _primeraVez_bandera = value;
            }
        }


        // ***********************( Getters y Setters )*********************** //
        /// <summary>
        /// -----------------------Español<br />
        /// Solo se llamara a la funcion cuando el estado anterior es igual al valor.<br />
        /// -Si el estado anterior a este es T.<br />
        /// -La funcion con la clabe a T se ejecutara.
        /// <br />-----------------------<br />
        /// Nota: Solo puedes tener una funcion por estado.
        /// <br />-----------------------English<br />
        /// Only the function will be called when the previous state is equal to the value.<br />
        /// -If the previous state to this is T.<br />
        /// -The function with the key to T will be executed.
        /// <br />-----------------------<br />
        /// Note: You can only have one function per state.
        /// </summary>
        public void OnEnterFrom<T>(Action _fun) where T : StateBase
        {
            _entrarDesde[typeof(T)] = _fun;
        }
        public void OnEnterFrom(Type _tipo, Action _fun)
        {
            _entrarDesde[_tipo] = _fun;
        }


        /// <summary>
        /// Solo se llamara a la funcion cuando el estado siguiente es igual al valor.
        /// <br />-----------------------<br />
        /// -Si el siguiente estado a este es T.<br />
        /// -La funcion con la clabe a T se ejecutara.
        /// <br />-----------------------
        /// </summary>
        public void OnExitTo<T>(Action _fun) where T : StateBase
        {
            _salirDesde[typeof(T)] = _fun;
        }
        public void OnExitTo(Type _tipo, Action _fun)
        {
            _salirDesde[_tipo] = _fun;
        }


        // ***********************( Metodos de Control )*********************** //
        public abstract void Enter();
        public abstract void Exit();

        public virtual StateBase Transition()
        {
            return null;
        }
        public virtual int TransitionIndex()
        {
            return -1;
        }


        public virtual Task EnterAsync()
        {
            Enter();
            return Task.CompletedTask;
        }
        public virtual Task ExitAsync()
        {
            Exit();
            return Task.CompletedTask;
        }


        // ***********************( Mi Unity )*********************** //
        public virtual void MiAwake() { }
        public virtual void MiOnEnable() { }
        public virtual void MiStart() { }
        public virtual void MiFixedUpdate() { }
        public virtual void MiUpdate() { }
        public virtual void MiLateUpdate() { }
        public virtual void MiOnDisable() { }
        public virtual void MiOnDestroy() { }


        // ***********************( Eventos )*********************** //
        public event Action OnFirtsEnter;


        // ***********************( Unity -> Mi )*********************** //
        private void Awake()
        {
            // Deberia funcionar pero hay que testealo pues tengo malas experiencias.
            this.ThisComponent = this.GetComponent(this.GetType());
            MiIndex = this.MachineState.ObtenerIndice(this);

            _entrarDesde = new Dictionary<Type, Action>();
            _salirDesde = new Dictionary<Type, Action>();


            // --- Atributos
            var _metodos = GetType().GetMethods(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            foreach (var _metodo in _metodos)
            {
                foreach (var _atributo in _metodo.GetCustomAttributes(true))
                {
                    if (_atributo is OnEnterFromAttribute _entrada)
                    {
                        Action _fun = (Action)Delegate.CreateDelegate(typeof(Action), this, _metodo);
                        OnEnterFrom(_entrada.Type, _fun);
                    }
                    else if (_atributo is OnExitToAttribute _salida)
                    {
                        Action _fun = (Action)Delegate.CreateDelegate(typeof(Action), this, _metodo);
                        OnExitTo(_salida.Type, _fun);
                    }
                }
            }


            MiAwake();
        }
        private void OnEnable()
        {
            if (InFirstEnter)
                OnFirtsEnter?.Invoke();

            ThisComponent = GetComponent(GetType());
            MiIndex = MachineState.ObtenerIndice(this);

            StateBase _estado = Transition();
            if (_estado != null)
            {
                MachineState.CambiarEstado(MachineState[_estado]);
            }
            else
            {
                int _indice_i = TransitionIndex();
                if (_indice_i >= 0)
                {
                    MachineState.CambiarEstado(_indice_i);
                }
            }

            MiOnEnable();
        }
        private void Start()
        {
            MiStart();
        }
        private void FixedUpdate()
        {
            this.MachineState.ActualizarTransiciones();

            MiFixedUpdate();
        }
        private void Update()
        {
            MiUpdate();
        }
        private void LateUpdate()
        {
            MiLateUpdate();
        }
        private void OnDisable()
        {
            MiOnDisable();
        }
        private void OnDestroy()
        {
            MiOnDestroy();
        }


        // ***********************( Metodos Funcionales )*********************** //
        internal bool f_CambioEnter_b<T>(T _estado_T)
        {
            if (_estado_T == null)
            {
                Debug.LogError($"(StateBase): El estado pasado es nulo.");
                return false;
            }

            if (_entrarDesde.Count() <= 0)
                return false;

            foreach (var item in _entrarDesde)
            {
                if (item.Key.GetType() == _estado_T.GetType())
                {
                    item.Value?.Invoke();
                    return true;
                }
            }

            return false;
        }

        internal bool f_CambioExit_b<T>(T _estado_T)
        {
            if (_estado_T == null)
            {
                Debug.LogError($"(StateBase): El estado pasado es nulo.");
                return false;
            }

            if (_salirDesde.Count() <= 0)
                return false;

            foreach (var item in _salirDesde)
            {
                if (item.Key.GetType() == _estado_T.GetType())
                {
                    item.Value?.Invoke();
                    return true;
                }
            }

            return false;
        }


        // ***********************( Contructores )*********************** //
        public abstract void Init<T>(T dependencia);
    }


    // ***********************( Atributos )*********************** //
    // En cuanto Unity Admita C# 11 Pasar a valores genericos.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OnEnterFromAttribute : Attribute
    {
        public Type Type { get; }
        public OnEnterFromAttribute(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OnExitToAttribute : Attribute
    {
        public Type Type { get; }
        public OnExitToAttribute(Type type)
        {
            Type = type;
        }
    }

}

/* // EJEMPLO DE COPILOT //
 public abstract class EstadoBase
{
    public abstract void Entrar();
    public abstract void Actualizar();
    public abstract void Salir();
}

public class EstadoCaminar : EstadoBase
{
    public override void Entrar() { Debug.Log("Entrando en estado Caminar"); }
    public override void Actualizar() { Debug.Log("Actualizando estado Caminar"); }
    public override void Salir() { Debug.Log("Saliendo de estado Caminar"); }
}

public class ControladorNazareno : MonoBehaviour
{
    private EstadoBase estadoActual;

    public void CambiarEstado(EstadoBase nuevoEstado)
    {
        if (estadoActual != null)
            estadoActual.Salir();

        estadoActual = nuevoEstado;
        estadoActual.Entrar();
    }

    private void Update()
    {
        if (estadoActual != null)
            estadoActual.Actualizar();
    }
}
 */