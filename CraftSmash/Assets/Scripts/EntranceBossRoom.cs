using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceBossRoom : MonoBehaviour
{
    public GameObject enterBossRoomTooltip;
    bool canTeleport = false;
    bool isTeleporting = false;
    PlayerCharacterController player;
    public GameObject playerSpawn;
    public float timer = 0;
    public bool isTeleportet = false;

    //Fading
    public Animator animator;
    
    public void Start()
    {
        enterBossRoomTooltip.SetActive(false);
        player = player = GameObject.Find("Player").GetComponent<PlayerCharacterController>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.E) && canTeleport == true)
        {
            isTeleporting = true;
        }

        if (isTeleporting)
        {
            FadeOut();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        enterBossRoomTooltip.SetActive(true);
        canTeleport = true;
    }

    private void OnTriggerExit(Collider other)
    {
        enterBossRoomTooltip.SetActive(false);
        canTeleport = false;
    }

    void FadeOut()
    {
        animator.SetBool("isFading", true);
        if (isTeleporting)
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                player.transform.position = playerSpawn.transform.position;
                isTeleporting = false;
                timer = 0f;
            }
        }
    }

}
