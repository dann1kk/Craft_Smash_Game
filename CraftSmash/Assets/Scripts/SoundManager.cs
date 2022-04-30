using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioClip jumping;
    [SerializeField] AudioSource audioSourceFX;
    
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        audioSourceFX.GetComponent<AudioSource>();
    }


    public void PlaySound(int index)
    {
        audioSourceFX.PlayOneShot(jumping);
    }
}
