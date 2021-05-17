using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustColor : MonoBehaviour
{

    public enum ColorState
    {
        None,
        Pingpong,
        One,
        Inverse
    }

    private ColorState colorState = ColorState.None;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
