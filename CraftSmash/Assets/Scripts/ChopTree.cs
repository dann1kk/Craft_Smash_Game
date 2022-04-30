using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class ChopTree : MonoBehaviour
{
    //References
    Animator animator;
    PlayerCharacterController player;

    // For chop animation
    [SerializeField]
    GameObject treeCenter;
    public float chopTime = 3f;
    float timer = 0f;
    bool isChopping = false;

    // Tree 
    public bool treeIsChopped = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacterController>();
        animator = GameObject.Find("Player").GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Tree chopping
        if (Input.GetKeyDown(KeyCode.E) && !player.isBowAiming)
        {
            if (player.equippedWeapon == "None" && treeIsChopped == false && player.chopTreeText.activeSelf == true && animator.GetCurrentAnimatorStateInfo(0).IsName("Running Blend Tree"))
            {
                Chop();
                animator.SetFloat("speedPercent", 0f);
                player.inputDir = Vector2.zero;
                player.velocity = Vector3.zero;
            }
            else
            {
                player.UnequipWeapons();
            }
        }
        ChopTimer();
    }

    void Chop()
    {
        //Range check to tree
        float distance = Vector3.Distance(player.transform.position, gameObject.transform.position);
        
        if (distance < 2.2f)
        {
            isChopping = true;
            player.transform.LookAt(new Vector3(treeCenter.transform.position.x, player.transform.position.y, treeCenter.transform.position.z));
            treeIsChopped = true;
        }
        player.canMove = false;
        animator.SetBool("isChopping", true);
        player.axeActive = true;         
    }

    void ChopTimer()
    {
        if (isChopping)
        {
            player.startChopSound = true;

            timer += Time.deltaTime;

            if (timer >= chopTime)
            {
                isChopping = false;
                timer = 0;
                animator.SetBool("isChopping", false);
                player.axeActive = false;
                player.canMove = true;
                float distance = Vector3.Distance(player.transform.position, treeCenter.transform.position);
                if (distance < 2.2f)
                {
                    Destroy(gameObject);
                    player.woodCount += 3;
                }
            }
        }
        
    }
}
