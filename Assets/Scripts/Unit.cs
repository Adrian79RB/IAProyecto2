using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public bool isSelected;
    public bool hasMoved;

    public int tileSpeed;
    public float moveSpeed;

    public GM gm;

    public int attackRadius;
    public bool hasAttacked;
    public List<Unit> enemiesInRange = new List<Unit>();
    public List<Village> enemyVillages = new List<Village>();
    public List<Village> villagesInRange = new List<Village>();

    public int playerNumber;

    public GameObject weaponIcon;

    // Attack Stats
    public int health;
    public int attackDamage;
    public int defenseDamage;
    public int armor;

    public DamageIcon damageIcon;

    public int cost;

	public GameObject deathEffect;
    public GameObject[] trees; 

	private Animator camAnim;

    public bool isKing;

    public Tile lastTile;

	private AudioSource source;
    private List<Tile> walkableTiles;
    private Tile[] tiles;

    public Text displayedText;

    private Village village;

    private void Start()
    {
		source = GetComponent<AudioSource>();
		camAnim = Camera.main.GetComponent<Animator>();
        tiles = FindObjectsOfType<Tile>();
        gm = FindObjectOfType<GM>();
        
        
            source = GetComponent<AudioSource>();
            camAnim = Camera.main.GetComponent<Animator>();
        

        UpdateHealthDisplay();
        village = FindObjectOfType<Village>();
        trees = GameObject.FindGameObjectsWithTag("Tree");
    }

    private void UpdateHealthDisplay ()
    {
        if (isKing)
        {
            displayedText.text = health.ToString();
        }
    }

    private void OnMouseDown() // select character or deselect if already selected
    {
        
        ResetWeaponIcon();

        if (isSelected == true)
        {
            
            isSelected = false;
            gm.selectedUnit = null;
            gm.ResetTiles();

        }

        else {
            if (playerNumber == 1) { // select unit only if it's his turn
                if (gm.selectedUnit != null)
                { // deselect the unit that is currently selected, so there's only one isSelected unit at a time
                    gm.selectedUnit.isSelected = false;
                }
              
                

                gm.ResetTiles();

                gm.selectedUnit = this;

                isSelected = true;
				if(source != null){
					source.Play();
				}
				
                GetWalkableTiles();
                if(transform.tag == "Siege"){
                    GetVillages();
                }
                else{
                    GetEnemies();
                }
                
            }

           

        }



        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        Debug.Log("Esta unidad: " + this.name + "; Nombre de la unidad selccionada" + gm.selectedUnit.name);
        Debug.Log(villagesInRange.Count);
        Debug.Log(gm.selectedUnit.villagesInRange.Count);
        if (col != null)
        {
            if (gm.selectedUnit.tag == "Ariete")
                gm.selectedUnit.hasAttacked = false;
            Debug.Log("Entra1");
            Unit unit = col.GetComponent<Unit>(); // double check that what we clicked on is a unit
            
            //Village village = col.GetComponent<Village>();
            if (unit != null && gm.selectedUnit != null && gm.selectedUnit.tag != "Ariete")
            {
                if (gm.selectedUnit.enemiesInRange.Contains(unit) && !gm.selectedUnit.hasAttacked )
                { // does the currently selected unit have in his list the enemy we just clicked on
                    gm.selectedUnit.Attack(unit);

                }
                
               
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && playerNumber == 1)
        {
            gm.UpdateInfoPanel(this);
        }
    }


    public List<Tile> GetTilesArray()
    {
        ResetWeaponIcon();

        if (gm.selectedUnit != null)
        {
            gm.selectedUnit.isSelected = false;
        }
        //gm.ResetTiles();
        gm.selectedUnit = this;

        isSelected = true;
        if (source != null)
        {
            source.Play();
        }

        walkableTiles = new List<Tile>();
        GetWalkableTiles();
        GetEnemies();
        return walkableTiles;
    }


    void GetWalkableTiles() { // Looks for the tiles the unit can walk on
        if (hasMoved == true) {
            return;
        }

        foreach (Tile tile in tiles) {
            if (Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed)
            { // how far he can move
                if (tile.isClear() && !tile.GetSelected())
                { // is the tile clear from any obstacles
                    tile.Highlight();
                    if(playerNumber == 2)
                        walkableTiles.Add(tile);
                }
            }
        }
    }

    public void GetEnemies() {
    
        enemiesInRange.Clear();

        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= attackRadius) // check is the enemy is near enough to attack
            {
                if (enemy.playerNumber != gm.playerTurn && !hasAttacked && gm.selectedUnit.tag != "Ariete") { // make sure you don't attack your allies
                    enemiesInRange.Add(enemy);
                    enemy.weaponIcon.SetActive(true);
                }
            }
        }
    }

    void GetVillages(){
        enemyVillages.Clear();

        Village[] villages = FindObjectsOfType<Village>();
        foreach (Village village in villages){
            if(Mathf.Abs(transform.position.x - village.transform.position.x)+Mathf.Abs(transform.position.y - village.transform.position.y)<= attackRadius){
                if(village.playerNumber != gm.playerTurn && !hasAttacked){
                    enemyVillages.Add(village);
                    village.weaponIcon.SetActive(true);
                }
            }
        }
    }

    public void Move(Transform movePos, int unitIndex)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(movePos, unitIndex));
    }

    public void Attack(Unit enemy) {
        hasAttacked = true;
        int enemyDamege = attackDamage - enemy.armor;
        int unitDamage = enemy.defenseDamage - armor;
        if(transform.tag == "Archer"){
            if(enemy.tag == "Knight"){
                enemyDamege = enemyDamege - enemy.armor;
            }
        }
        if(transform.tag == "Knight"){
            if(enemy.tag == "Bat"){
                enemyDamege=0;
            }
            if(enemy.tag == "Archer"){
                unitDamage = unitDamage - armor;
            }
        }
        if(transform.tag == "Bat"){
            if(enemy.tag == "Knight"){
                unitDamage = 0;
            }
        }
        if(transform.tag == "King"){

        }
        
        

        if (enemyDamege >= 1 && transform.tag != "Ariete")
        {
            if (transform.tag == "Archer")
                for (int i = 0; i < trees.Length; i++)
                    if ((Mathf.Round(trees[i].transform.position.x) == transform.position.x && trees[i].transform.position.y >= transform.position.y && trees[i].transform.position.y <= enemy.transform.position.y) || (Mathf.Round(trees[i].transform.position.x) == transform.position.x && trees[i].transform.position.y <= transform.position.y && trees[i].transform.position.y >= enemy.transform.position.y))
                        enemyDamege--;
                    else if ((Mathf.Round(trees[i].transform.position.y) == transform.position.y && trees[i].transform.position.x >= transform.position.x && trees[i].transform.position.x <= enemy.transform.position.x) || (Mathf.Round(trees[i].transform.position.y) == transform.position.y && trees[i].transform.position.x <= transform.position.x && trees[i].transform.position.x >= enemy.transform.position.x))
                        enemyDamege--;


            enemy.health -= enemyDamege;
            enemy.UpdateHealthDisplay();
            DamageIcon d = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            d.Setup(enemyDamege);
        }

        if (transform.tag == "Archer" && enemy.tag != "Ariete")
        {
            for (int i = 0; i < trees.Length; i++)
                if (trees[i].transform.position.x == transform.position.x || trees[i].transform.position.y == transform.position.y)
                    enemyDamege--;
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= 1) // check is the enemy is near enough to attack
            {
                if (unitDamage >= 1)
                {
                    health -= unitDamage;
                    UpdateHealthDisplay();
                    DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
                    d.Setup(unitDamage);
                }
            }
        } else {
            if (unitDamage >= 1)
            {
                health -= unitDamage;
                UpdateHealthDisplay();
                DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
                d.Setup(unitDamage);
            }
        }

        if (enemy.health <= 0)
        {
            if(gm.playerTurn == 2)
            {
                gm.agent.enemiesKilled++;
            }

            if (deathEffect != null){
				Instantiate(deathEffect, enemy.transform.position, Quaternion.identity);
				camAnim.SetTrigger("shake");
			}

            if (enemy.isKing)
            {
                gm.ShowVictoryPanel(enemy.playerNumber);
            }

            GetWalkableTiles(); // check for new walkable tiles (if enemy has died we can now walk on his tile)
            gm.RemoveInfoPanel(enemy);
            enemy.lastTile.SetSelected(false);
            enemy.lastTile = null;
            Destroy(enemy.gameObject);
        }

        if (health <= 0)
        {

            if (deathEffect != null)
			{
				Instantiate(deathEffect, enemy.transform.position, Quaternion.identity);
				camAnim.SetTrigger("shake");
			}

			if (isKing)
            {
                gm.ShowVictoryPanel(playerNumber);
            }

            gm.ResetTiles(); // reset tiles when we die
            gm.RemoveInfoPanel(this);
            lastTile.SetSelected(false);
            lastTile = null;
            Destroy(gameObject);
        }

        gm.UpdateInfoStats();
  

    }
    void AttackVillage(Village village){
        hasAttacked = true;
        int damage = attackDamage;
        DamageIcon d = Instantiate(damageIcon, village.transform.position, Quaternion.identity);
        d.Setup(damage);

        if(village.health <= 0){
            if (deathEffect != null){
				Instantiate(deathEffect, village.transform.position, Quaternion.identity);
				camAnim.SetTrigger("shake");
			}
            GetWalkableTiles();
            gm.RemoveInfoPanel(village);
            Destroy(village.gameObject);
        }
    }

    public void AttackVillage(Village village)
    {
        hasAttacked = true;

        int villageDamege = attackDamage - village.armor;
        int unitDamage = village.defenseDamage - armor;
        Debug.Log("Quiere atacar");
        if (transform.tag == "Ariete")
        {
            Debug.Log("Entra");
            if (villageDamege >= 1)
            {
                village.health -= villageDamege;
                DamageIcon d = Instantiate(damageIcon, village.transform.position, Quaternion.identity);
                d.Setup(villageDamege);
            }

            else
            {
                if (unitDamage >= 1)
                {
                    health -= unitDamage;
                    UpdateHealthDisplay();
                    DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
                    d.Setup(unitDamage);
                }
            }

            if (village.health <= 0)
            {

                if (deathEffect != null)
                {
                    Instantiate(deathEffect, village.transform.position, Quaternion.identity);
                    camAnim.SetTrigger("shake");
                }


                GetWalkableTiles(); // check for new walkable tiles (if enemy has died we can now walk on his tile)
                Destroy(village.gameObject);

            }

            if (health <= 0)
            {

                if (deathEffect != null)
                {
                    Instantiate(deathEffect, village.transform.position, Quaternion.identity);
                    camAnim.SetTrigger("shake");
                }

                gm.ResetTiles(); // reset tiles when we die
                gm.RemoveInfoPanel(this);
                Destroy(gameObject);
            }

            gm.UpdateInfoStats();
        }

    }


    public void ResetWeaponIcon() {
        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            enemy.weaponIcon.SetActive(false);
        }
        Village[] villages = FindObjectsOfType<Village>();
        foreach(Village village in villages){
            village.weaponIcon.SetActive(false);
        }
    }

    IEnumerator StartMovement(Transform movePos, int unitIndex)
    { // Moves the character to his new position.
        if (playerNumber == 2)
        {
            yield return new WaitForSeconds(0.1f * unitIndex);
        }

        while (transform.position.x != movePos.position.x)
        { // first aligns him with the new tile's x pos
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(movePos.position.x, transform.position.y), moveSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        while (transform.position.y != movePos.position.y) // then y
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, movePos.position.y), moveSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        hasMoved = true;
        ResetWeaponIcon();
        GetEnemies();
        GetVillages();
        gm.MoveInfoPanel(this);
        
        transform.position = movePos.position;
    }

    void GetVillages()
    {
         
        villagesInRange.Clear();
        Village[] villages = FindObjectsOfType<Village>();
        foreach (Village village in villages)
        {
            if (Mathf.Abs(transform.position.x - village.transform.position.x) + Mathf.Abs(transform.position.y - village.transform.position.y) <= attackRadius) // check is the enemy is near enough to attack
            {
                if (village.playerNumber != gm.playerTurn && !hasAttacked && gm.selectedUnit.tag == "Ariete")
                {
                    // make sure you don't attack your allies
                    villagesInRange.Add(village);
                    village.weaponIcon.SetActive(true);
                    Debug.Log(gm.selectedUnit.villagesInRange.Contains(village));
                }
                

            }
        }
    }



}
