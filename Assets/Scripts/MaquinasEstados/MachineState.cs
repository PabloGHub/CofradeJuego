using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;

namespace SuiMachine
{
    // TODO: Esta creado lo nombres, ahora toca Implentarlo en los demas sistemas. // NOTA: que nombres? no me acuerdo, me falta contexto XD
    // TODO: Agregar un sistema de serializacion para guardar el estado actual de la maquina.
    // TODO: Poder borrar estados de la lista de estados posibles.
    // TODO: Terminar el sistema de transiciones (EnterOnState, ExitOnState).

    /// <summary>
    /// --------------------------------------------------------------
    /// <br />
    /// Maquina de Estados  
    /// <br />
    /// --------------------------------------------------------------
    /// </summary>
    public class MachineState /* <O> */ /* : MonoBehaviour*/
    {
        // ***********************( Variables/Declaraciones )*********************** //
        private StateBase _estadoActual { get; set; } // Representa el estado actual de la maquina
        public List<StateBase> _estadosPosibles { get; set; }
        private Dictionary<string, int> _nombresEstados = new Dictionary<string, int>();
        private List<StateBase> _estadosPersistentes = new List<StateBase>();

        private Dictionary<Func<bool>, StateBase> _transiciones;
        private GameObject _go;


        // ***********************( Getters y Setters )*********************** //
        public StateBase State
        {
            get { return _estadoActual; }
            set
            {
                if (_estadoActual == value)
                    return;

                if (_estadoActual != null)
                {
                    // Si no detecta ninguna funcion de salida, se ejecuta la salida normal. 
                    if (!_estadoActual.f_CambioExit_b<StateBase>(value))
                        _estadoActual.Exit();

                    _estadoActual.enabled = false;
                }

                var _estadoAnterior = value;
                _estadoActual = value;
                _estadoActual.MachineState = this;

                _estadoActual.enabled = true;
                OnEstadoCambiado?.Invoke(_estadoActual);


                // Si no detecta ninguna funcion de entrada, se ejecuta la entrada normal.
                if (!_estadoActual.f_CambioEnter_b<StateBase>(_estadoAnterior))
                    _estadoActual.Enter();

                ActualizarTransiciones();
            }
        }


        /// <summary>
        /// Devuelve el estado en la posicion del indice.<br />
        /// Guarda el estado en la posicion del indice.<br />
        /// </summary>
        /// <param name="_indice_i"></param>
        /// <returns></returns>
        public StateBase this[int _indice_i]
        {
            get
            {
                if (_indice_i < 0 || _indice_i >= _estadosPosibles.Count)
                {
                    Debug.LogError("(MachineState): El índice de estado es inválido.");
                    return null;
                }
                return _estadosPosibles[_indice_i];
            }
            set
            {
                if (_indice_i < 0)
                {
                    Debug.LogError("(MachineState): El índice de estado es inválido.");
                    return;
                }
                _estadosPosibles[_indice_i] = value;
            }
        }

        /// <summary>
        /// Devuelve el indice del estado.<br />
        /// Guarda el estado en la posicion del indice.<br />
        /// </summary>
        /// <param name="_estado"></param>
        /// <returns></returns>
        public int this[StateBase _estado]
        {
            get
            {
                if (_estado == null)
                {
                    Debug.LogError("(MachineState): El estado proporcionado es null.");
                    return -1;
                }
                return ObtenerIndice(_estado);
            }
            set
            {
                if (_estado == null)
                {
                    Debug.LogError("(MachineState): El estado proporcionado es null.");
                    return;
                }
                _estadosPosibles[value] = _estado;
            }
        }


        public int IndexState
        {
            get { return ObtenerIndice(State); }
        }


        public int Count
        {
            get { return _estadosPosibles.Count; }
        }


        // ***********************( Eventos )*********************** //
        public event Action<StateBase> OnEstadoCambiado;



        // ***********************( Metodos Estados )*********************** //
        /// <summary>
        /// Cambia el estado actual de la máquina de estados.
        /// </summary>
        /// <param name="nuevoEstado">Posicion en int del 'estadosPosibles'</param>
        public StateBase CambiarEstado(int nuevoEstado)
        {
            if (_estadosPosibles == null)
                return null;

            if (nuevoEstado > _estadosPosibles.Count)
            {
                Debug.LogError("*- El nuevoEstado es mayor a la cantidad de estadosPosibles -*");
                return null;
            }

            StateBase _posibleNovoEstado = _estadosPosibles[nuevoEstado];
            if (_posibleNovoEstado == null)
            {
                Debug.LogError("*- Intento de cambiar estado pasando un 'int' nulo -*");
                return null;
            }

            if (State == _posibleNovoEstado)
                return null;

            State = _posibleNovoEstado;
            return State;
        }

        // ---( Asincronos )--- //
        public async Task CambiarEstadoAsync(StateBase nuevoEstado)
        {
            if (State != null)
            {
                await State.ExitAsync();
                State.enabled = false;
            }

            State = nuevoEstado;
            State.MachineState = this;
            State.enabled = true;
            await State.EnterAsync();
        }

        // ---( Persistentes )--- //
        public void AgregarEstadoPersistente(StateBase estado)
        {
            if (!_estadosPersistentes.Contains(estado))
            {
                _estadosPersistentes.Add(estado);
                estado.MachineState = this;
                estado.enabled = true;
                estado.Enter();
            }
        }

        public void RemoverEstadoPersistente(StateBase estado)
        {
            if (_estadosPersistentes.Contains(estado))
            {
                estado.Exit();
                estado.enabled = false;
                _estadosPersistentes.Remove(estado);
            }
        }


        // ***********************( Metodos Transiciones )*********************** //
        /// <summary>
        /// Agrega una transición a la máquina de estados.  
        /// Una transicion es una condición que, al cumplirse, cambia el estado actual de la máquina.
        /// </summary>
        /// <param name="condicion">Es la condicion en lamda para cambiar '() => _parar_b == true'</param>
        /// <param name="estadoDestino">Estado al que cambiara pasando el int de la posicion de 'estadosPosibles'</param>
        public void AgregarTransicion(Func<bool> condicion, int estadoDestino)
        {
            if (_transiciones == null)
                _transiciones = new Dictionary<Func<bool>, StateBase>();

            if (estadoDestino < 0 || estadoDestino >= _estadosPosibles.Count)
            {
                Debug.LogError("(MachineState): El índice de estado destino es inválido en AgregarTransicion.");
                return;
            }

            _transiciones[condicion] = _estadosPosibles[estadoDestino];
            //Debug.Log($"Transición agregada: {estadosPosibles[estadoDestino].GetType().Name}");
        }


        /// <summary>
        /// Actualiza las transiciones de la máquina de estados.
        /// </summary>
        public void ActualizarTransiciones()
        {
            if (_transiciones == null)
                return;

            foreach (var transicion in _transiciones)
            {
                if (transicion.Key.Invoke())
                {
                    CambiarEstado(ObtenerIndice(transicion.Value));
                    break;
                }
            }
        }

        // ***********************( Indices )*********************** //
        /// <summary>
        /// Obtiene el índice del estado o subEstado en la lista de estados posibles.
        /// </summary>
        /// <param name="estado">Estado del que se quiere sacar el indice</param>
        /// <returns>Retona un int del indice</returns>
        public int ObtenerIndice(StateBase estado)
        {
            if (estado == null)
            {
                Debug.LogError("(MachineState): El estado proporcionado es null.");
                return -1;
            }

            int indice = _estadosPosibles.IndexOf(estado);
            if (indice == -1)
            {
                Debug.LogWarning($"(MachineState): El estado {estado.GetType().Name} no se encuentra en la lista de estados posibles.");
            }

            return indice;
        }



        // ***********************( Serealizacion )*********************** //
        [Serializable]
        public class EstadoSerializable
        {
            public int IndiceEstadoActual;
            private Dictionary<Func<bool>, StateBase> _transiciones;
            private GameObject _go;

            public List<StateBase> EstadosPosibles { get; set; }

            private List<StateBase> _estadosPersistentes = new List<StateBase>();
            // Agregar más datos según sea necesario
        }

        public EstadoSerializable Serializar()
        {
            return new EstadoSerializable
            {
                IndiceEstadoActual = IndexState,
            };
        }

        public void Deserializar(EstadoSerializable datos)
        {
            CambiarEstado(datos.IndiceEstadoActual);
        }



        // ***********************( Funciones Constructores )*********************** //
        /// <summary>
        /// Crea un nuevo estado de tipo T y lo inicializa con la dependencia proporcionada.
        /// Añade al objeto actual el componente del estado.
        /// Guarda la maquina de estado el PADRE o this.
        /// LLama al metodo Init() del estado.
        /// </summary>
        /// <typeparam name="T">Estado que se quiera craer</typeparam>
        /// <typeparam name="D">Clase de la que se quiera pasar la dependencia</typeparam>
        /// <param name="dependencia">Dependencia que se quiera pasar al nuevo Estado</param>
        /// <returns>Retonar el nuevo Estado agregado y desactivado</returns>
        public T CrearEstado<T, D>(D dependencia) where T : StateBase where D : class
        {
            var estado = _go.AddComponent<T>();
            estado.enabled = false;
            estado.MachineState = this;
            estado.Source = dependencia; // ORIGINAL
            estado.Init(dependencia);

            if (!_estadosPosibles.Contains(estado))
                _estadosPosibles.Add(estado);

            string _nombreEstado_s = estado.GetType().Name;
            if (!_nombresEstados.ContainsKey(_nombreEstado_s))
                _nombresEstados.Add(_nombreEstado_s, ObtenerIndice(estado));

            return estado;
        }

        // ***********************( Constructores )*********************** //
        public MachineState(GameObject goHost, List<StateBase> estadosPosibles)
        {
            if (estadosPosibles == null)
                estadosPosibles = new List<StateBase>();

            this._estadosPosibles = estadosPosibles;

            inicializar(goHost);
        }
        public MachineState(GameObject goHost)
        {
            this.inicializar(goHost);
        }


        /// <summary>
        /// Inicializa la máquina de estados con el GameObject host.
        /// </summary>
        /// <param name="goHost">gameObject necesario para la maquina de estado</param>
        private void inicializar(GameObject goHost)
        {
            if (goHost == null)
            {
                Debug.LogError("(MachineState): El GameObject host es null en Inicializar.");
                return;
            }

            if (_transiciones == null)
                _transiciones = new Dictionary<Func<bool>, StateBase>();

            if (_estadosPosibles == null)
                _estadosPosibles = new List<StateBase>();

            _go = goHost;
        }
    }
}