using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class MaquinaDeEstados : MonoBehaviour
{
    // ***********************( Variables/Declaraciones )*********************** //
    public abstract EstadoBase Estado { get; set; } // Representa el estado actual de la maquina
    public abstract EstadoBase SubEstado { get; set; } // Representa el subEstado actual de la maquina

    //private EstadoBase _estadoActual { get; set; }
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

            if (_transiciones.Count > 0)
                ActualizarTransiciones();
        }
    }

    private EstadoBase _subEstadoActual;
    public EstadoBase SubEstadoActual
    {
        get { return _subEstadoActual; }
        set { _subEstadoActual = value; }
    }

    public Dictionary<Func<bool>, EstadoBase> _transiciones;
    public List<EstadoBase> estadosPosibles { get; set; }
    public List<EstadoBase> subEstadosPosibles { get; set; }
    public GameObject _go;


    // ***********************( Eventos )*********************** //
    public event Action<EstadoBase> OnEstadoCambiado;
    //public event Action<EstadoBase> OnSubEstadoCambiado;


    // ***********************( Unity )*********************** //
    private void Awake()
    {
        if (_transiciones == null)
            _transiciones = new Dictionary<Func<bool>, EstadoBase>();

        if (estadosPosibles == null)
            estadosPosibles = new List<EstadoBase>();
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

        if (_transiciones == null)
            _transiciones = new Dictionary<Func<bool>, EstadoBase>();

        if (estadosPosibles == null)
            estadosPosibles = new List<EstadoBase>();

        _go = goHost;
    }

    public void CambiarEstado(EstadoBase nuevoEstado)
    {
        if (nuevoEstado == null)
            Debug.LogError("*- Intento de cambiar estado pasando un 'EstadoBase' nulo -*");

        EstadoActual = nuevoEstado;
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
    //public void CambiarSubEstado(EstadoBase nuevoSubEstado)
    //{
    //    if (_subEstadoActual != null)
    //        _subEstadoActual.Salir();

    //    _subEstadoActual = nuevoSubEstado;
    //    _subEstadoActual.Entrar();
    //}

    //public void AgregarTransicion(Func<bool> condicion, EstadoBase estadoDestino)
    //{
    //    if (_transiciones == null)
    //        _transiciones = new Dictionary<Func<bool>, EstadoBase>();

    //    if (estadoDestino == null)
    //    {
    //        Debug.LogError("El estado destino es null en AgregarTransicion.");
    //        return;
    //    }

    //    _transiciones[condicion] = estadoDestino;

    //    if (_transiciones.Count > 0)
    //        ActualizarTransiciones();
    //}
    public void AgregarTransicion(Func<bool> condicion, int estadoDestino)
    {
        if (_transiciones == null)
            _transiciones = new Dictionary<Func<bool>, EstadoBase>();

        if (estadoDestino < 0 || estadoDestino >= estadosPosibles.Count)
        {
            Debug.LogError("El índice de estado destino es inválido en AgregarTransicion.");
            return;
        }

        _transiciones[condicion] = estadosPosibles[estadoDestino];
        Debug.Log($"Transición agregada: {estadosPosibles[estadoDestino].GetType().Name}");

        if (_transiciones.Count > 0)
            ActualizarTransiciones();
    }
    public void ActualizarTransiciones()
    {
        foreach (var transicion in _transiciones)
        {
            Debug.Log("transicion: " + transicion.ToString());
            if (transicion.Key.Invoke())
            {
                CambiarEstado(transicion.Value);
                var estadoDestino = transicion.Value;
                break;
            }
        }
    }

    // ***********************( Unity )*********************** //
    //private void LateUpdate()
    //{
    //    if (_transiciones.Count > 0)
    //        ActualizarTransiciones();
    //}

    // ***********************( Constructores )*********************** //
    public T CrearEstado<T, D>(D dependencia) where T : EstadoBase where D : class
    {
        var estado = _go.AddComponent<T>();
        estado.MaquinaEstados = this;
        estado.enabled = false;
        estado.Init(dependencia);
        return estado;
    }
}