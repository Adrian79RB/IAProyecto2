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

    private GameObject[,] teselas;



    // Start is called before the first frame update
    void Start()
    {
        teselas = new GameObject[maxFilas, maxColumnas];
        for(int i = 0; i < maxFilas; i++)
        {
            for(int j = 0; j < maxColumnas; j++)
            {
                Vector2 position = new Vector2 (transform.position.x + j * 1, transform.position.y + i * 1);
                int chosenIndex = weigthedRandom();
                Instantiate(tiles[chosenIndex].tile, position, Quaternion.identity, transform);
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
