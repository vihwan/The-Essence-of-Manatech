using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("flash");
    }
}
