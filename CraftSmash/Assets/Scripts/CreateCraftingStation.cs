using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCraftingStation : MonoBehaviour
{
    //References
    Animator animator;
    PlayerCharacterController player;

    //Variables
    public bool stationCreated = false;
    public bool isCreatingStation = false;
    float timer = 0f;
    float createTime = 3f;
    Vector3 stationEndPosition;
    Vector3 stationStartPosition;
    float creationSpeed = 0.7f;
    [SerializeField] GameObject createStationTrigger;
    [SerializeField] ParticleSystem smokeParticle;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacterController>();
        animator = GameObject.Find("Player").GetComponentInChildren<Animator>();
        stationEndPosition = gameObject.transform.position;
        stationStartPosition = new Vector3(28f, 19.53f, 134.45f);
        gameObject.transform.position = stationStartPosition;
        smokeParticle.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        CreateStation();
        CreateTimer();
    }

    void CreateStation()
    {
        float distance = Vector3.Distance(player.transform.position, createStationTrigger.transform.position);

        //Range and Activation
        if (distance < 2f && stationCreated == false)
        {
            player.createStationText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E) && HasMaterialsToCraft())
            {
                isCreatingStation = true;
                smokeParticle.Play();
                player.canMove = false;
                player.hammerActive = true;
                animator.SetBool("isChopping", true);
                player.createStationText.SetActive(false);
                stationCreated = true;
                player.woodCount -= 4;
                player.ironCount -= 3;
            }
        }
        else
        {
            player.createStationText.SetActive(false);
        }

        // Station is building
        if (isCreatingStation)
        {
            player.startChopSound = true;
            if (gameObject.transform.position.y < stationEndPosition.y)
            {
                gameObject.transform.position += new Vector3(0f, creationSpeed * Time.deltaTime, 0f);
            }
            else
            {
                
                smokeParticle.Stop();
            }
        }
    }

    void CreateTimer()
    {
        if (isCreatingStation)
        {
            timer += Time.deltaTime;

            if (timer >= createTime)
            {
                timer = 0;
                animator.SetBool("isChopping", false);
                player.hammerActive = false;
                player.canMove = true;
                isCreatingStation = false;
            }
        }

    }

    bool HasMaterialsToCraft()
    {
        // Need 4 wood and 3 iron to build crafting station
        if (player.woodCount >= 4 && player.ironCount >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
