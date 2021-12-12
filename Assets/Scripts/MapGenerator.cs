using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct Tesela
{
    public int peso;
    public GameObject tile;
}

public class MapGenerator : MonoBehaviour
{
    
    public int maxColumnas;
    public int maxFilas;

    public Tesela[] tiles;
    public GameObject tree;
    public Transform treeFather;

    public Tile[,] teselas;


    public GameObject[] prefabsBlue;
    public GameObject[] prefabsRed;
    private Transform UnitsFather;

    public float offSet;

    // Start is called before the first frame update
    void Start()
    {
        UnitsFather = GameObject.Find("Units").transform;
        teselas = new Tile[maxFilas, maxColumnas];

        for(int i = 0; i < maxFilas; i++)
        {
            for(int j = 0; j < maxColumnas; j++)
            {
                Vector2 position = new Vector2 (transform.position.x + j * 1, transform.position.y + i * 1);
                int chosenIndex = weigthedRandom();
                teselas[i, j] = Instantiate(tiles[chosenIndex].tile, position, Quaternion.identity, transform).GetComponent<Tile>();

                if(tiles[chosenIndex].tile.name == "Tile 1" && UnityEngine.Random.value < 0.1)
                {
                    Instantiate(tree, position, Quaternion.identity, treeFather);
                }
                else if(tiles[chosenIndex].tile.name == "TileAgua")
                {
                    teselas[i, j].tag = "river";
                }
            }
        }

        int k = 0;

        while (k < prefabsBlue.Length)
        {
            Vector2 locationUnit = new Vector2(Mathf.RoundToInt(UnityEngine.Random.Range(transform.position.x, transform.position.x + maxColumnas - 1) * offSet), Mathf.RoundToInt(UnityEngine.Random.Range(transform.position.y + maxFilas / 2, transform.position.y + maxFilas - 1) * offSet));
            Collider2D col = Physics2D.OverlapCircle(locationUnit, 0.3f, LayerMask.GetMask("Obstacle"));

            if (col == null)
            {
                Instantiate(prefabsBlue[k], locationUnit, Quaternion.identity, UnitsFather);
                k++;
            }
        }

        k = 0;

        while (k < prefabsRed.Length)
        {

            Vector2 locationUnit = new Vector2(Mathf.RoundToInt(UnityEngine.Random.Range(transform.position.x, transform.position.x + maxColumnas - 1) * offSet), Mathf.RoundToInt(UnityEngine.Random.Range(transform.position.y, transform.position.y + (maxFilas / 2) - 1) * offSet));
            Collider2D col = Physics2D.OverlapCircle(locationUnit, 0.3f, LayerMask.GetMask("Obstacle"));

            if (col == null)
            {
                Instantiate(prefabsRed[k], locationUnit, Quaternion.identity, UnitsFather);
                k++;
            }
        }

        for (int i = 0; i < maxFilas; i++)
        {
            for (int j = 0; j < maxColumnas; j++)
            {
                if (i > 0) {
                    teselas[i, j].arcs.Add(teselas[i - 1, j]);

                    if (teselas[i - 1, j].tag == "river")
                        teselas[i, j].weigths.Add(10);
                    else
                        teselas[i, j].weigths.Add(1);
                }
                if (i < maxFilas - 1) {
                    teselas[i, j].arcs.Add(teselas[i + 1, j]);
                    if (teselas[i + 1, j].tag == "river")
                        teselas[i, j].weigths.Add(10);
                    else
                        teselas[i, j].weigths.Add(1);
                }
                if (j > 0)
                {
                    teselas[i, j].arcs.Add(teselas[i, j - 1]);
                    if (teselas[i, j - 1].tag == "river")
                        teselas[i, j].weigths.Add(10);
                    else
                        teselas[i, j].weigths.Add(1);
                }
                if (j < maxColumnas - 1)
                {
                    teselas[i, j].arcs.Add(teselas[i, j + 1]);
                    if (teselas[i, j + 1].tag == "river")
                        teselas[i, j].weigths.Add(10);
                    else
                        teselas[i, j].weigths.Add(1);
                }

                //Comprobar si la tesela esta ocupada por Unit
                if (!teselas[i, j].GetComponent<Tile>().isClear())
                {
                    teselas[i, j].GetComponent<Tile>().SetSelected(true);
                    Collider2D col = Physics2D.OverlapCircle(teselas[i, j].transform.position, 0.2f, LayerMask.GetMask("Obstacle"));
                    if (col.GetComponent<Unit>())
                    {
                        col.GetComponent<Unit>().lastTile = teselas[i, j].GetComponent<Tile>();
                    }

                    if (col.GetComponent<Village>())
                    {
                        col.GetComponent<Village>().lastTile = teselas[i, j].GetComponent<Tile>();
                    }
                }
            }
        }
    }

    private int weigthedRandom()
    {
        int max = tiles[0].peso;
        for(int i = 1; i < tiles.Length; i++)
        {
            max += tiles[i].peso;
        }
        int target = Mathf.RoundToInt(UnityEngine.Random.Range(0, max));
        int chosenNumber = 0;

        while(target > tiles[chosenNumber].peso && chosenNumber < tiles.Length)
        {
            target -= tiles[chosenNumber].peso;
            chosenNumber++;
        }
        return chosenNumber;
    }
}
