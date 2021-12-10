using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer rend;
    public Color highlightedColor;
    public Color creatableColor;

    public LayerMask obstacles;

    public bool isWalkable;
    public bool isCreatable;
    private bool isSelected;

    //public List<GameObject> obst = new List<GameObject>();


    private GM gm;

    public float amount;
    private bool sizeIncrease;

    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        gm = FindObjectOfType<GM>();
        rend = GetComponent<SpriteRenderer>();
    }

    public bool isClear() // does this tile have an obstacle on it. Yes or No?
    {

        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f, obstacles);
        if (col == null)
        {
            //Debug.Log(isWalkable);
            return true;
        }
        else {
            isWalkable = false;
            return false;
        }
    }

    public void Highlight() {
		
        rend.color = highlightedColor;
        isWalkable = true;
    }

    public void Reset()
    {
        rend.color = Color.white;
        isWalkable = false;
        isCreatable = false;
    }

    public void SetCreatable() {
        rend.color = creatableColor;
        isCreatable = true;
    }

    public void SetSelected(bool state)
    {
        isSelected = state;
    }
    
    public bool GetSelected()
    {
        return isSelected;
    }

    private void OnMouseDown()
    {
        if (isWalkable == true) {
            gm.selectedUnit.lastTile.isSelected = false;
            isSelected = true;
            gm.selectedUnit.lastTile = this;
            gm.selectedUnit.Move(this.transform, -1);
        } else if (isCreatable == true && gm.createdUnit != null) {
            Unit unit = Instantiate(gm.createdUnit, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            unit.hasMoved = true;
            unit.hasAttacked = true;
            gm.ResetTiles();
            gm.createdUnit = null;
        } else if (isCreatable == true && gm.createdVillage != null) {
            Instantiate(gm.createdVillage, new Vector3(transform.position.x, transform.position.y, 0) , Quaternion.identity);
            gm.ResetTiles();
            gm.createdVillage = null;
        }

        
    }


    private void OnMouseEnter()
    {
        /*for (int i = 0; i < obst.Count; i++)
        {
            //Tile .obdt. GM 

            if (Mathf.Round(obst[i].transform.position.x) == transform.position.x && Mathf.Round(obst[i].transform.position.y) >= transform.position.y && Mathf.Round(obst[i].transform.position.y) <= gm.transform.position.y)
            {
                Debug.Log("En el if 1");
               // if (gm.transform.position.y >= Mathf.Round(obst[i].transform.position.y))
                
                    isWalkable = false;
                    Debug.Log("En el if 1.2");
                    Debug.Log("Elemento: " + obst[i] + "Posicion obst: " + obst[i].transform.position + " Posicion tile: " + transform.position);
                    Debug.Log(isWalkable);
                

                //else isWalkable = true;

            }

            else if (Mathf.Round(obst[i].transform.position.x) == transform.position.x && obst[i].transform.position.y <= transform.position.y && obst[i].transform.position.y >= gm.transform.position.y)
            {
                if (transform.position.y >= obst[i].transform.position.y + 1)
                {
                    isWalkable = false;
                    Debug.Log("En el if 2");
                }
            }

            else if (Mathf.Round(obst[i].transform.position.y) == transform.position.y && obst[i].transform.position.x >= transform.position.x && obst[i].transform.position.x <= gm.transform.position.x)
            {
                if (transform.position.x >= obst[i].transform.position.x + 1)
                {
                    isWalkable = false;
                    Debug.Log("En el if 3");
                }
            }


            else if (Mathf.Round(obst[i].transform.position.y) == transform.position.y && obst[i].transform.position.x <= transform.position.x && obst[i].transform.position.x >= gm.transform.position.x)
            {
                if (transform.position.x >= obst[i].transform.position.x + 1)
                {
                    isWalkable = false;
                    Debug.Log("En el if 4");
                }
            }
        
        }*/

        if (isClear() == true) {
			source.Play();
			sizeIncrease = true;
            transform.localScale += new Vector3(amount, amount, amount);
        }
        
    }

    private void OnMouseExit()
    {

        if (isClear() == true)
        {
            sizeIncrease = false;
            transform.localScale -= new Vector3(amount, amount, amount);
        }

        if (isClear() == false && sizeIncrease == true) {
            sizeIncrease = false;
            transform.localScale -= new Vector3(amount, amount, amount);
        }
    }
}
