using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class MaquinaDeEstados : MonoBehaviour
{
    // ***********************( Variables/Declaraciones )*********************** //
    public abstract EstadoBase Estado { get; set; } // Representa el estado actual de la maquina
    public abstract EstadoBase SubEstado { get; set; } // Representa el subEstado actual de la maquina

   
    public EstadoBase EstadoActual
    {
        get { return Estado; }
        set
        {
            if (Estado == value)
                return;

            if (Estado != null)
            {
                Estado.Salir();
                Estado.enabled = false;
            }

            Estado = value;
            Estado.MaquinaEstados = this;

            OnEstadoCambiado?.Invoke(Estado);

            Estado.enabled = true;
            Estado.Entrar();


            ActualizarTransiciones();
        }
    }

    public EstadoBase SubEstadoActual
    {
        get { return SubEstado; }
        set
        {
            if (Estado == value)
                return;

            if (SubEstado != null)
            {
                SubEstado.Salir();
                SubEstado.enabled = false;
            }

            SubEstado = value;
            SubEstado.MaquinaEstados = this;

            OnSubEstadoCambiado?.Invoke(SubEstado);

            SubEstado.enabled = true;
            SubEstado.Entrar();


            ActualizarTransiciones();
        }
    }

    [HideInInspector] public Dictionary<Func<bool>, EstadoBase> Transiciones;
    [HideInInspector] public Dictionary<Func<bool>, EstadoBase> SubTransiciones;
    [HideInInspector] public List<EstadoBase> estadosPosibles { get; set; }
    [HideInInspector] public List<EstadoBase> subEstadosPosibles { get; set; }
    [HideInInspector] public GameObject _go;


    // ***********************( Eventos )*********************** //
    public event Action<EstadoBase> OnEstadoCambiado;
    public event Action<EstadoBase> OnSubEstadoCambiado;


    // ***********************( Unity )*********************** //
    private void Awake()
    {
        if (Transiciones == null)
            Transiciones = new Dictionary<Func<bool>, EstadoBase>();

        if (SubTransiciones == null)
            SubTransiciones = new Dictionary<Func<bool>, EstadoBase>();

        if (estadosPosibles == null)
            estadosPosibles = new List<EstadoBase>();

        if (subEstadosPosibles == null)
            subEstadosPosibles = new List<EstadoBase>();
    }
    private void LateUpdate()
    {
        ActualizarTransiciones();
    }

    // ***********************( Metodos Funcionales )*********************** //
    public void Inicializar(GameObject goHost, List<EstadoBase> estadosPosibles)
    {
        if (estadosPosibles == null)
            estadosPosibles = new List<EstadoBase>();

        this.estadosPosibles = estadosPosibles;

        Inicializar(goHost);
    }
    /// <summary>
    /// Inicializa la máquina de estados con el GameObject host.
    /// </summary>
    /// <param name="goHost">gameObject necesario para la maquina de estado</param>
    public void Inicializar(GameObject goHost)
    {
        if (goHost == null)
        {
            Debug.LogError("El GameObject host es null en Inicializar.");
            return;
        }

        if (Transiciones == null)
            Transiciones = new Dictionary<Func<bool>, EstadoBase>();

        if (SubTransiciones == null)
            SubTransiciones = new Dictionary<Func<bool>, EstadoBase>();

        if (estadosPosibles == null)
            estadosPosibles = new List<EstadoBase>();

        if (subEstadosPosibles == null)
            subEstadosPosibles = new List<EstadoBase>();

        _go = goHost;
    }


    /// <summary>
    /// Cambia el estado actual de la máquina de estados.
    /// </summary>
    /// <param name="nuevoEstado">Posicion en int del 'estadosPosibles'</param>
    public void CambiarEstado(int nuevoEstado)
    {
        // TODO: comprobar si el int es mayor al tamaño de la lista.
        /*
         A veces pasa:
            ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
            Parameter name: index
         */
        EstadoBase _posibleNovoEstado = estadosPosibles[nuevoEstado];
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
        EstadoBase _posibleNovoEstado = subEstadosPosibles[nuevoEstado];
        if (_posibleNovoEstado == null)
        {
            Debug.LogError("*- Intento de cambiar subEstado pasando un 'int' nulo -*");
            return;
        }

        if (SubEstadoActual == _posibleNovoEstado)
            return;

        SubEstadoActual = _posibleNovoEstado;
    }


    // TODO: Agregar Restringciones.
    /// <summary>
    /// Agrega una transición a la máquina de estados.  
    /// Una transicion es una condición que, al cumplirse, cambia el estado actual de la máquina.
    /// </summary>
    /// <param name="condicion">Es la condicion en lamda para cambiar '() => _parar_b == true'</param>
    /// <param name="estadoDestino">Estado al que cambiara pasando el int de la posicion de 'estadosPosibles'</param>
    public void AgregarTransicion(Func<bool> condicion, int estadoDestino)
    {
        if (Transiciones == null)
            Transiciones = new Dictionary<Func<bool>, EstadoBase>();

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
            SubTransiciones = new Dictionary<Func<bool>, EstadoBase>();

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
    public int ObtenerIndice(EstadoBase estado)
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
    public int ObtenerIndiceEstado(EstadoBase estado)
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
    public int ObtenerIndiceSubEstado(EstadoBase subEstado)
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
    public T CrearEstado<T, D>(D dependencia) where T : EstadoBase where D : class
    {
        var estado = _go.AddComponent<T>();
        estado.MaquinaEstados = this;
        estado.enabled = false;
        //estado.MiIndex = ObtenerIndice(estado); // No funciona!
        estado.Init(dependencia);
        return estado;
    }
}