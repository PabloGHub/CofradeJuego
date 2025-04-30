using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class StateBase : MonoBehaviour
{
    // ***********************( Variables/Declaraciones )*********************** //
    public MachineState MachineState { get; set; }
    public object Source { get; set; } // Clase padre donde se se instancio la Maquina de Estados.
    public int MiIndex { get; set; }
    public Component ThisComponent { get; set; }

    // ***********************( Metodos de Control )*********************** //
    public abstract void Entrar();
    public abstract void Salir();


    // ***********************( Mi Unity )*********************** //
    public virtual void MiAwake() { }
    public virtual void MiOnEnable() { }
    public virtual void MiStart() { }
    public virtual void MiFixedUpdate() { }
    public virtual void MiUpdate() { }
    public virtual void MiLateUpdate() { }
    public virtual void MiOnDisable() { }
    public virtual void MiOnDestroy() { }

    

    // ***********************( Unity -> Mi )*********************** //
    private void Awake()
    {
        //this.ThisComponent = this.GetComponent(this.GetType());
        //MiIndex = this.MachineState.ObtenerIndice(this);
        
        MiAwake();
    }
    private void OnEnable()
    {
        this.ThisComponent = this.GetComponent(this.GetType());
        MiIndex = this.MachineState.ObtenerIndice(this);

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

    // ***********************( Contructores )*********************** //
    public abstract void Init<T>(T dependencia);
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