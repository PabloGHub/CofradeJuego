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
        set { estadoActual = value; }
    }

    public EstadoBase subEstadoActual;
    private Dictionary<Func<bool>, EstadoBase> transiciones = new Dictionary<Func<bool>, EstadoBase>();



    // ***********************( Eventos )*********************** //
    public event Action OnEntrar;
    public event Action OnSalir;


    // ***********************( Metodos Abstractos )*********************** //
    public abstract void Entrar();
    public abstract void Actualizar();
    public abstract void Salir();



    // ***********************( Metodos Virtuales )*********************** //
    public virtual void MiAwake() { }
    public virtual void MiStart() { }
    public virtual void MiFixedUpdate() { }
    public virtual void MiUpdate() { }


    // ***********************( Metodos Funcionales )*********************** //
    public void CambiarEstado(EstadoBase nuevoEstado)
    {
        if (estadoActual != null)
            estadoActual.Salir();

        estadoActual = nuevoEstado;
        estadoActual.Entrar();
    }
    public void CambiarSubEstado(EstadoBase nuevoSubEstado)
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
                CambiarEstado(transicion.Value);
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
}
