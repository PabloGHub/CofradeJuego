using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class EstadoBase : MonoBehaviour
{
    // ***********************( Variables/Declaraciones )*********************** //
    private EstadoBase estadoActual;
    public EstadoBase EstadoActual
    {
        get { return estadoActual; }
        private set
        {
            estadoActual = value;
            OnEstadoCambiado?.Invoke(estadoActual);
        }
    }

    private EstadoBase subEstadoActual;
    public EstadoBase SubEstadoActual
    {
        get { return subEstadoActual; }
        set { subEstadoActual = value; }
    }

    private Dictionary<Func<bool>, EstadoBase> transiciones = new Dictionary<Func<bool>, EstadoBase>();


    // ***********************( Eventos )*********************** //
    public event Action<EstadoBase> OnEstadoCambiado;
    public event Action OnEntrar;
    public event Action OnSalir;


    // ***********************( Metodos Abstractos )*********************** //
    public virtual void Entrar() { }
    public virtual void Actualizar() { }
    public virtual void Salir() { }


    // ***********************( Mi Unity )*********************** //
    public virtual void MiAwake() { }
    public virtual void MiStart() { }
    public virtual void MiFixedUpdate() { }
    public virtual void MiUpdate() { }


    // ***********************( Metodos Funcionales )*********************** //
    public void CambiarEstado(ref EstadoBase nuevoEstado)
    {
        if (estadoActual != null)
            estadoActual.Salir();

        estadoActual = nuevoEstado;
        estadoActual.Entrar();
    }
    public void CambiarSubEstado(ref EstadoBase nuevoSubEstado)
    {
        if (subEstadoActual != null)
            subEstadoActual.Salir();

        subEstadoActual = nuevoSubEstado;
        subEstadoActual.Entrar();
    }

    public void AgregarTransicion(Func<bool> condicion, EstadoBase estadoDestino)
    {
        transiciones[condicion] = estadoDestino;
    }
    public virtual void ActualizarTransiciones()
    {
        foreach (var transicion in transiciones)
        {
            if (transicion.Key.Invoke())
            {
                //CambiarEstado(ref transicion.Value);
                var estadoDestino = transicion.Value; // Almacenar el valor en una variable local
                CambiarEstado(ref estadoDestino);    // Usar la variable local como referencia
                break;
            }
        }
    }
    

    // ***********************( Llamas -> Unity )*********************** //
    private void Awake()
    {
        MiAwake();
    }
    private void Start()
    {
        MiStart();
    }
    private void FixedUpdate()
    {
        MiFixedUpdate();
    }
    private void Update()
    {
        MiUpdate();
    }
    //private void OnEnable()
    //{
    //    if (estadoActual != null)
    //        estadoActual.OnEntrar += Entrar;
    //}
}
