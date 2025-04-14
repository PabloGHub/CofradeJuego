using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class EstadoBase : MonoBehaviour
{
    // ***********************( Variables/Declaraciones )*********************** //
    private EstadoBase _estadoActual;
    public EstadoBase EstadoActual
    {
        get { return _estadoActual; }
        set
        {
            if (_estadoActual != null)
            {
                _estadoActual.Salir(); 
                Destroy(_estadoActual);
            }

            _estadoActual = _go.AddComponent(value.GetType()) as EstadoBase;
            //_estadoActual = value;

            OnEstadoCambiado?.Invoke(_estadoActual);

            _estadoActual.Entrar();
        }
    }

    private EstadoBase _subEstadoActual;
    public EstadoBase SubEstadoActual
    {
        get { return _subEstadoActual; }
        set { _subEstadoActual = value; }
    }

    private Dictionary<Func<bool>, EstadoBase> _transiciones = new Dictionary<Func<bool>, EstadoBase>();
    private GameObject _go;


    // ***********************( Eventos )*********************** //
    public event Action<EstadoBase> OnEstadoCambiado;


    // ***********************( Metodos de Control )*********************** //
    public abstract void Entrar();
    public abstract void Salir();


    // ***********************( Mi Unity )*********************** //
    public virtual void MiAwake() { }
    public virtual void MiStart() { }
    public virtual void MiFixedUpdate() { }
    public virtual void MiUpdate() { }
    public virtual void MiOnDestroy() { }


    // ***********************( Metodos Funcionales )*********************** //
    public void Inicializar(out EstadoBase nuevoEstado, GameObject goHost)
    {
        //nuevoEstado = new EstadoNulo();
        nuevoEstado = goHost.AddComponent<EstadoNulo>();
        _estadoActual = nuevoEstado;
    }

    public void CambiarEstado(EstadoBase nuevoEstado)
    {
        EstadoActual = nuevoEstado;
    }
    public void CambiarSubEstado(EstadoBase nuevoSubEstado)
    {
        if (_subEstadoActual != null)
            _subEstadoActual.Salir();

        _subEstadoActual = nuevoSubEstado;
        _subEstadoActual.Entrar();
    }

    public void AgregarTransicion(Func<bool> condicion, EstadoBase estadoDestino)
    {
        _transiciones[condicion] = estadoDestino;
    }
    public virtual void ActualizarTransiciones()
    {
        foreach (var transicion in _transiciones)
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

        if (_transiciones.Count > 0)
            ActualizarTransiciones();
    }
    private void OnDestroy()
    {
       MiOnDestroy();
    }

    // ***********************( Contructores )*********************** //
    public EstadoBase() { }
    public EstadoBase(ref EstadoBase nuevoEstado)
    {
        // No funca.
        _estadoActual = nuevoEstado;
    }
    public EstadoBase(out EstadoBase nuevoEstado, GameObject goHost)
    {
        nuevoEstado = new EstadoNulo();
        nuevoEstado = goHost.AddComponent<EstadoNulo>();
        _estadoActual = nuevoEstado;
        _go = goHost;

        _estadoActual.Entrar();
    }
}

public class MaquinaDeEstados : EstadoBase
{
    public MaquinaDeEstados() { }
    public MaquinaDeEstados(out EstadoBase nuevoEstado, GameObject goHost) : base(out nuevoEstado, goHost)
    { }

    public override void Entrar() { }
    public override void Salir() { }
}

public class EstadoNulo : EstadoBase
{
    public override void Entrar() { }
    public override void Salir() { }
}

public class EstadoInicio : EstadoBase
{
    public EstadoInicio(out EstadoBase nuevoEstado, GameObject goHost) : base(out nuevoEstado, goHost)
    { }

    public override void Entrar() { }
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