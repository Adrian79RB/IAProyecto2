using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{

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
}
