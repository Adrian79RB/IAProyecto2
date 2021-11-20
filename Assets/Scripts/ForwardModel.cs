using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardModel
{
    public GM gameStateInstance;

    public ForwardModel(GM initalState)
    {
        gameStateInstance = initalState;
    }
    public void MoveUnit(Unit unit, Tile tile, GM currentState)
    {

    }

    public void SetVillage(Village village, Tile tile, GM currentState)
    {

    }

    public void Attack (Unit allyUnit, Unit enemyUnit, GM currentState)
    {

    }

    public void CreateUnit ( Unit unit, GM currentState)
    {

    }

    public void CreateVillage (Village village, GM currentState)
    {

    }

    internal float HeuristicFunction()
    {
        throw new NotImplementedException();
    }
}
