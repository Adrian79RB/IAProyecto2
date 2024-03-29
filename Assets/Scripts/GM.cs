﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class GM : MonoBehaviour
{
    public Unit selectedUnit;
    public Village selectedVillage;

    public int playerTurn = 1;

    public Transform selectedUnitSquare;


    private Animator camAnim;
    public Image playerIcon; 
    public Sprite playerOneIcon;
    public Sprite playerTwoIcon;

    public GameObject unitInfoPanel;
    public Vector2 unitInfoPanelShift;
    Unit currentInfoUnit;
    Village currentInfoVillage;
    public Text heathInfo;
    public Text attackDamageInfo;
    public Text armorInfo;
    public Text defenseDamageInfo;

    public int player1Gold;
    public int player2Gold;

    public Text player1GoldText;
    public Text player2GoldText;

    public Unit createdUnit;
    public Village createdVillage;

    public GameObject blueVictory;
    public GameObject darkVictory;

    public Agent agent;

	private AudioSource source;

    public Village village;

    private void Start()
    {
		source = GetComponent<AudioSource>();
        camAnim = Camera.main.GetComponent<Animator>();
        GetGoldIncome(1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("b")) {
            EndTurn();
        }

        if (selectedUnit != null) // moves the white square to the selected unit!
        {
            selectedUnitSquare.gameObject.SetActive(true);
            selectedUnitSquare.position = selectedUnit.transform.position;
        }
        
        else
        {
            if(selectedVillage != null){
                selectedUnitSquare.gameObject.SetActive(true);
                selectedUnitSquare.position = selectedVillage.transform.position;
            }
            else{
                selectedUnitSquare.gameObject.SetActive(false);
            }
            
        }

    }

    // Sets panel active/inactive and moves it to the correct place
    public void UpdateInfoPanel(Unit unit) {

        if (unit.Equals(currentInfoUnit) == false)
        {
            unitInfoPanel.transform.position = (Vector2)unit.transform.position + unitInfoPanelShift;
            unitInfoPanel.SetActive(true);

            currentInfoUnit = unit;

            UpdateInfoStats();

        } else {
            if(currentInfoVillage == null){
                unitInfoPanel.SetActive(false);
            }
            
            currentInfoUnit = null;
        }

    }
    public void UpdateInfoPanVil(Village village){
        if(village.Equals(currentInfoVillage)== false){
            unitInfoPanel.transform.position =(Vector2)village.transform.position + unitInfoPanelShift;
            unitInfoPanel.SetActive(true);

            currentInfoVillage = village;
            UpdateInfoVillage();

        }
        else{
            if(currentInfoUnit==null){
                unitInfoPanel.SetActive(false);
            }
            
            currentInfoVillage = null;
        }
    }

    // Updates the stats of the infoPanel
    public void UpdateInfoStats() {
        if (currentInfoUnit != null)
        {
            attackDamageInfo.text = currentInfoUnit.attackDamage.ToString();
            defenseDamageInfo.text = currentInfoUnit.defenseDamage.ToString();
            armorInfo.text = currentInfoUnit.armor.ToString();
            heathInfo.text = currentInfoUnit.health.ToString();
        }
    }
    public void UpdateInfoVillage(){
        if (currentInfoVillage != null)
        {
            attackDamageInfo.text = "X";
            defenseDamageInfo.text = "X";
            armorInfo.text = "X";
            heathInfo.text = currentInfoVillage.health.ToString();
        }       
    }

    // Moves the udpate panel (if the panel is actived on a unit and that unit moves)
    public void MoveInfoPanel(Unit unit) {
        if (unit.Equals(currentInfoUnit))
        {
            unitInfoPanel.transform.position = (Vector2)unit.transform.position + unitInfoPanelShift;
        }
    }

    // Deactivate info panel (when a unit dies)
    public void RemoveInfoPanel(Unit unit) {
        if (unit.Equals(currentInfoUnit))
        {
            unitInfoPanel.SetActive(false);
			currentInfoUnit = null;
        }
    }

     public void RemoveInfoPanel(Village village) {
        if (village.Equals(currentInfoVillage))
        {
            unitInfoPanel.SetActive(false);
			currentInfoVillage = null;
        }
    }

    public void ResetTiles() {
        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles)
        {
            tile.Reset();
        }
    }

    public void EndTurn() {
		source.Play();
        camAnim.SetTrigger("shake");

        // deselects the selected unit when the turn ends
        if (selectedUnit != null) {
            selectedUnit.ResetWeaponIcon();
            selectedUnit.isSelected = false;
            selectedUnit = null;
            selectedVillage = null;
        }

        ResetTiles();

        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units) {
            unit.hasAttacked = false;
            unit.hasMoved = false;
            unit.ResetWeaponIcon();
        }

        if (playerTurn == 1) {
            playerIcon.sprite = playerTwoIcon;
            playerTurn = 2;
        } else if (playerTurn == 2) {
            playerIcon.sprite = playerOneIcon;
            playerTurn = 1;
        }

        GetGoldIncome(playerTurn);
        GetComponent<CharacterCreation>().CloseCharacterCreationMenus();
        createdUnit = null;
    }

    void GetGoldIncome(int playerTurn) {
        foreach (Village village in FindObjectsOfType<Village>())
        {
            if (village.playerNumber == playerTurn)
            {
                if (playerTurn == 1)
                {
                    player1Gold += village.goldPerTurn;
                }
                else
                {
                    player2Gold += village.goldPerTurn;
                }
            }
        }
        UpdateGoldText();
    }

    public void UpdateGoldText()
    {
        player1GoldText.text = player1Gold.ToString();
        player2GoldText.text = player2Gold.ToString();
    }

    // Victory UI

    public void ShowVictoryPanel(int playerNumber) {

        if (playerNumber == 1)
        {
            blueVictory.SetActive(true);
        } else if (playerNumber == 2) {
            darkVictory.SetActive(true);
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
