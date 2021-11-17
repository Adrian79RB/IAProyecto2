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
    // Enemy Units
    List<GameObject> enemyUnits;

    // Ally Units
    List<GameObject> allyUnits;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GM>();
        tilesFather = GameObject.Find("Tiles").transform;

        Transform units = GameObject.Find("Units").transform;
        enemyUnits = new List<GameObject>();
        allyUnits = new List<GameObject>();

        for (int i = 0; i < units.childCount; i++)
        {
            if (units.GetChild(i).name.Contains("Dark"))
                enemyUnits.Add(units.GetChild(i).gameObject);
            else
                allyUnits.Add(units.GetChild(i).gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
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
                        forwardModel.CreateUnit(knight.GetComponent<Unit>());
                        unitsBought[j, 0]++;
                    }
                    else if (randomChoose < 0.4)
                    {
                        forwardModel.CreateUnit(archer.GetComponent<Unit>());
                        unitsBought[j, 1]++;
                    }
                    else if (randomChoose < 0.6)
                    {
                        forwardModel.CreateUnit(dragon.GetComponent<Unit>());
                        unitsBought[j, 2]++;
                    }
                    else if (randomChoose < 0.8)
                    {
                        forwardModel.CreateVillage(village.GetComponent<Village>());
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

    internal bool tryToMove()
    {
        throw new NotImplementedException();
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
        float minDistance = Mathf.Infinity;
        int choosenIndex = 0;
        for (int i = 0; i < tilesFather.childCount; i++)
        {
            if (tilesFather.GetChild(i).GetComponent<Tile>().isCreatable)
            {
                float distance = Vector3.Distance(tilesFather.GetChild(i).position, enemyKing.transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
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

    public void AddEnemyToList(GameObject enemy)
    {
        enemyUnits.Add(enemy);
    }


    public bool tryToAttack()
    {
        
    }
}
