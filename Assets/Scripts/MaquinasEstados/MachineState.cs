using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;



/// <summary>
/// -------------------------------------------------------------- <br />
/// Maquina de Estados  
/// 
/// <br /> --------------------------------------------------------------
/// </summary>
public class MachineState /* <O> */ /* : MonoBehaviour*/
{
    // ***********************( Variables/Declaraciones )*********************** //
    private StateBase _estado { get; set; } // Representa el estado actual de la maquina

    private Dictionary<Func<bool>, StateBase> _transiciones;
    private GameObject _go;

    public List<StateBase> EstadosPosibles { get; set; }

    private List<StateBase> _estadosPersistentes = new List<StateBase>();

    
    // ***********************( Getters y Setters )*********************** //
    public StateBase EstadoActual
    {
        get { return _estado; }
        set
        {
            if (_estado == value)
                return;

            if (_estado != null)
            {
                _estado.Exit();
                _estado.enabled = false;
            }

            _estado = value;
            _estado.MachineState = this;

            OnEstadoCambiado?.Invoke(_estado);

            _estado.enabled = true;
            _estado.Enter();


            ActualizarTransiciones();
        }
    }

    public StateBase this[int _indice_i]
    {
        get
        {
            if (_indice_i < 0 || _indice_i >= EstadosPosibles.Count)
            {
                Debug.LogError("(MachineState): El índice de estado es inválido.");
                return null;
            }
            return EstadosPosibles[_indice_i];
        }
        set
        {
            if (_indice_i < 0)
            {
                Debug.LogError("(MachineState): El índice de estado es inválido.");
                return;
            }
            EstadosPosibles[_indice_i] = value;
        }
    }
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
            EstadosPosibles[value] = _estado;
        }
    }


    public int IndexState
    {
        get { return ObtenerIndice(EstadoActual); }
    }


    public int Count
    {
        get { return EstadosPosibles.Count; }
    }

    // ***********************( Eventos )*********************** //
    public event Action<StateBase> OnEstadoCambiado;



    // ***********************( Metodos Estados )*********************** //
    /// <summary>
    /// Cambia el estado actual de la máquina de estados.
    /// </summary>
    /// <param name="nuevoEstado">Posicion en int del 'estadosPosibles'</param>
    public void CambiarEstado(int nuevoEstado)
    {
        if (EstadosPosibles == null)
            return;

        if (nuevoEstado > EstadosPosibles.Count)
        {
            Debug.LogError("*- El nuevoEstado es mayor a la cantidad de estadosPosibles -*");
            return;
        }

        StateBase _posibleNovoEstado = EstadosPosibles[nuevoEstado];
        if (_posibleNovoEstado == null)
        {
            Debug.LogError("*- Intento de cambiar estado pasando un 'int' nulo -*");
            return;
        }

        if (EstadoActual == _posibleNovoEstado)
            return;

        EstadoActual = _posibleNovoEstado;
    }

    // ---( Asincronos )--- //
    public async Task CambiarEstadoAsync(StateBase nuevoEstado)
    {
        if (EstadoActual != null)
        {
            await EstadoActual.ExitAsync();
            EstadoActual.enabled = false;
        }

        EstadoActual = nuevoEstado;
        EstadoActual.MachineState = this;
        EstadoActual.enabled = true;
        await EstadoActual.EnterAsync();
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

        if (estadoDestino < 0 || estadoDestino >= EstadosPosibles.Count)
        {
            Debug.LogError("(MachineState): El índice de estado destino es inválido en AgregarTransicion.");
            return;
        }

        _transiciones[condicion] = EstadosPosibles[estadoDestino];
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

        int indice = EstadosPosibles.IndexOf(estado);
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
    public /*static*/ T CrearEstado<T, D>(D dependencia) where T : StateBase where D : class
    {
        var estado = _go.AddComponent<T>();
        estado.enabled = false;
        estado.MachineState = this;
        estado.Source = dependencia; // ORIGINAL
        estado.Init(dependencia);
        return estado;
    }

    // ***********************( Constructores )*********************** //
    public MachineState(GameObject goHost, List<StateBase> estadosPosibles)
    {
        if (estadosPosibles == null)
            estadosPosibles = new List<StateBase>();

        this.EstadosPosibles = estadosPosibles;

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

        if (EstadosPosibles == null)
            EstadosPosibles = new List<StateBase>();

        _go = goHost;
    }
}