using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffect : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void RemoveEffect(float time = 3f)
    {
        Invoke(nameof(DestroyEffect), time);
    }

    private void DestroyEffect()
    {
        ObjectPool.ReturnObjectToPool<BombEffect>(this.gameObject);
    }


}
