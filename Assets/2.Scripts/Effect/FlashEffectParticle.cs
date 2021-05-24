using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffectParticle : MonoBehaviour
{

    private ParticleSystem particle;

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    //public void RemoveEffect(float time = 3f)
    //{
    //    Invoke(nameof(DestroyEffect), time);
    //}

    //private void DestroyEffect()
    //{
    //    ObjectPool.ReturnObjectToPool<FlashEffectParticle>(this.gameObject);
    //}
}
