using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject[] prefabsBlue;
    public GameObject[] prefabsRed;
    private Transform UnitsFather;

    //public int gridX = 38;
    //public int gridY = 34;
    //public float spacing = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        UnitsFather = GameObject.Find("Units").transform;

        for(int i = 0; i < prefabsBlue.Length; i++)
        {
            int indexUnit = Random.Range( 0, prefabsBlue.Length);
            Vector2 locationUnit = new Vector2(Random.Range(-27, 17), Random.Range(-16, 23));

            Instantiate(prefabsBlue[indexUnit], locationUnit, Quaternion.identity, UnitsFather);
        }

        for(int i = 0; i < prefabsRed.Length; i++)
        {
            int indexUnit = Random.Range( 0, prefabsRed.Length);
            Vector2 locationUnit = new Vector2(Random.Range(-27, 17), Random.Range(-16, 23));

            Instantiate(prefabsRed[indexUnit], locationUnit, Quaternion.identity, UnitsFather);
        }  

        /* for (int y = 0; y < gridY; y++) {
            for (int x = 0; x < gridX; x++) {
                Vector2 pos = new Vector2(x, y);// * spacing;
                Instantiate(prefabs[0], pos, Quaternion.identity);
            }
        } */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
