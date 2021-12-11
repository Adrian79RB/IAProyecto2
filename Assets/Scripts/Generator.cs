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

        int i = 0;

        while(i < prefabsBlue.Length)
        {
            Vector2 locationUnit = new Vector2(Mathf.RoundToInt(Random.Range(mapa.transform.position.x, mapa.transform.position.x + mapa.maxColumnas-1) * offSet), Mathf.RoundToInt(Random.Range(mapa.transform.position.y + mapa.maxFilas/2, mapa.transform.position.y + mapa.maxFilas-1) * offSet));
            Collider2D col = Physics2D.OverlapCircle(locationUnit, 0.3f, LayerMask.GetMask("Obstacle"));

            if(col == null)
            {
                Instantiate(prefabsBlue[i], locationUnit, Quaternion.identity, UnitsFather);
                i++;
            }
        }

        i = 0;

        while(i < prefabsRed.Length)
        {

            Vector2 locationUnit = new Vector2(Mathf.RoundToInt(Random.Range(mapa.transform.position.x, mapa.transform.position.x + mapa.maxColumnas-1) * offSet), Mathf.RoundToInt(Random.Range(mapa.transform.position.y, mapa.transform.position.y + (mapa.maxFilas/2)-1) * offSet));
            Collider2D col = Physics2D.OverlapCircle(locationUnit, 0.3f, LayerMask.GetMask("Obstacle"));

            if(col == null)
            {
                Instantiate(prefabsRed[i], locationUnit, Quaternion.identity, UnitsFather);
                i++;
            }
        }
    }
}
