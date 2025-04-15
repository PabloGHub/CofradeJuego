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

            //if (Transiciones.Count > 0)
            //    ActualizarTransiciones();
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

            //if (SubTransiciones.Count > 0)
            //    ActualizarTransiciones();
        }
    }

    public Dictionary<Func<bool>, EstadoBase> Transiciones;
    public Dictionary<Func<bool>, EstadoBase> SubTransiciones;
    public List<EstadoBase> estadosPosibles { get; set; }
    public List<EstadoBase> subEstadosPosibles { get; set; }
    public GameObject _go;


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



    public void CambiarEstado(int nuevoEstado)
    {
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
    // TODO: Descubrir porque no funciona.
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
        Debug.Log($"Transición agregada: {estadosPosibles[estadoDestino].GetType().Name}");
    }
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
        Debug.Log($"SubTransición agregada: {subEstadosPosibles[estadoDestino].GetType().Name}");
    }



    public void ActualizarTransiciones()
    {
        if (Transiciones.Count > 0)
            ActualizarTrnas();

        if (SubTransiciones.Count > 0)
            ActualizarSubTrnas();
    }
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
    public T CrearEstado<T, D>(D dependencia) where T : EstadoBase where D : class
    {
        var estado = _go.AddComponent<T>();
        estado.MaquinaEstados = this;
        estado.enabled = false;
        //estado.MiIndex = ObtenerIndice(estado); // No funciona!
        //Debug.Log($"Estado creado: {estado.GetType().Name}, Index: {estado.MiIndex}");
        estado.Init(dependencia);
        return estado;
    }
}