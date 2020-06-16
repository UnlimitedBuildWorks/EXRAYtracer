using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Line_Script : MonoBehaviour
{
    private LineRenderer lRend;
    private Vector3 m_p1, m_p2;
    // Start is called before the first frame update
    void Start()
    {
        lRend = GetComponent<LineRenderer>();
        lRend.positionCount = 2;
        lRend.startWidth = lRend.endWidth = 0.01f;
        lRend.SetPosition(0, m_p1);
        lRend.SetPosition(1, m_p2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Vector3 p1, Vector3 p2)
    {
        m_p1 = p1;
        m_p2 = p2;
    }
}
