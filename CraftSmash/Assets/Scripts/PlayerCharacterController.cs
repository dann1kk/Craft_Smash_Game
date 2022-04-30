using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterController: MonoBehaviour
{
    //Player Values
    [Header("Health Settings")]
    public float maxHealthPoints = 50;
    public float currentHealthPoints;

    [Header("Player States")]
    public bool isAlive = true;
    public bool canInteract = false;
    public bool isGrounded = false;
    public float interactRange = 2f;
    public bool hasWon = false;
    public bool escapeMenu = false;
    public bool craftingMenu = false;
    public bool introTutorial = true;
    

    //Movement
    [Header("Movement")]
    public Vector2 inputDir;
    public Vector3 velocity;
    public bool canMove = true;
    public float walkSpeed = 2;
    public float runSpeedDefault = 6;
    public float runSpeed = 6;
    public float gravity = -12;
    public float jumpHeight = 1;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;
    public bool strafeLeft = false;
    public bool strafeRight = false;

    float steptimer = 0;

    //Bow Aiming Movement
    private Vector3 moveDirection = Vector3.zero;
    public float horizontalMovement = 0f;
    public float verticalMovement = 0f;


    //Inventory
    [Header("Inventory")]
    public int woodCount = 0;
    public int stoneCount = 0;
    public int leafCount = 0;
    public int ironCount = 0;
    public bool hasCraftedSword = false;
    public bool hasCraftedBow = false;
    public int arrowCount = 0;
    public bool noArrowText = false;
    float messageTimer = 0;
    float gatherTimer = 0;

    //Tools
    [Header("Tools")]
    [SerializeField] GameObject axe;
    [SerializeField] GameObject hammer;

    [SerializeField] GameObject sword;
    [SerializeField] GameObject bow;

    [Header("Interface")]
    public GameObject chopTreeText;
    public GameObject gatherStoneText;
    public GameObject gatherBushText;
    public GameObject gatherIronText;
    public GameObject createStationText;
    float chopTreeTimer = 0.2f;
    public int chopCount = 0;


    // Combat States and Weapons
    [Header("Combat")]
    public string equippedWeapon = "None";
    public bool isWeaponEquipped = false;
    public bool axeActive = false;
    public bool hammerActive = false;
    public bool swordActive = false;
    public bool bowActive = false;
    public bool isSwordAttacking = false;
    public float timerSwordActive = 0;
    public float attackTimer = 0;
    public bool isBowAiming = false;
    public float bowShootTimer = 0;
    float swordAttackTimer = 0;

    //References
    Animator animator;
    Transform cameraT;
    CharacterController controller;
    BossController boss;

    //Audio
    AudioSource audioSourceFX;
    [SerializeField] AudioClip[] audioClips;
    float musicVolume;
    float soundEffectVolume = 1f;
    public bool startChopSound = false;
    public bool startBushSound = false;
    public float bushSoundTimer = 0;

    public bool startBossAttackSound = false;



    // Gathering arrays for interface elements
    GameObject[] trees;
    GameObject[] stones;
    GameObject[] bushes;
    GameObject[] iron;

    void Start()
    {
        hasCraftedBow = true;
        hasCraftedSword = true;

        arrowCount += 20;
        // References
        animator = GetComponentInChildren<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        boss = GameObject.Find("Boss").GetComponent<BossController>();
        audioSourceFX = GetComponent<AudioSource>();

        // Gathering arrays for interface elements
        trees = GameObject.FindGameObjectsWithTag("Tree");
        stones = GameObject.FindGameObjectsWithTag("Stone");
        bushes = GameObject.FindGameObjectsWithTag("Bush");
        iron = GameObject.FindGameObjectsWithTag("Iron");

        //Initialize health
        currentHealthPoints = maxHealthPoints;
    }

    void Update()
    {
        //Movement
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = input.normalized;
        Move(inputDir);

        // animator
        float animationSpeedPercent = currentSpeed / runSpeed;
        if (canMove)
        {
            animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
        }
        swordAttackTimer += Time.deltaTime;

        Jump();
        CheckForGrounded();

        AxeActive();
        HammerActive();
        ChangeWeapon();
        SwordActive();
        BowActive();
        SwordAttack();
        NoArrowTimer();

        CanInteract();
        CheckForTrees();
        CheckForStones();
        CheckForBushes();
        CheckForIron();

        Die();
        Win();

        EscapeMenu();

        chopSound();
        GatherBushSound();
        BossAttackSound();

    }

    void Move(Vector2 inputDir)
    {
        //Gravity
        velocityY += Time.deltaTime * gravity;

        //Player Rotation
        if (inputDir != Vector2.zero && canMove == true && !escapeMenu && !craftingMenu && !isBowAiming && !introTutorial)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        // Smooth runspeed 
        float targetSpeed = runSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        if (isBowAiming && !hasWon)
        {
            //Bow aiming movement
            horizontalMovement = Input.GetAxis("Horizontal");
            verticalMovement = Input.GetAxis("Vertical");

            Vector3 targetDirection = new Vector3(horizontalMovement, velocityY, verticalMovement);
            targetDirection = Camera.main.transform.TransformDirection(targetDirection) * currentSpeed;

            // Move the controller
            controller.Move(targetDirection * Time.deltaTime);

            animator.SetFloat("HorizontalMovement", horizontalMovement);
            animator.SetFloat("VerticalMovement", verticalMovement);

        }
        else
        { 
            velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
           

            if (canMove && !hasWon && !escapeMenu && !craftingMenu && !introTutorial)
            {
                controller.Move(velocity * Time.deltaTime);
            }
            currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
        }

        if (animator.GetFloat("speedPercent") > 0.3f && !hasWon && isAlive)
        {
            if (isBowAiming)
            {
                steptimer += Time.deltaTime;
                if (steptimer > 0.5f)
                {
                    audioSourceFX.volume = Random.Range(0.8f, 1);
                    audioSourceFX.PlayOneShot(audioClips[6]);
                    steptimer = 0;
                }

            }
            else
            {
                if (animator.GetBool("isJumping") == false)
                {
                    steptimer += Time.deltaTime;
                    if (steptimer > 0.3f)
                    {
                        audioSourceFX.volume = Random.Range(0.8f, 1);
                        audioSourceFX.PlayOneShot(audioClips[6]);
                        steptimer = 0;
                    }
                }
                
            }
            
        }
        

        //Reset jumping
        if (controller.isGrounded)
        {
            velocityY = 0;
            animator.SetBool("isJumping", false);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !escapeMenu && isGrounded && isWeaponEquipped == false && canMove == true && !hasWon && !craftingMenu && !introTutorial)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
            animator.SetBool("isJumping", true);
            audioSourceFX.PlayOneShot(audioClips[0]);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoints -= damage;
    }

    public void AttackBoss(float damage)
    {
        boss.TakeDamage(damage);
    }

    void ChangeWeapon()
    {
        //Sword
        if (Input.GetKeyDown(KeyCode.Alpha1) && isAlive && !hasWon && !craftingMenu &&!escapeMenu && hasCraftedSword)
        {
            if (!isWeaponEquipped || equippedWeapon == "Bow")
            {
                timerSwordActive = 0f;
                equippedWeapon = "Sword";
                swordActive = true;
                bowActive = false;
                animator.SetBool("isBowEquipped", false);
                animator.SetBool("isSwordEquipped", true);
                isWeaponEquipped = true;
                audioSourceFX.pitch = 1f;
                audioSourceFX.PlayOneShot(audioClips[2]);
            }
            else
            {
                timerSwordActive = 0f;
                swordActive = false;
                equippedWeapon = "None";
                animator.SetBool("isSwordEquipped", false);
                isWeaponEquipped = false;
                audioSourceFX.pitch = 1f;
                audioSourceFX.PlayOneShot(audioClips[3]);
            }
        }

        //Bow
        if (Input.GetKeyDown(KeyCode.Alpha2) && isAlive && !hasWon && !craftingMenu && !escapeMenu && hasCraftedBow)
        {
            if (!isWeaponEquipped || equippedWeapon == "Sword")
            {
                timerSwordActive = 0f;
                swordActive = false;
                bowActive = true;
                equippedWeapon = "Bow";
                isWeaponEquipped = true;
                animator.SetBool("isSwordEquipped", false);
                animator.SetBool("isBowEquipped", true);
                animator.SetTrigger("BowEquip");
                audioSourceFX.pitch = 1;
                audioSourceFX.PlayOneShot(audioClips[4]);
            }
            else
            {
                timerSwordActive = 0f;
                bowActive = false;
                equippedWeapon = "None";
                isWeaponEquipped = false;
                animator.SetBool("isBowEquipped", false);
                animator.SetTrigger("BowUnequip");
                audioSourceFX.pitch = 1;
                audioSourceFX.PlayOneShot(audioClips[4]);
            }
        }
    }

    public void UnequipWeapons()
    {
        isWeaponEquipped = false;
        swordActive = false;
        bowActive = false;
        timerSwordActive = 0f;
        animator.SetBool("isSwordEquipped", false);
        animator.SetBool("isBowEquipped", false);
        equippedWeapon = "None";
    }

    void SwordActive()
    {
        if (swordActive)
        {
            timerSwordActive += Time.deltaTime;
            if (timerSwordActive >= 0.8f)
            {
                sword.gameObject.SetActive(true);

            }
        }
        else
        {
            timerSwordActive += Time.deltaTime;
            if (timerSwordActive >= 0.8f)
            {
                sword.gameObject.SetActive(false);
            }

        }
    }

    void BowActive()
    {
        if (bowActive)
        {
            timerSwordActive += Time.deltaTime;
            if (timerSwordActive >= 0.8f)
            {
                bow.gameObject.SetActive(true);

            }
        }
        else
        {
            timerSwordActive += Time.deltaTime;
            if (timerSwordActive >= 0.8f)
            {
                bow.gameObject.SetActive(false);
            }

        }

        if (bow.gameObject.activeSelf && !hasWon)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && arrowCount > 0)
            {
                isBowAiming = true;
                audioSourceFX.volume = 0.7f;
                audioSourceFX.PlayOneShot(audioClips[8]);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && arrowCount == 0)
            {
                noArrowText = true;
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                isBowAiming = false;
            }

            //When aiming Bow
            if (isBowAiming)
            {
                animator.SetBool("isBowAiming", true);
                CreateAimTarget();
                runSpeed = walkSpeed;

                bowShootTimer += Time.deltaTime;

                //Shooting with bow
                if (Input.GetKeyDown(KeyCode.Mouse0) && arrowCount > 0)
                {
                    if (bowShootTimer >= 0.8f)
                    {
                        animator.SetTrigger("BowShoot");
                        audioSourceFX.pitch = Random.Range(0.8f, 1f);
                        audioSourceFX.PlayOneShot(audioClips[1]);
                        bowShootTimer = 0;
                    }
                }
            }
            else
            {
                runSpeed = runSpeedDefault;
                animator.SetBool("isBowAiming", false);
            }
        }
    }

    public void NoArrowTimer()
    {
        if (noArrowText)
        {
            messageTimer += Time.deltaTime;

            if (messageTimer >= 2f)
            {
                noArrowText = false;
                messageTimer = 0;
            }
        }
    }

    void CreateAimTarget()
    {
        //When aiming, always look in camera direction
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        if (forward.sqrMagnitude != 0.0f)
        {
            forward.Normalize();
            transform.LookAt(transform.position + forward);
        }
    }

    void SwordAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && swordActive && !hasWon && isAlive && !escapeMenu)
        {
            

            if (swordAttackTimer > 1f)
            {
                animator.SetTrigger("Attacking");
                isSwordAttacking = true;
                audioSourceFX.volume = 0.7f;
                audioSourceFX.PlayOneShot(audioClips[7]);
                swordAttackTimer = 0;
            }
            
        }

        //Only attack if in melee range
        float distance = Vector3.Distance(boss.transform.position, transform.position);
        if (isSwordAttacking == true && distance < 3f)
        {
            attackTimer += Time.deltaTime;

            //Moment of Sword Impact
            if (attackTimer >= 0.5f)
            {
                if (boss.currentShieldPoints <= 0)
                {
                    boss.TakeDamage(20f);
                    audioSourceFX.volume = 0.4f;
                    audioSourceFX.PlayOneShot(audioClips[11]);
                    audioSourceFX.volume = soundEffectVolume;
                    isSwordAttacking = false;
                    attackTimer = 0f;
                }
            }
        }
    }

    void CanInteract()
    {
        if (!canInteract)
        {
            chopTreeText.gameObject.SetActive(false);
            gatherStoneText.gameObject.SetActive(false);
            gatherBushText.gameObject.SetActive(false);
            gatherIronText.gameObject.SetActive(false);
        }
    }

    void AxeActive()
    {
        if (axeActive)
        {
            axe.SetActive(true);
        }
        else
        {
            axe.SetActive(false);
        }
    }

    void HammerActive()
    {
        if (hammerActive)
        {
            hammer.SetActive(true);
        }
        else
        {
            hammer.SetActive(false);

        }
    }

    void Die()
    {
        if (currentHealthPoints <= 0f)
        {
            animator.SetBool("dead", true);
            canMove = false;
            isAlive = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Win()
    {
        if (boss.currentHealthPoints <= 0f)
        {
            velocity = Vector3.zero;
            inputDir = Vector2.zero;
            animator.SetFloat("speedPercent", 0);
            animator.SetFloat("horizontalMovement", 0);
            animator.SetFloat("verticalMovement", 0);
            animator.SetBool("isBowAiming", false);
            isBowAiming = false;
            hasWon = true;
            canMove = false;
            isSwordAttacking = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void CheckForTrees()
    {
        foreach (GameObject element in trees)
        {
            if (element != null)
            {
                float distance = Vector3.Distance(element.transform.position, transform.position);
                if (distance < interactRange)
                {
                    chopTreeText.gameObject.SetActive(true);
                }
            }
            
        }
    }

    void CheckForStones()
    {
        foreach (GameObject element in stones)
        {
            if (element != null)
            {
                float distance = Vector3.Distance(element.transform.position, transform.position);
                if (distance < interactRange)
                {
                    gatherStoneText.gameObject.SetActive(true);
                }
            }

        }
    }

    void CheckForIron()
    {
        foreach (GameObject element in iron)
        {
            if (element != null)
            {
                float distance = Vector3.Distance(element.transform.position, transform.position);
                if (distance < interactRange)
                {
                    gatherIronText.gameObject.SetActive(true);
                }
            }

        }
    }

    void CheckForBushes()
    {
        foreach (GameObject element in bushes)
        {
            if (element != null)
            {
                float distance = Vector3.Distance(element.transform.position, transform.position);
                if (distance < interactRange)
                {
                    gatherBushText.gameObject.SetActive(true);
                }
            }

        }
    }

    void CheckForGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);
        if (hit.distance > 0.1f)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }
    }

    void EscapeMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !craftingMenu)
        {
            if (escapeMenu)
            {
                escapeMenu = false;
                
            }
            else
            {
                escapeMenu = true;
            }
        }
    }

    void chopSound()
    {
        if (startChopSound)
        {
            chopTreeTimer += Time.deltaTime;
            if (chopTreeTimer > 1.7f)
            {
                if (chopCount < 2)
                {
                    audioSourceFX.PlayOneShot(audioClips[9]);
                    chopCount++;
                    chopTreeTimer = 0;
                }
                else
                {
                    startChopSound = false;
                }
            }
        }
        else
        {
            chopCount = 0;
            chopTreeTimer = 0.9f;
        }
    }

    void GatherBushSound()
    {
        if (startBushSound)
        {
            bushSoundTimer += Time.deltaTime;
            if (bushSoundTimer >= 0.5f)
            {
                audioSourceFX.PlayOneShot(audioClips[5]);
                startBushSound = false;
                bushSoundTimer = 0;
            }
        }
    }

    void BossAttackSound()
    {
        if (startBossAttackSound)
        {
            audioSourceFX.volume = 0.4f;
            audioSourceFX.PlayOneShot(audioClips[10]);
            audioSourceFX.volume = soundEffectVolume;
        }
        startBossAttackSound = false;
    }
}
