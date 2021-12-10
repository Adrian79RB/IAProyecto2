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
    public GameObject weaponIcon;
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
    public GameObject deathEffect;
    public DamageIcon damageIcon;
    public GM gm;
    public Unit unit;
    public bool isSelected;

    // Attack Stats
    public int health;
    public int attackDamage;
    public int defenseDamage;
    public int armor;

    private void Start()
    {
        gm = FindObjectOfType<GM>();
    }

    public void OnMouseDown()
    {
        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        Village village = col.GetComponent<Village>();
        if (gm.selectedUnit != null)
        {
            Debug.Log(gm.selectedUnit.villagesInRange[0]);
            Debug.Log(village);
            Debug.Log(gm.selectedUnit.villagesInRange.Contains(village));
            Debug.Log(gm.selectedUnit);
            if (gm.selectedUnit.villagesInRange.Contains(village))
            { // does the currently selected unit have in his list the enemy we just clicked on
                Debug.Log("Entra3");
                gm.selectedUnit.AttackVillage(village);

            }


        }
    }

}
