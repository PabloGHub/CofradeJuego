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
            if (estadoActual != null)
                estadoActual.Salir();

            estadoActual = value;
            OnEstadoCambiado?.Invoke(estadoActual);

            estadoActual.Entrar();
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
    public void Inicializar(ref EstadoBase nuevoEstado)
    {
        estadoActual = nuevoEstado;
    }

    private void CambiarEstado(EstadoBase nuevoEstado)
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
                var estadoDestino = transicion.Value;
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