using System.Collections.Generic;
using System;
using UnityEngine;

// TODO: 
public abstract class MachineState /* : MonoBehaviour*/
{
    // ***********************( Variables/Declaraciones )*********************** //
    private StateBase _estado { get; set; } // Representa el estado actual de la maquina
    private StateBase _subEstado { get; set; } // Representa el subEstado actual de la maquina


    public StateBase EstadoActual
    {
        get { return _estado; }
        set
        {
            if (_estado == value)
                return;

            if (_estado != null)
            {
                _estado.Salir();
                _estado.enabled = false;
            }

            _estado = value;
            _estado.MaquinaEstados = this;

            OnEstadoCambiado?.Invoke(_estado);

            _estado.enabled = true;
            _estado.Entrar();


            ActualizarTransiciones();
        }
    }

    public StateBase SubEstadoActual
    {
        get { return _subEstado; }
        set
        {
            if (_estado == value)
                return;

            if (_subEstado != null)
            {
                _subEstado.Salir();
                _subEstado.enabled = false;
            }

            _subEstado = value;
            _subEstado.MaquinaEstados = this;

            OnSubEstadoCambiado?.Invoke(_subEstado);

            _subEstado.enabled = true;
            _subEstado.Entrar();


            ActualizarTransiciones();
        }
    }

    [HideInInspector] public Dictionary<Func<bool>, StateBase> Transiciones;
    [HideInInspector] public Dictionary<Func<bool>, StateBase> SubTransiciones;
    [HideInInspector] public List<StateBase> estadosPosibles { get; set; }
    [HideInInspector] public List<StateBase> subEstadosPosibles { get; set; }
    [HideInInspector] public GameObject _go;


    // ***********************( Eventos )*********************** //
    public event Action<StateBase> OnEstadoCambiado;
    public event Action<StateBase> OnSubEstadoCambiado;


    // ***********************( Metodos )*********************** //
    private void inicializar(GameObject goHost, List<StateBase> estadosPosibles)
    {
        if (estadosPosibles == null)
            estadosPosibles = new List<StateBase>();

        this.estadosPosibles = estadosPosibles;

        inicializar(goHost);
    }
    /// <summary>
    /// Inicializa la máquina de estados con el GameObject host.
    /// </summary>
    /// <param name="goHost">gameObject necesario para la maquina de estado</param>
    private void inicializar(GameObject goHost)
    {
        if (goHost == null)
        {
            Debug.LogError("El GameObject host es null en Inicializar.");
            return;
        }

        if (Transiciones == null)
            Transiciones = new Dictionary<Func<bool>, StateBase>();

        if (SubTransiciones == null)
            SubTransiciones = new Dictionary<Func<bool>, StateBase>();

        if (estadosPosibles == null)
            estadosPosibles = new List<StateBase>();

        if (subEstadosPosibles == null)
            subEstadosPosibles = new List<StateBase>();

        _go = goHost;
    }


    /// <summary>
    /// Cambia el estado actual de la máquina de estados.
    /// </summary>
    /// <param name="nuevoEstado">Posicion en int del 'estadosPosibles'</param>
    public void CambiarEstado(int nuevoEstado)
    {
        if (estadosPosibles == null)
            return;

        if (nuevoEstado > estadosPosibles.Count)
        {
            Debug.LogError("*- El nuevoEstado es mayor a la cantidad de estadosPosibles -*");
            return;
        }

        StateBase _posibleNovoEstado = estadosPosibles[nuevoEstado];
        if (_posibleNovoEstado == null)
        {
            Debug.LogError("*- Intento de cambiar estado pasando un 'int' nulo -*");
            return;
        }

        if (EstadoActual == _posibleNovoEstado)
            return;

        EstadoActual = _posibleNovoEstado;
    }
    /// <summary>
    /// Cambia el subEstado actual de la máquina de estados.
    /// </summary>
    /// <param name="nuevoEstado">Posicion en int del 'subEstadosPosibles'</param>
    public void CambiarSubEstado(int nuevoEstado)
    {
        StateBase _posibleNovoEstado = subEstadosPosibles[nuevoEstado];
        if (_posibleNovoEstado == null)
        {
            Debug.LogError("*- Intento de cambiar subEstado pasando un 'int' nulo -*");
            return;
        }

        if (SubEstadoActual == _posibleNovoEstado)
            return;

        SubEstadoActual = _posibleNovoEstado;
    }


    /// <summary>
    /// Agrega una transición a la máquina de estados.  
    /// Una transicion es una condición que, al cumplirse, cambia el estado actual de la máquina.
    /// </summary>
    /// <param name="condicion">Es la condicion en lamda para cambiar '() => _parar_b == true'</param>
    /// <param name="estadoDestino">Estado al que cambiara pasando el int de la posicion de 'estadosPosibles'</param>
    public void AgregarTransicion(Func<bool> condicion, int estadoDestino)
    {
        if (Transiciones == null)
            Transiciones = new Dictionary<Func<bool>, StateBase>();

        if (estadoDestino < 0 || estadoDestino >= estadosPosibles.Count)
        {
            Debug.LogError("El índice de estado destino es inválido en AgregarTransicion.");
            return;
        }

        Transiciones[condicion] = estadosPosibles[estadoDestino];
        //Debug.Log($"Transición agregada: {estadosPosibles[estadoDestino].GetType().Name}");
    }
    /// <summary>
    /// Agrega una transición a la máquina de estados.  
    /// Una transicion es una condición que, al cumplirse, cambia el estado actual de la máquina.
    /// </summary>
    /// <param name="condicion">Es la condicion en lamda para cambiar '() => _parar_b == true'</param>
    /// <param name="estadoDestino">Estado al que cambiara pasando el int de la posicion de 'subEstadosPosibles'</param>
    public void AgregarSubTransicion(Func<bool> condicion, int estadoDestino)
    {
        if (SubTransiciones == null)
            SubTransiciones = new Dictionary<Func<bool>, StateBase>();

        if (estadoDestino < 0 || estadoDestino >= estadosPosibles.Count)
        {
            Debug.LogError("El índice de estado destino es inválido en AgregarTransicion.");
            return;
        }

        SubTransiciones[condicion] = subEstadosPosibles[estadoDestino];
        //Debug.Log($"SubTransición agregada: {subEstadosPosibles[estadoDestino].GetType().Name}");
    }


    /// <summary>
    /// Actualiza TODAS las transiciones de la máquina de estados.
    /// llama a ActualizarTrnas() y ActualizarSubTrnas()
    /// </summary>
    public void ActualizarTransiciones()
    {
        if (Transiciones == null)
            return;

        if (Transiciones.Count > 0)
            ActualizarTrnas();

        if (SubTransiciones.Count > 0)
            ActualizarSubTrnas();
    }
    /// <summary>
    /// Actualiza las transiciones de la máquina de estados.
    /// </summary>
    public void ActualizarTrnas()
    {
        foreach (var transicion in Transiciones)
        {
            if (transicion.Key.Invoke())
            {
                CambiarEstado(ObtenerIndiceEstado(transicion.Value));
                break;
            }
        }
    }
    /// <summary>
    /// Actualiza las subtransiciones de la máquina de estados. 
    /// </summary>
    public void ActualizarSubTrnas()
    {
        foreach (var transicion in SubTransiciones)
        {
            if (transicion.Key.Invoke())
            {
                CambiarEstado(ObtenerIndiceSubEstado(transicion.Value));
                break;
            }
        }
    }

    // ***********************( Mios )*********************** //
    /// <summary>
    /// Obtiene el índice del estado o subEstado en la lista de estados posibles.
    /// </summary>
    /// <param name="estado">Estado del que se quiere sacar el indice</param>
    /// <returns>Retona un int del indice</returns>
    public int ObtenerIndice(StateBase estado)
    {
        int indice = -1;
        if (estado == null)
        {
            Debug.LogError("El estado proporcionado es null.");
            return indice;
        }

        indice = ObtenerIndiceEstado(estado);
        if (indice == -1)
        {
            indice = ObtenerIndiceSubEstado(estado);
        }

        return indice;
    }

    /// <summary>
    /// Obtiene el índice del estado en la lista de estados posibles.
    /// </summary>
    /// <param name="estado">Estado del que se quiere sacar el indice</param>
    /// <returns>Retona un int del indice</returns>
    public int ObtenerIndiceEstado(StateBase estado)
    {
        if (estado == null)
        {
            Debug.LogError("El estado proporcionado es null.");
            return -1;
        }

        int indice = estadosPosibles.IndexOf(estado);
        if (indice == -1)
        {
            Debug.LogWarning($"El estado {estado.GetType().Name} no se encuentra en la lista de estados posibles.");
        }

        return indice;
    }
    /// <summary>
    /// Obtiene el índice del subEstado en la lista de subEstados posibles.
    /// </summary>
    /// <param name="subEstado">SubEstado del que se quiere sacar el indice</param>
    /// <returns>Retona un int del indice</returns>
    public int ObtenerIndiceSubEstado(StateBase subEstado)
    {
        if (subEstado == null)
        {
            Debug.LogError("El subEstado proporcionado es null.");
            return -1;
        }

        int indice = subEstadosPosibles.IndexOf(subEstado);
        if (indice == -1)
        {
            Debug.LogWarning($"El subEstado {subEstado.GetType().Name} no se encuentra en la lista de subEstados posibles.");
        }

        return indice;
    }

    // ***********************( Constructores )*********************** //
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
        estado.MaquinaEstados = this;
        estado.enabled = false;
        //estado.MiIndex = ObtenerIndice(estado); // ¡No funciona!
        estado.Init(dependencia);
        return estado;
    }


    public MachineState(GameObject goHost)
    {
        this.inicializar(goHost);
    }
    public MachineState(GameObject goHost, List<StateBase> estadosPosibles)
    {
        this.inicializar(goHost, estadosPosibles);
    }
}