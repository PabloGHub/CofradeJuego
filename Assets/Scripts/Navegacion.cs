using CommandTerminal;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Navegacion : MonoBehaviour
{
    // ***********************( Declaraciones )*********************** //
    public static Navegacion nav;
    public Transform[] trayectoria;

    private int v_cantidadDifurcaciones_i = 0;
    private List<List<Transform>> v_caminosPosible_Transform = new List<List<Transform>>();

    // ***********************( Funciones Unity )*********************** //
    private void Awake()
    {
        if (nav != this)
        {
            nav = this;
        }
    }

    private void Start()
    {
        int c = 0;

        for (int i = 0; i < trayectoria.Length; i++)
        {
            Punto p = trayectoria[i].GetComponent<Punto>();
            if (p.difurcacion)
            {
                if (c == 0)
                {
                    v_caminosPosible_Transform.Add(new List<Transform>());
                    v_cantidadDifurcaciones_i++;
                }

                v_caminosPosible_Transform[v_cantidadDifurcaciones_i - 1].Add(trayectoria[i]);
                c++;
            }
            else
                c = 0;
            
        }

        Debug.Log("Cantidad de Difurcaciones: " + v_cantidadDifurcaciones_i);
        for (int i = 0; i < v_cantidadDifurcaciones_i; i++)
        {
            Debug.Log("Difurcacion " + i + ": " + v_caminosPosible_Transform[i].Count);
        }
    }
    // ***********************( Funciones Nuestras )*********************** //
    public bool comprobarCaminos()
    {
        int v_verificado_i = 0;

        for (int i = 0; i < v_caminosPosible_Transform.Count; i++)
        {
            for (int j = 0; j < v_caminosPosible_Transform[i].Count; j++)
            {
                if (v_caminosPosible_Transform[i][j].GetComponent<Punto>().v_elegido_b)
                {
                    v_verificado_i++;
                    break;
                }
            }
        }

        return v_verificado_i == v_cantidadDifurcaciones_i;
    }

    public void activarCaminos(CommandArg[] args)
    {
        int v_difurcacion_i = 0;
        for (int i = 0; i < args.Length; i++)
        {
            if (i % 2 == 0)
            {
                v_difurcacion_i = args[i].Int;
                for (int j = 0; j < v_caminosPosible_Transform[v_difurcacion_i].Count; j++)
                {
                    v_caminosPosible_Transform[v_difurcacion_i][j].GetComponent<Punto>().v_elegido_b = false;
                }
            }
            else
                v_caminosPosible_Transform[v_difurcacion_i][args[i].Int].GetComponent<Punto>().v_elegido_b = true;
        }
    }

    [RegisterCommand(Help = "Selecionar Caminos")]
    static void CommandCamino(CommandArg[] args)
    {
        if (args.Length == 0)
        {
            Terminal.Log("No se ha seleccionado ningun camino");
            return;
        }
        Navegacion.nav.activarCaminos(args);
    }
}