using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ControladorNazareno : MaquinaDeEstados
{
    // ***********************( Declaraciones )*********************** //
    private float cercaniaAlObjetivo = 2.5f;
    public int v_objetivoIndex_i = 0;
    public Movimiento v_movimiento;
    private Transform v_objetivo_Transform;

    // Datos
    public string nombre;
    public int id;

    // --- Maquina de Estados --- //
    public override EstadoBase Estado { get; set; }
    public override EstadoBase SubEstado { get; set; }

    // ***********************( Funciones Unity )*********************** //
    private void Start()
    {
        // Inicializar Maquina de Estados
        Inicializar(gameObject);
        estadosPosibles = new List<EstadoBase>
        {
            CrearEstado<EstadoLejosAdelantado, ControladorNazareno>(this),
            CrearEstado<EstadoLejosMedio, ControladorNazareno>(this),
            CrearEstado<EstadoLejosAtrasado, ControladorNazareno>(this)
        };


        // Movimiento
        v_movimiento = GetComponent<Movimiento>();
        if (v_movimiento == null)
        {
            Debug.LogError("El Nazareno no tiene un componente Movimiento.");
            return;
        }
        v_objetivo_Transform = Navegacion.nav.trayectoria[v_objetivoIndex_i];
        v_movimiento.v_objetivo_Transform = v_objetivo_Transform;
    }

    /*
        private void Update()
        {
            if (ControladorPPAL.v_pausado_b)
                return;

            bool v_lejosPeloton_b = Vector3.Distance(transform.position, Peloton.peloton.transform.position) > Peloton.peloton.v_distanciaAlPeloton_f;
            if (v_lejosPeloton_b && v_objetivoIndex_i < Peloton.peloton.v_objetivoIndex_i)
            {
            
            }
            else if (v_lejosPeloton_b)
            {
                v_movimiento.v_esperando_b = true;
            }
            else 
            {
                v_movimiento.v_esperando_b = false;
            }
        }
     */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("puntoControl") && collision.gameObject == v_objetivo_Transform.gameObject)
        {
            if (Vector3.Distance(transform.position, v_movimiento.v_objetivo_Transform.position) < cercaniaAlObjetivo)
            {
                actualizarObjetivo();
            }
        }
    }

    // ***********************( Funciones Nuestras )*********************** //
    void extracion()
    {
        // TODO: cuando llega a carrera destuir o ocular al nazareno.
    }

    private void actualizarObjetivo()
    {
        v_objetivoIndex_i++;

        if (v_objetivoIndex_i >= Navegacion.nav.trayectoria.Length)
            extracion();

        while (true)
        {
            Punto punto = Navegacion.nav.trayectoria[v_objetivoIndex_i].GetComponent<Punto>();

            if (!punto.difurcacion)
            {
                break;
            }
            else if (!punto.v_elegido_b)
            {
                v_objetivoIndex_i++;
            }
            else
            {
                break;
            }
        }

        v_objetivo_Transform = Navegacion.nav.trayectoria[v_objetivoIndex_i];
        v_movimiento.v_objetivo_Transform = v_objetivo_Transform;
    }

    // ***********************( Estados de la MAQUINA DE ESTADOS )*********************** //
    // ************ Estado de Movimiento ************ //
    // --- LEJOS --- //
    class EstadoLejosAdelantado : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }

        public override void Entrar()
        {
            // Código para entrar en el estado
        }

        public override void Salir()
        {
            // Código para salir del estado
        }
    }
    class EstadoLejosMedio : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }

        public override void Entrar()
        {
            // Código para entrar en el estado
        }

        public override void Salir()
        {
            // Código para salir del estado
        }
    }
    class EstadoLejosAtrasado : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }

        public override void Entrar()
        {
            // Código para entrar en el estado
        }

        public override void Salir()
        {
            // Código para salir del estado
        }
    }


    // --- CERCA --- //
    class EstadoCerca : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }

        public override void Entrar()
        {
            // Código para entrar en el estado
        }
        public override void Salir()
        {
            // Código para salir del estado
        }
    }


    // --- ATACANDO --- //
    class EstadoNada : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }

        public override void Entrar()
        {
            // Código para entrar en el estado
        }
        public override void Salir()
        {
            // Código para salir del estado
        }
    }
    class EstadoAtacar : EstadoBase
    {
        private ControladorNazareno v_controladorNazareno_s;
        public override void Init<T>(T dependencia)
        {
            v_controladorNazareno_s = dependencia as ControladorNazareno;
        }

        public override void Entrar()
        {
            // Código para entrar en el estado
        }
        public override void Salir()
        {
            // Código para salir del estado
        }
    }

    // ************ Estado de Movimiento ************ //
}