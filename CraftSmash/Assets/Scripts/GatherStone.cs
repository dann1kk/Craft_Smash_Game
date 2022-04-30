using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class GatherStone : MonoBehaviour
{
    //References
    Animator animator;
    PlayerCharacterController player;

    [SerializeField]
    GameObject lookAtPosition;
    public float gatherTime = 3f;
    float timer = 0f;
    bool isGathering = false;

    public bool stoneIsGathered = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacterController>();
        animator = GameObject.Find("Player").GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E) && stoneIsGathered == false && !player.isBowAiming)
        {
            if (player.equippedWeapon == "None" && stoneIsGathered == false && player.gatherStoneText.activeSelf && animator.GetCurrentAnimatorStateInfo(0).IsName("Running Blend Tree"))
            {
                Gather();
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
            stoneIsGathered = true;
            
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
                    player.stoneCount += 1;
                }
            }
        }

    }
}
