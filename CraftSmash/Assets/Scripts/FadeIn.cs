using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public Animator animator;

    public void OnFadeComplete()
    {
        animator.SetBool("isFading", false);
    }
}
