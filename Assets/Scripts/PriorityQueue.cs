﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
    public class NodoPQ
    {
        public Tile tile;
        public NodoPQ anterior, siguiente;
        public float prioridad;
    }
    private NodoPQ raiz;
    private int length;

    public PriorityQueue()
    {
        raiz = null;
    }

    public void Insertar(Tile nodoNew, float prioridad)
    {
        length++;
        NodoPQ nuevo = new NodoPQ();
        nuevo.tile = nodoNew;
        nuevo.prioridad = prioridad;
        if (raiz == null)
        { //Si la cola est� vacia
            raiz = nuevo;
            raiz.anterior = null;
            raiz.siguiente = null;
        }
        else
        {
            if (raiz.prioridad >= nuevo.prioridad)
            { //Si el nuevo nodo sustituye al primero
                NodoPQ auxiliar = raiz;
                nuevo.siguiente = auxiliar;
                raiz = nuevo;
                auxiliar.anterior = raiz;
            }
            else
            {
                for (NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente)
                { // Si el nuevo nodo se situa entre dos
                    if (nodo.anterior != null && nuevo.prioridad < nodo.prioridad && nuevo.prioridad >= nodo.anterior.prioridad)
                    {
                        nodo.anterior.siguiente = nuevo;
                        nuevo.anterior = nodo.anterior;
                        nodo.anterior = nuevo;
                        nuevo.siguiente = nodo;
                        return;
                    }
                    else if (nodo.siguiente == null) //Si llegamos al �ltimo nodo y siguen siendo m�s peque�os
                    {
                        nodo.siguiente = nuevo;
                        nuevo.anterior = nodo;
                        nuevo.siguiente = null;
                    }
                }
            }
        }
    }

    //Devolver el nodo raiz con menor prioridad de la lista
    public Tile Devolver()
    {
        if (raiz != null)
        {
            NodoPQ primero = raiz;
            raiz = raiz.siguiente;

            if (raiz != null)
                raiz.anterior = null;

            primero.siguiente = null;
            length--;
            return primero.tile;
        }
        return null;
    }

    //Consultar si la cola está vacia
    public bool Vacia()
    {
        if (length == 0)
        {
            return true;
        }
        return false;
    }

    //Cambiar la prioridad de un nodo de la cola
    public void CambiarPrio(Tile nodoComp, float nuevaPrio)
    {

        for (NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente)
        {
            if (nodo.tile == nodoComp)
            {
                if (nodo.anterior != null)
                    nodo.anterior.siguiente = nodo.siguiente;
                if (nodo.siguiente != null)
                    nodo.siguiente.anterior = nodo.anterior;
                nodo.anterior = null;
                nodo.siguiente = null;
                Insertar(nodoComp, nuevaPrio);
                return;
            }
        }

    }

    //Comprobamos si un nodo esta en la cola de prioridad
    public bool EncontrarNodo(Tile nodoBuscado)
    {
        for (NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente)
        {
            if (nodo.tile == nodoBuscado)
                return true;
        }
        return false;
    }

    //Devolvemos un nodo que hemos buscado
    public Tile ConsultarNodo(Tile nodoBuscado)
    {
        for (NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente)
        {
            if (nodo.tile == nodoBuscado)
                return nodo.tile;
        }

        return null;
    }

    public void EliminarNodo(Tile nodoEliminado)
    {
        if (length > 0)
        {
            for (NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente)
            {
                if (nodo.tile == nodoEliminado)
                {
                    if (nodo == raiz)
                    {
                        raiz = nodo.siguiente;
                        nodo.siguiente.anterior = null;
                        nodo.siguiente = null;
                    }
                    else if (nodo.siguiente == null)
                    {
                        nodo.anterior.siguiente = null;
                        nodo.anterior = null;
                    }
                    else
                    {
                        nodo.anterior.siguiente = nodo.siguiente;
                        nodo.siguiente.anterior = nodo.anterior;
                        nodo.siguiente = null;
                        nodo.anterior = null;
                    }
                    length--;
                }
            }
        }
    }

    public int getLegth()
    {
        return length;
    }

    public void MostrarContenido()
    {
        int i = 1;
        for (NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente)
        {
            Debug.Log("Nodo " + i + ": " + nodo.tile.transform.position);
            i++;
        }
    }
}
