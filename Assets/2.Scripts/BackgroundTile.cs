using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public float positionX;
    public float positionY;

    public void Init(float x, float y)
    {
        positionX = x;
        positionY = y;
    }
}
