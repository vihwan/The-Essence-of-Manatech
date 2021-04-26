using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    //이 위치값은 한번 초기화가 된다면, 값이 더이상 바뀌지 않아야 합니다.
    public float positionX;
    public float positionY;

    public void Init(float x, float y)
    {
        positionX = x;
        positionY = y;
    }
}
