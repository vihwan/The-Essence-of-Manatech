using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPingpong : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        /*        float pingpong = Mathf.PingPong(Time.time * 5000, 20f);
                Vector3 prevPos = transform.position;
                this.transform.position = new Vector3(pingpong, prevPos.y, prevPos.z);*/

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Screen Width : " + Screen.width + ", Screen Height : " + Screen.height);

            foreach (var item in Screen.resolutions)
            {
                print("Screen Resolutions : " + item);

            }
            print("Screen Current Resolutions : " + Screen.currentResolution);
        }
    }
}
