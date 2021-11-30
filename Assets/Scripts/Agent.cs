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

    public int enemiesKilled;

    BehaviourTree tree;
    ForwardModel forwardModel;

    Transform tilesFather;

    // Ally Units
    List<GameObject> allyUnits;
    
    int enemiesCount;

    // Start is called before the first frame update
    void Start()
    {
        tree = new BehaviourTree();

        gameManager = FindObjectOfType<GM>();
        tilesFather = GameObject.Find("Tiles").transform;

        enemiesCount = 0;
        Unit[] units = FindObjectsOfType<Unit>();
        allyUnits = new List<GameObject>();

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].name.Contains("Blue"))
                allyUnits.Add(units[i].gameObject);
            else
                enemiesCount++;
        }

        enemiesKilled = 0;

        Tile[] tileState = FindObjectsOfType<Tile>();
        Unit[] unitState = FindObjectsOfType<Unit>();
        forwardModel = new ForwardModel(tileState, unitState);
    }

    private void Update()
    {
        if (gameManager.playerTurn == 2)
            StartTurn();
    }

    public void StartTurn()
    {
        allyUnits = new List<GameObject>();
        enemiesCount = 0;
        Unit[] units = FindObjectsOfType<Unit>();

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].name.Contains("Blue"))
                allyUnits.Add(units[i].gameObject);
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

    public void EndAgentTurn()
    {
        allyUnits = null;
        enemiesCount = 0;

        gameManager.EndTurn();
    }

    public bool tryToShop()
    {
        if (gameManager.player2Gold > 40 && UnityEngine.Random.value > 0.2)
        {
            Unit[] units = FindObjectsOfType<Unit>();
            Village[] villages = FindObjectsOfType<Village>();
            float[] scores = new float[maxActions];
            int[,] unitsBought = new int[maxActions, 4];
            for (int i = 0; i < maxActions; i++)
            {
                float currentGold = gameManager.player2Gold;
                float score = 0f;

                while (currentGold >= knight.GetComponent<Unit>().cost)
                {
                    float randomChoose = UnityEngine.Random.value;
                    if (randomChoose < 0.2)
                    {
                        if (currentGold > knight.GetComponent<Unit>().cost)
                        {
                            score += forwardModel.CreateUnit(knight.GetComponent<Unit>(), units, villages);
                            unitsBought[i, 0]++;
                        }
                        currentGold -= knight.GetComponent<Unit>().cost;
                    }
                    else if (randomChoose < 0.4)
                    {
                        if (currentGold > archer.GetComponent<Unit>().cost)
                        {
                            score += forwardModel.CreateUnit(archer.GetComponent<Unit>(), units, villages);
                            unitsBought[i, 1]++;
                        }
                        currentGold -= archer.GetComponent<Unit>().cost;
                    }
                    else if (randomChoose < 0.6)
                    {
                        if (currentGold > dragon.GetComponent<Unit>().cost)
                        {
                            score += forwardModel.CreateUnit(dragon.GetComponent<Unit>(), units, villages);
                            unitsBought[i, 2]++;
                        }
                        currentGold -= dragon.GetComponent<Unit>().cost;
                    }
                    else if (randomChoose < 0.8)
                    {
                        if (currentGold > village.GetComponent<Village>().cost)
                        {
                            score += forwardModel.CreateVillage(village.GetComponent<Village>(), units, villages);
                            unitsBought[i, 3]++;
                        }
                        currentGold -= village.GetComponent<Village>().cost;
                    }
                }
                scores[i] = score;
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
        Unit[] currentUnits = FindObjectsOfType<Unit>();
        Village[] villages = FindObjectsOfType<Village>();

        //StartCoroutine(movingUnits(currentUnits, villages));
        for (int i = 0; i < allyUnits.Count; i++)
        {
            Unit ally = allyUnits[i].GetComponent<Unit>();
            var tiles = ally.GetTilesArray();
            float maxScore = 0f;
            Tile targetTile = null;
            Debug.Log("Unidad buscando casilla: " + ally.transform.name);

            foreach (Tile tile in tiles)
            {
                Debug.Log("Tile: " + tile.transform.position + "; State: " + tile.isClear());
                var score = forwardModel.MoveUnit(ally, tile, currentUnits, villages);
                if (score > maxScore)
                {
                    maxScore = score;
                    targetTile = tile;
                }
            }

            if (targetTile)
            {
                Debug.Log("Tile Final: " + targetTile.transform.position + "; state: " + targetTile.isClear());
                ally.Move(targetTile.transform);
                ally.ResetWeaponIcon();
            }
        }

        return true;
    }

    IEnumerator movingUnits(Unit[] currentUnits, Village[] villages)
    {
        for (int i = 0; i < allyUnits.Count; i++)
        {
            Unit ally = allyUnits[i].GetComponent<Unit>();
            var tiles = ally.GetTilesArray();
            float maxScore = 0f;
            Tile targetTile = null;
            Debug.Log("Unidad buscando casilla: " + ally.transform.name);

            foreach (Tile tile in tiles)
            {
                Debug.Log("Tile: " + tile.transform.position + "; State: " + tile.isClear());
                var score = forwardModel.MoveUnit(ally, tile, currentUnits, villages);
                if (score > maxScore)
                {
                    maxScore = score;
                    targetTile = tile;
                }
            }

            if (targetTile)
            {
                Debug.Log("Tile Final: " + targetTile.transform.position + "; state: " + targetTile.isClear());
                ally.Move(targetTile.transform);
                ally.ResetWeaponIcon();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void BuyNewUnit(int scoreIndex, int[,] unitsBought)
    {
        for (int i = 0; i < unitsBought[scoreIndex, 0]; i++)
        {
            if (!knight.GetComponent<Unit>().gm)
                knight.GetComponent<Unit>().gm = gameManager;
            characterCreation.BuyUnit(knight.GetComponent<Unit>());
            chooseCharacterCreationTile();
        }

        for (int i = 0; i < unitsBought[scoreIndex, 1]; i++)
        {
            if (!archer.GetComponent<Unit>().gm)
                archer.GetComponent<Unit>().gm = gameManager;
            characterCreation.BuyUnit(archer.GetComponent<Unit>());
            chooseCharacterCreationTile();
        }

        for (int i = 0; i < unitsBought[scoreIndex, 2]; i++)
        {
            if (!dragon.GetComponent<Unit>().gm)
                dragon.GetComponent<Unit>().gm = gameManager;
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
        Unit[] curretUnits = FindObjectsOfType<Unit>();
        Village[] villages = FindObjectsOfType<Village>();
        Debug.Log("Elegimos la tesela de creación");

        for (int i = 0; i < tilesFather.childCount; i++)
        {
            if (tilesFather.GetChild(i).GetComponent<Tile>().isClear())
            {
                var score = 0f;
                if (gameManager.createdUnit != null)
                {
                    score = forwardModel.MoveUnit(gameManager.createdUnit, tilesFather.GetChild(i).GetComponent<Tile>(), curretUnits, villages);
                }
                else if(gameManager.createdVillage != null)
                {
                    score = forwardModel.SetVillage(gameManager.createdVillage, tilesFather.GetChild(i).GetComponent<Tile>(), curretUnits, villages);
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
            Unit[] units = FindObjectsOfType<Unit>();
            Village[] villages = FindObjectsOfType<Village>();
            ally.GetEnemies();
            if(!ally.hasAttacked && ally.enemiesInRange.Count > 0)
            {
                Unit targetEnemy = ally.enemiesInRange[0];
                float maxScore = 0f;

                for(int j = 1; j < ally.enemiesInRange.Count; j++)
                {
                    var score = forwardModel.Attack(ally, ally.enemiesInRange[j], units, villages);
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
