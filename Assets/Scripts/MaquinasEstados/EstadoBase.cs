using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class EstadoBase : MonoBehaviour
{
    // ***********************( Variables/Declaraciones )*********************** //
    public MaquinaDeEstados MaquinaEstados { get; set; }
    public int MiIndex { get; set; }
    //private Component _esteComponente { get; set; }

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

    

    // ***********************( Llamas -> Unity )*********************** //
    private void Awake()
    {
        MiAwake();
    }
    private void OnEnable()
    {
        MiOnEnable();
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
    private void LateUpdate()
    {
        //MaquinaEstados.ActualizarTransiciones();

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

    //public EstadoBase(MaquinaDeEstados v_maquina)
    //{
    //    _esteComponente = v_maquina.gameObject.AddComponent(this.GetType());
    //    _esteComponente.enabled = false;
    //    MaquinaEstados = v_maquina;
    //}
    //public EstadoBase(EstadoBase nuevoEstado, GameObject goHost)
    //{
    //    nuevoEstado = new EstadoNulo();
    //    nuevoEstado = goHost.AddComponent<EstadoNulo>();
    //    _estadoActual = nuevoEstado;
    //    _go = goHost;

    //    _estadoActual.Entrar();
    //}
}


public class EstadoNulo : EstadoBase
{
    //public EstadoNulo(MaquinaDeEstados v_siMismo) : base(v_siMismo)
    //{ }

    public override void Entrar() { }

    public override void Init<T>(T dependencia)
    {
        throw new NotImplementedException();
    }

    public override void Salir() { }
}

public class EstadoInicio : EstadoBase
{
    //public EstadoInicio(MaquinaDeEstados v_siMismo) : base(v_siMismo)
    //{ }
    //public EstadoInicio(EstadoBase nuevoEstado, GameObject goHost) : base(nuevoEstado, goHost)
    //{ }

    public override void Entrar() { }

    public override void Init<T>(T dependencia)
    {
        throw new NotImplementedException();
    }

    public override void Salir() { }
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