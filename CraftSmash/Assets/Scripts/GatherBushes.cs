using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class GatherBushes : MonoBehaviour
{
    //References
    Animator animator;
    PlayerCharacterController player;

    [SerializeField]
    GameObject lookAtPosition;
    public float gatherTime = 3f;
    float timer = 0f;
    bool isGathering = false;
    float gatherTimer = 0;

    public bool bushIsGathered = false;

    //Audio
    AudioSource audioSourceFX;
    [SerializeField] AudioClip audioClip;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacterController>();
        animator = GameObject.Find("Player").GetComponentInChildren<Animator>();
        audioSourceFX = GetComponent<AudioSource>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E) && bushIsGathered == false && !player.isBowAiming)
        {
            if (player.equippedWeapon == "None" && bushIsGathered == false && player.gatherBushText.activeSelf && animator.GetCurrentAnimatorStateInfo(0).IsName("Running Blend Tree"))
            {
                Gather();
                player.startBushSound = true;
                animator.SetFloat("speedPercent", 0f);
                player.inputDir = Vector2.zero;
                player.velocity = Vector3.zero;
            }
            else
            {
                player.UnequipWeapons();
            }
        }
        GatherTimer();
    }

    void Gather()
    {
        float distance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        if (distance < 2f)
        {
            isGathering = true;
            player.transform.LookAt(new Vector3(lookAtPosition.transform.position.x, player.transform.position.y, lookAtPosition.transform.position.z));
            bushIsGathered = true;
        }
        player.canMove = false;
        animator.SetBool("isGathering", true);
    }

    void GatherTimer()
    {
        if (isGathering)
        {
            timer += Time.deltaTime;

            

            if (timer >= gatherTime)
            {
                isGathering = false;
                timer = 0;
                animator.SetBool("isGathering", false);
                player.canMove = true;
                float distance = Vector3.Distance(player.transform.position, lookAtPosition.transform.position);
                if (distance < 2f)
                {
                    Destroy(gameObject);
                    player.leafCount += 2;
                }
            }
        }

    }
}

