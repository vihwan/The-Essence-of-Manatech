using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPingpong : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float pingpong = Mathf.PingPong(Time.time * 5000, 20f);
        Vector3 prevPos = transform.position;
        this.transform.position = new Vector3(pingpong, prevPos.y, prevPos.z);
    }
}
