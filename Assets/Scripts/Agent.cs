using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public GM gameManager;
    public CharacterCreation characterCreation;
    public int maxActions;

    // Units
    public GameObject knight;
    public GameObject archer;
    public GameObject dragon;
    public GameObject village;

    BehaviourTree tree;
    ForwardModel forwardModel;

    Transform tilesFather;

    // Ally Units
    List<GameObject> allyUnits;
    
    int enemiesCount;
    Transform units;

    // Enemy King
    GameObject enemyKing;

    // Start is called before the first frame update
    void Start()
    {
        tree = new BehaviourTree();
        gameManager = FindObjectOfType<GM>();
        tilesFather = GameObject.Find("Tiles").transform;

        enemyKing = GameObject.Find("Dark King");
        enemiesCount = 0;
        units = GameObject.Find("Units").transform;
        allyUnits = new List<GameObject>();

        for (int i = 0; i < units.childCount; i++)
        {
            if (units.GetChild(i).name.Contains("Blue"))
                allyUnits.Add(units.GetChild(i).gameObject);
            else
                enemiesCount++;
        }

    }

    public void TurnChanged()
    {
        allyUnits = new List<GameObject>();
        enemiesCount = 0;

        for (int i = 0; i < units.childCount; i++)
        {
            if (units.GetChild(i).name.Contains("Blue"))
                allyUnits.Add(units.GetChild(i).gameObject);
            else
                enemiesCount++;
        }

        if (tree.StartBehaviour(this) == 2){
            Debug.Log("Funciona y se ha hecho algo.");
        }
        else if(tree.StartBehaviour(this) == 3)
        {
            Debug.Log("Funciona pero no ha hecho nada");
        }
    }

    public bool tryToShop()
    {
        if (forwardModel.gameStateInstance.player2Gold > 40 && UnityEngine.Random.value > 0.2)
        {
            float[] scores = new float[maxActions];
            int[,] unitsBought = new int[maxActions, 4];
            for (int i = 0; i < maxActions; i++)
            {
                int maxTries = Mathf.RoundToInt(forwardModel.gameStateInstance.player2Gold / 40);
                int j = 0;

                while (j < maxTries && forwardModel.gameStateInstance.player2Gold >= 40)
                {
                    float randomChoose = UnityEngine.Random.value;
                    if (randomChoose < 0.2)
                    {
                        forwardModel.CreateUnit(knight.GetComponent<Unit>(), gameManager);
                        unitsBought[j, 0]++;
                    }
                    else if (randomChoose < 0.4)
                    {
                        forwardModel.CreateUnit(archer.GetComponent<Unit>(), gameManager);
                        unitsBought[j, 1]++;
                    }
                    else if (randomChoose < 0.6)
                    {
                        forwardModel.CreateUnit(dragon.GetComponent<Unit>(), gameManager);
                        unitsBought[j, 2]++;
                    }
                    else if (randomChoose < 0.8)
                    {
                        forwardModel.CreateVillage(village.GetComponent<Village>(), gameManager);
                        unitsBought[j, 3]++;
                    }

                    j++;
                }
                scores[i] = forwardModel.HeuristicFunction();
            }

            int scoreIndex = 0;
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] > scores[scoreIndex])
                    scoreIndex = i;
            }

            BuyNewUnit(scoreIndex, unitsBought);
            return true;
        }
        return false;
    }

    public bool tryToMove()
    {
        for (int i = 0; i < allyUnits.Count; i++)
        {
            Unit ally = allyUnits[i].GetComponent<Unit>();
            var tiles = ally.GetTilesArray();
            float maxScore = 0f;
            Tile targetTile = null;

            foreach(Tile tile in tiles)
            {
                forwardModel.MoveUnit(ally, tile, gameManager);
                var score = forwardModel.HeuristicFunction();
                if (score > maxScore)
                {
                    maxScore = score;
                    targetTile = tile;
                }
            }

            ally.Move(targetTile.transform);
        }

        return true;
    }

    private void BuyNewUnit(int scoreIndex, int[,] unitsBought)
    {
        for (int i = 0; i < unitsBought[scoreIndex, 0]; i++)
        {
            characterCreation.BuyUnit(knight.GetComponent<Unit>());
            chooseCharacterCreationTile();
        }

        for (int i = 0; i < unitsBought[scoreIndex, 1]; i++)
        {
            characterCreation.BuyUnit(archer.GetComponent<Unit>());
            chooseCharacterCreationTile();
        }

        for (int i = 0; i < unitsBought[scoreIndex, 2]; i++)
        {
            characterCreation.BuyUnit(dragon.GetComponent<Unit>());
            chooseCharacterCreationTile();
        }

        for (int i = 0; i < unitsBought[scoreIndex, 3]; i++)
        {
            characterCreation.BuyVillage(village.GetComponent<Village>());
            chooseCharacterCreationTile();
        }
    }

    private void chooseCharacterCreationTile()
    {
        float maxScore = 0f;
        int choosenIndex = 0;
        for (int i = 0; i < tilesFather.childCount; i++)
        {
            if (tilesFather.GetChild(i).GetComponent<Tile>().isCreatable)
            {
                var score = 0f;
                if (gameManager.createdUnit != null)
                {
                    forwardModel.MoveUnit(gameManager.createdUnit, tilesFather.GetChild(i).GetComponent<Tile>(), gameManager);
                    score = forwardModel.HeuristicFunction();
                }
                else if(gameManager.createdVillage != null)
                {
                    forwardModel.SetVillage(gameManager.createdVillage, tilesFather.GetChild(i).GetComponent<Tile>(), gameManager);
                    score = forwardModel.HeuristicFunction();
                }

                if(score > maxScore)
                {
                    maxScore = score;
                    choosenIndex = i;
                }
            }
        }

        Unit unit = Instantiate(gameManager.createdUnit, new Vector3(tilesFather.GetChild(choosenIndex).position.x, tilesFather.GetChild(choosenIndex).position.y, 0), Quaternion.identity);
        unit.hasMoved = true;
        unit.hasAttacked = true;
        allyUnits.Add(unit.gameObject);
        gameManager.ResetTiles();
        gameManager.createdUnit = null;
    }

    public bool tryToAttack()
    {
        for (int i = 0; i < allyUnits.Count; i++)
        {
            Unit ally = allyUnits[i].GetComponent<Unit>();
            ally.GetEnemies();
            if(!ally.hasAttacked && ally.enemiesInRange.Count > 0)
            {
                Unit targetEnemy = ally.enemiesInRange[0];
                float maxScore = 0f;

                for(int j = 1; j < ally.enemiesInRange.Count; j++)
                {
                    forwardModel.Attack(ally, ally.enemiesInRange[j], gameManager);
                    var score = forwardModel.HeuristicFunction();
                    if (score > maxScore)
                    {
                        maxScore = score;
                        targetEnemy = ally.enemiesInRange[j];
                    }
                }

                ally.Attack(targetEnemy);
            }
        }

        return true;
    }

    public bool checkEnemiesContact()
    {
        List<Unit> enemiesInContact = new List<Unit>();

        for(int i = 0; i < allyUnits.Count; i++)
        {
            for (int j = 0; j < allyUnits[i].GetComponent<Unit>().enemiesInRange.Count; j++)
            {
                var enemy = allyUnits[i].GetComponent<Unit>().enemiesInRange[j];
                if (!enemiesInContact.Contains(enemy))
                {
                    enemiesInContact.Add(enemy);
                }
            }
        }

        if (enemiesInContact.Count == enemiesCount)
            return true;
        else
            return false;
    }
}
