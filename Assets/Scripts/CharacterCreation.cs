using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{

    GM gm;

    public Button player1openButton;
    public Button player2openButton;

    public GameObject player1Menu;
    public GameObject player2Menu;

    private void Start()
    {
        gm = FindObjectOfType<GM>();
    }

    private void Update()
    {
        if (gm.playerTurn == 1)
        {
            player1openButton.interactable = true;
            player2openButton.interactable = false;
        }
        else
        {
            player2openButton.interactable = true;
            player1openButton.interactable = false;
        }
    }

    public void ToggleMenu(GameObject menu) {
        menu.SetActive(!menu.activeSelf);
    }

    public void CloseCharacterCreationMenus() {
        player1Menu.SetActive(false);
        player2Menu.SetActive(false);
    }

    public void BuyUnit (Unit unit) {

        if (unit.playerNumber == 1 && unit.cost <= gm.player1Gold)
        {
            player1Menu.SetActive(false);
            gm.player1Gold -= unit.cost;
        } else if (unit.playerNumber == 2 && unit.cost <= gm.player2Gold)
        {
            player2Menu.SetActive(false);
            gm.player2Gold -= unit.cost;
        } else {
            print("NOT ENOUGH GOLD, SORRY!");
            return;
        }

        gm.UpdateGoldText();
        gm.createdUnit = unit;

        DeselectUnit();
        SetCreatableTilesForUnits();
    }

    public void BuyVillage(Village village) {
        if (village.playerNumber == 1 && village.cost <= gm.player1Gold)
        {
            player1Menu.SetActive(false);
            gm.player1Gold -= village.cost;
        }
        else if (village.playerNumber == 2 && village.cost <= gm.player2Gold)
        {
            player2Menu.SetActive(false);
            gm.player2Gold -= village.cost;
        }
        else
        {
            print("NOT ENOUGH GOLD, SORRY!");
            return;
        }
        gm.UpdateGoldText();
        gm.createdVillage = village;

        DeselectUnit();

        SetCreatableTilesForVillages();

    }

    void SetCreatableTilesForUnits()
    {
        gm.ResetTiles();

        Village[] villages = FindObjectsOfType<Village>();

        foreach (Village village in villages)
        {
            if (village.playerNumber == gm.playerTurn)
            {
                Tile tile = village.lastTile;
                for (int i = 0; i < tile.arcs.Count; i++)
                {
                    if (tile.arcs[i].isClear() && !tile.arcs[i].GetSelected())
                        tile.arcs[i].SetCreatable();
                }
            }
        }
    }

    void SetCreatableTilesForVillages()
    {
        gm.ResetTiles();

        Village[] villages = FindObjectsOfType<Village>();

        foreach (Village village in villages)
        {
            if (village.playerNumber == gm.playerTurn)
            {
                Tile tile = village.lastTile;
                for (int i = 0; i < tile.arcs.Count; i++)
                {
                    if (tile.arcs[i].isClear() && !tile.arcs[i].GetSelected())
                        tile.arcs[i].SetCreatable();

                    for (int j = 0; j < tile.arcs[i].arcs.Count; j++)
                    {
                        if (tile.arcs[i].arcs[j].isClear() && !tile.arcs[i].arcs[j].GetSelected() && !tile.arcs[i].arcs[j].isCreatable)
                            tile.arcs[i].arcs[j].SetCreatable();
                    }
                }
            }
        }
    }

    void DeselectUnit() {
        if (gm.selectedUnit != null)
        {
            gm.selectedUnit.isSelected = false;
            gm.selectedUnit = null;
        }
    }




}
