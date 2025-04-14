using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class EstadoBase : MonoBehaviour
{
    // ***********************( Variables/Declaraciones )*********************** //
    public EstadoBase MaquinaEstados { get; set; }

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

    public Dictionary<Func<bool>, EstadoBase> _transiciones;
    public List<EstadoBase> estadosPosibles { get; set; }
    public GameObject _go;


    // ***********************( Eventos )*********************** //
    public event Action<EstadoBase> OnEstadoCambiado;


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


    // ***********************( Metodos Funcionales )*********************** //
    public void Inicializar(EstadoBase maquina, GameObject goHost, List<EstadoBase> estadosPosibles)
    {
        this.estadosPosibles = estadosPosibles;
        Inicializar(maquina, goHost);
    }
    public void Inicializar(EstadoBase maquina, GameObject goHost)
    {
        //_estadoActual =  nuevoEstado;
        _go = goHost;
        MaquinaEstados = maquina;

        if (_estadoActual != null)
        {
            _estadoActual.Salir();
            Destroy(_estadoActual);
        }

        EstadoBase nuevoEstado = new EstadoNulo();
        _estadoActual = _go.AddComponent(nuevoEstado.GetType()) as EstadoBase;
        //_estadoActual = nuevoEstado;

        _estadoActual.Entrar();
    }

    public void CambiarEstado(EstadoBase nuevoEstado)
    {
        if (nuevoEstado == null)
            Debug.LogError("*- Intento de cambiar estado pasando un nulo -*");

        EstadoActual = nuevoEstado;
    }
    public void CambiarEstado(int nuevoEstado)
    {
        var _posibleNovoEstado = MaquinaEstados.estadosPosibles[nuevoEstado];
        if (_posibleNovoEstado == null)
        {
            Debug.LogError("*- Intento de cambiar estado pasando un nulo -*");
            return;
        }

        EstadoActual = _posibleNovoEstado;
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
        MaquinaEstados._transiciones[condicion] = estadoDestino;
    }
    public void AgregarTransicion(Func<bool> condicion, int estadoDestino)
    {
        MaquinaEstados._transiciones[condicion] = MaquinaEstados.estadosPosibles[estadoDestino];
    }
    public virtual void ActualizarTransiciones()
    {
        foreach (var transicion in MaquinaEstados._transiciones)
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
    

    // ***********************( Llamas -> Unity )*********************** //
    private void Awake()
    {
        MaquinaEstados._transiciones = new Dictionary<Func<bool>, EstadoBase>();
        MaquinaEstados.estadosPosibles = new List<EstadoBase>();

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

        if (MaquinaEstados._transiciones.Count > 0)
            ActualizarTransiciones();
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
    public EstadoBase() { }
    public EstadoBase(EstadoBase nuevoEstado)
    {
        // No funca.
        _estadoActual = nuevoEstado;
    }
    public EstadoBase(EstadoBase nuevoEstado, GameObject goHost)
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
    public MaquinaDeEstados(EstadoBase nuevoEstado, GameObject goHost) : base(nuevoEstado, goHost)
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
    public EstadoInicio(EstadoBase nuevoEstado, GameObject goHost) : base(nuevoEstado, goHost)
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