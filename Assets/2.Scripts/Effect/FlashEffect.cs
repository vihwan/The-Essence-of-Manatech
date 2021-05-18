using System;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void RemoveEffect(float time = 3f)
    {
        Invoke(nameof(DestroyEffect), time);
    }

    private void DestroyEffect()
    {
        ObjectPool.ReturnObjectToPool<FlashEffect>(this.gameObject);
    }
}
