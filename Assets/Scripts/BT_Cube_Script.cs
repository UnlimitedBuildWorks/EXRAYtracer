using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Cube_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSize(Vector3 p1, Vector3 p2)
    {
        transform.position = new Vector3((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f, (p1.z + p2.z) / 2.0f);
        transform.localScale = new Vector3((p2.x - p1.x) / 1.0f, (p2.y - p1.y) / 1.0f, (p2.z - p1.z) / 1.0f);
    }
}
