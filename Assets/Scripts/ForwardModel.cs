using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ForwardModel
{
    public Tile[] currentTilesState;
    public Unit[] currentUnitsState;
    public int enemiesKilled;

    public ForwardModel(Tile[] initalTileState, Unit[] initialUnitState)
    {
        currentTilesState = initalTileState;
        currentUnitsState = initialUnitState;
    }

    public float MoveUnit(Unit unit, Tile tile, Unit[] units, Village[] villages)
    {
        GameObject unitCopied = GameObject.Instantiate(unit.gameObject);

        while (unitCopied.transform.position.x != tile.transform.position.x)
        {
            unitCopied.transform.position = Vector2.MoveTowards(unitCopied.transform.position, new Vector2(tile.transform.position.x, unitCopied.transform.position.y), unitCopied.GetComponent<Unit>().moveSpeed * Time.deltaTime);
        }
        while (unitCopied.transform.position.y != tile.transform.position.y)
        {
            unitCopied.transform.position = Vector2.MoveTowards(unitCopied.transform.position, new Vector2(unitCopied.transform.position.x, tile.transform.position.y), unitCopied.GetComponent<Unit>().moveSpeed * Time.deltaTime);
        }

        unitCopied.GetComponent<Unit>().hasMoved = true;
        unitCopied.GetComponent<Unit>().ResetWeaponIcon();
        unitCopied.GetComponent<Unit>().GetEnemies();
        unitCopied.GetComponent<Unit>().GetVillages();

        for (int i = 0; i < units.Length; i++)
        {
            if(units[i] == unit)
            {
                units[i] = unitCopied.GetComponent<Unit>();
                break;
            }
        }

        var finalScore = HeuristicFunction(units, villages);

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == unitCopied.GetComponent<Unit>())
            {
                units[i] = unit;
                break;
            }
        }
        unitCopied.GetComponent<Unit>().ResetWeaponIcon();
        GameObject.Destroy(unitCopied);

        return finalScore;
    }

    public float SetVillage(Village village, Tile tile, Unit[] units, Village[] villages)
    {
        GameObject villageCopied = GameObject.Instantiate(village.gameObject);

        villageCopied.transform.position = tile.transform.position;

        for(int i = 0; i < villages.Length; i++)
        {
            if(villages[i] == village)
            {
                villages[i] = villageCopied.GetComponent<Village>();
                break;
            }
        }

        var finalScore = HeuristicFunction(units, villages);

        for (int i = 0; i < villages.Length; i++)
        {
            if (villages[i] == villageCopied.GetComponent<Village>())
            {
                villages[i] = village;
                break;
            }
        }
        GameObject.Destroy(villageCopied);

        return finalScore;
    }

    public float Attack (Unit allyUnit, Unit enemyUnit, Unit[] units, Village[] villages)
    {
        GameObject copiedEnemy = GameObject.Instantiate(enemyUnit.gameObject);
        GameObject copiedAlly = GameObject.Instantiate(allyUnit.gameObject);

        int enemyDamege = copiedAlly.GetComponent<Unit>().attackDamage - copiedEnemy.GetComponent<Unit>().armor;
        int unitDamage = copiedEnemy.GetComponent<Unit>().defenseDamage - copiedAlly.GetComponent<Unit>().armor;

        if (enemyDamege >= 1)
        {
            copiedEnemy.GetComponent<Unit>().health -= enemyDamege;
        }

        if (copiedAlly.tag == "Archer" && enemyUnit.tag != "Archer")
        {
            if (Mathf.Abs(copiedAlly.transform.position.x - copiedEnemy.transform.position.x) + Mathf.Abs(copiedAlly.transform.position.y - copiedEnemy.transform.position.y) <= 1) // check is the enemy is near enough to attack
            {
                if (unitDamage >= 1)
                {
                    copiedAlly.GetComponent<Unit>().health -= unitDamage;
                }
            }
        }
        else
        {
            if (unitDamage >= 1)
            {
                copiedAlly.GetComponent<Unit>().health -= unitDamage;
            }
        }

        //TODO: Falta calcular el daño para los edificios

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == allyUnit)
                units[i] = copiedAlly.GetComponent<Unit>();
            if (units[i] == enemyUnit)
                units[i] = copiedEnemy.GetComponent<Unit>();
        }

        //TODO: Falta cambiar los edificios

        var finalScore = HeuristicFunction(units, villages);

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == copiedAlly.GetComponent<Unit>())
                units[i] = allyUnit;
            if (units[i] == copiedEnemy.GetComponent<Unit>())
                units[i] = enemyUnit;
        }

        GameObject.Destroy(copiedAlly);
        GameObject.Destroy(copiedEnemy);

        return finalScore;

    }

    public float CreateUnit ( Unit unit, Unit[] units, Village[] villages)
    {
        Unit[] newUnits = new Unit[units.Length + 1];

        for (int i = 0; i < newUnits.Length; i++)
        {
            if (i >= units.Length)
                newUnits[i] = unit;
            else
                newUnits[i] = units[i];
        }

        return HeuristicFunction(newUnits, villages);
    }

    public float CreateVillage (Village village, Unit[] units, Village[] villages)
    {
        Village[] newVillages = new Village[villages.Length + 1];

        for (int i = 0; i < newVillages.Length; i++)
        {
            if (i >= villages.Length)
                newVillages[i] = village;
            else
                newVillages[i] = villages[i];
        }

        return HeuristicFunction(units, newVillages);

    }

    internal float HeuristicFunction(Unit[] units, Village[] villages)
    {
        List<Unit> allyUnits = new List<Unit>();
        List<Unit> enemiesInRange = new List<Unit>();
        List<Village> villagesInRange = new List<Village>();
        float allyHealth = 0f;
        float enemyHealth = 0f;
        float nearEnemies = 0f;
        
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].name.Contains("Blue"))
            {
                allyUnits.Add(units[i]);
                if (units[i].name.Contains("King"))
                    allyHealth += units[i].health * 1.75f;
                else
                    allyHealth += units[i].health;
            }
            else
            {
                if (units[i].name.Contains("King"))
                    enemyHealth += units[i].health * 1.75f;
                else
                    enemyHealth += units[i].health * 1.2f;
            }
        }

        /*for (int i = 0; i < villages.Length; i++)
        {
            if (villages[i].name.Contains("Blue"))
                allyHealth += villages[i].health + 2;
            else
                enemyHealth += villages[i].health + 2;
        }*/

        for (int i = 0; i < allyUnits.Count; i++)
        {
            for (int j = 0; j < allyUnits[i].enemiesInRange.Count; j++)
            {
                if (!enemiesInRange.Contains(allyUnits[i].enemiesInRange[j]))
                {
                    enemiesInRange.Add(allyUnits[i].enemiesInRange[j]);
                    nearEnemies++;
                }
            }

            /*for (int j = 0; j < allyUnits[i].villagesInRange.Count; j++)
            {
                if (!villagesInRange.Contains(allyUnits[i].villagesInRange[j]))
                {
                    villagesInRange.Add(allyUnits[i].villagesInRange[j]);
                    nearEnemies++;
                }
            }*/
        }

        return allyHealth - enemyHealth + nearEnemies + 2 * enemiesKilled;    
    }
}
