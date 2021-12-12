using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y - 6, Input.mousePosition.z);
        transform.position = pos;
    }
}
