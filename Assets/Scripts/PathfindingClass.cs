using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathfindingClass
{
    public static void obtenerCamino(Tile target, Tile start, ref List<Tile> ordenFather )
    {
        //Creamos el nodo inicial del pathfinding
        Tile nodoActual = target;
        nodoActual.costSoFar = 0;
        float distanceToStart = Mathf.Sqrt(Mathf.Pow(nodoActual.transform.position.x - start.transform.position.x, 2) + Mathf.Pow(nodoActual.transform.position.y - start.transform.position.y, 2));
        nodoActual.estimatedTotalCost = nodoActual.costSoFar + distanceToStart;


        //Creamos las listas abierta y cerrada
        PriorityQueue openedQueue = new PriorityQueue();
        PriorityQueue closedQueue = new PriorityQueue();

        openedQueue.Insertar(nodoActual, nodoActual.estimatedTotalCost);
        
        while (openedQueue.getLegth() > 0)
        {
            nodoActual = openedQueue.Devolver();
            if (nodoActual == start)
                break;

            for (int i = 0; i < nodoActual.arcs.Count; i++)
            {
                Tile nextNode = nodoActual.arcs[i].GetComponent<Tile>();
                float distanceToNextNode = nodoActual.costSoFar + nodoActual.weigths[i];

                if (closedQueue.EncontrarNodo(nextNode))
                {
                    Tile nodoAuxiliar = closedQueue.ConsultarNodo(nextNode);
                    if (nodoAuxiliar != null && nodoAuxiliar.costSoFar <= distanceToNextNode)
                        continue;

                    closedQueue.EliminarNodo(nodoAuxiliar);
                    distanceToStart = nodoAuxiliar.estimatedTotalCost - nodoAuxiliar.costSoFar;
                }
                else if (openedQueue.EncontrarNodo(nextNode))
                {
                    Tile nodoAuxiliar = openedQueue.ConsultarNodo(nextNode);
                    if (nodoAuxiliar != null && nodoAuxiliar.costSoFar <= distanceToNextNode)
                        continue;

                    distanceToStart = nodoAuxiliar.estimatedTotalCost - nodoAuxiliar.costSoFar;
                }
                else
                {
                    distanceToStart = Mathf.Sqrt(Mathf.Pow(nextNode.transform.position.x - start.transform.position.x, 2) + Mathf.Pow(nextNode.transform.position.y - start.transform.position.y, 2));
                }

                nextNode.costSoFar = distanceToNextNode;
                nextNode.estimatedTotalCost = distanceToNextNode + distanceToStart;
                nextNode.father = nodoActual;

                if (!openedQueue.EncontrarNodo(nextNode) && !closedQueue.EncontrarNodo(nextNode))
                    openedQueue.Insertar(nextNode, nextNode.estimatedTotalCost);
                else
                    openedQueue.CambiarPrio(nextNode, nextNode.estimatedTotalCost);

            }
            closedQueue.Insertar(nodoActual, nodoActual.estimatedTotalCost);
        }

        if (nodoActual == start)
        {
            while (nodoActual != target)
            {
                Debug.Log("Hola");
                ordenFather.Add(nodoActual);
                nodoActual = nodoActual.father;
            }
            ordenFather.Add(target);
        }
    }
}
