using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    public GameObject weaponIcon;
    public int goldPerTurn;
    public int playerNumber;
    public int cost;
    public int health;
   
    public GameObject deathEffect;
    public DamageIcon damageIcon;
    public Unit unit;
    public bool isSelected;

    public Tile lastTile;

    // Attack Stats
    public int armor;

    private GM gm;

    public void Start(){
        gm = FindObjectOfType<GM>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gm.UpdateInfoPanVil(this);
        }
    }

    public void OnMouseDown()
    {
        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        if (col != null)
        {
            Village village = col.GetComponent<Village>();
            if (village != null && gm.selectedUnit != null && gm.selectedUnit.tag != "Ariete")
            { // does the currently selected unit have in his list the enemy we just clicked on
                if(gm.selectedUnit.enemyVillages.Contains(village) && !gm.selectedUnit.hasAttacked)
                    gm.selectedUnit.AttackVillage(village);
            }
        }
    }

}
