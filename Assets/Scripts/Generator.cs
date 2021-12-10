using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject[] prefabsBlue;
    public GameObject[] prefabsRed;
    private Transform UnitsFather;

    private MapGenerator mapa;

    public float offSet;


    // Start is called before the first frame update
    void Start()
    {
        mapa = GameObject.Find("Mapa").GetComponent<MapGenerator>();

        UnitsFather = GameObject.Find("Units").transform;

        for(int i = 0; i < prefabsBlue.Length; i++)
        {
            int indexUnit = Random.Range( 0, prefabsBlue.Length);
            Vector2 locationUnit = new Vector2(Mathf.RoundToInt(Random.Range(mapa.transform.position.x, mapa.transform.position.x + mapa.maxColumnas-1) * offSet), Mathf.RoundToInt(Random.Range(mapa.transform.position.y + mapa.maxFilas/2, mapa.transform.position.y + mapa.maxFilas-1) * offSet));

            Instantiate(prefabsBlue[indexUnit], locationUnit, Quaternion.identity, UnitsFather);
        }

        for(int i = 0; i < prefabsRed.Length; i++)
        {

            int indexUnit = Random.Range( 0, prefabsRed.Length);
            Vector2 locationUnit = new Vector2(Mathf.RoundToInt(Random.Range(mapa.transform.position.x, mapa.transform.position.x + mapa.maxColumnas-1) * offSet), Mathf.RoundToInt(Random.Range(mapa.transform.position.y, mapa.transform.position.y + (mapa.maxFilas/2)-1) * offSet));

            Instantiate(prefabsRed[indexUnit], locationUnit, Quaternion.identity, UnitsFather);
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
