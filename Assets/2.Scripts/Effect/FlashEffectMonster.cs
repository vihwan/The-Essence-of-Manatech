using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffectMonster : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
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
        ObjectPool.ReturnObjectToPool<FlashEffectMonster>(this.gameObject);
    }
}