using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//tween 클래스는 조건절이 붙어있지 않습니다.

public class Tween<T>
{
    //시점에 대한 값
    private T start;

    //현재 값
    private T current;

    //종점에 대한 값
    private T end;

    //목표로 하는 시간
    private float speed;

    private float elapsedTime;

    //업데이트가 완료된 상태를 가리키는 변수
    private bool isUpdate;

    //외부로부터 연산된 함수를 받아오기 위한 델리게이트
    //1.시작점 2.종점 3.시간 4.리턴타입
    // 으로 구성된 함수를 받아오겠다는 의미
    private System.Func<T, T, float, T> action;


    public bool IsEnd { get => isUpdate; }

    public void SetEnd(bool state)
    {
        this.isUpdate = state;
    }

    public void SetTween(System.Func<T,T,float,T> func)
    {
        action = func;
    }

    public void Execute(T start, T end, float speed)
    {
        this.start = start;
        this.end = end;
        this.speed = speed;
        elapsedTime = 0f;
        isUpdate = false;
    }


    public T Update()
    {
        //업데이트가 완료된 상태라면
        if (isUpdate)
            return end;

        if(action != null)
        {
            elapsedTime += Time.deltaTime / speed;
            //경과된 시간값을 Clamp01을 사용하여 0과 1사이의 값으로 맞춰준다.
            elapsedTime =  Mathf.Clamp01(elapsedTime);

            current = action(start, end, elapsedTime);

            //1.0초가 경과되면 업데이트 종료
            if (elapsedTime >= 1.0f)
                isUpdate = true;
        }

        return current;
    }
}
