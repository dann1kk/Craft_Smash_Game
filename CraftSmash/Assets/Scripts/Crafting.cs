using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
    bool craftingUI = true;
    bool isCrafting = false;
    float craftTimer = 0f;
    float itemCraftTimer = 5f;

    GameObject canCraftPanel;
    GameObject anvilObject;

    //References
    Animator animator;
    PlayerCharacterController player;

    //UI Control
    GameObject createSwordbutton;
    GameObject createBowbutton;
    GameObject createArrowbutton;

    GameObject swordRessourceText;
    GameObject bowRessourceText;
    GameObject arrowRessourceText;

    Text craftProgressText;
    Image craftProgressBar;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacterController>();
        animator = GameObject.Find("Player").GetComponentInChildren<Animator>();
        canCraftPanel = GameObject.Find("Crafting");
        anvilObject = GameObject.Find("Anvil");

        swordRessourceText = GameObject.Find("SwordErrorText");
        bowRessourceText = GameObject.Find("BowErrorText");
        arrowRessourceText = GameObject.Find("ArrowErrorText");

        createSwordbutton = GameObject.Find("CreateSwordButton");
        createBowbutton = GameObject.Find("CreateBowButton");
        createArrowbutton = GameObject.Find("CreateArrowButton");

        craftProgressText = GameObject.Find("CraftingProgressBarText").GetComponent<Text>();
        craftProgressBar = GameObject.Find("CraftingProgressBarForeground").GetComponent<Image>();

        craftProgressText.text = " ";
        craftProgressBar.fillAmount = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        CraftingActive();
        CraftingBar();
    }

    void CraftingActive()
    {
        float distance = Vector3.Distance(player.transform.position, anvilObject.transform.position);

        //Range and Activation
        if (distance < 2f)
        {
            CheckForRessources();

            if (!craftingUI)
            {
                canCraftPanel.SetActive(true);
                player.canMove = true;
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!craftingUI && !player.escapeMenu)
                {
                    player.inputDir = new Vector2(0, 0);
                    player.velocity = new Vector3(0, 0, 0);
                    craftingUI = true;
                    canCraftPanel.SetActive(false);
                    player.craftingMenu = true;
                    player.canMove = false;

                    animator.SetFloat("speedPercent", 0f);
                    player.inputDir = Vector2.zero;
                    player.velocity = Vector3.zero;
                }
                else
                {
                    CloseUI();
                }
                
            }
        }
        else
        {
            craftingUI = false;
            canCraftPanel.SetActive(false);
        }
    }

    public void CloseUI()
    {
        craftingUI = false;
        player.craftingMenu = false;
    }

    void CheckForRessources()
    {
        //Ressources for sword
        if (player.ironCount < 3 || player.woodCount < 2 || player.leafCount < 2)
        {
            if (!player.hasCraftedSword)
            {
                swordRessourceText.SetActive(true);
                createSwordbutton.SetActive(false);
            }
            else if(player.hasCraftedSword)
            {
                swordRessourceText.SetActive(false);
                createSwordbutton.SetActive(false);
            }
        }
        else
        {
            if (player.hasCraftedSword)
            {
                swordRessourceText.SetActive(false);
                createSwordbutton.SetActive(false);
            }
            else
            {
                swordRessourceText.SetActive(false);
                createSwordbutton.SetActive(true);
            }
            
        }

        //Ressources for bow
        if (player.woodCount < 4 || player.leafCount < 4)
        {
            if (!player.hasCraftedBow)
            {
                bowRessourceText.SetActive(true);
                createBowbutton.SetActive(false);
            }
            else if (player.hasCraftedBow)
            {
                bowRessourceText.SetActive(false);
                createBowbutton.SetActive(false);
            }
        }
        else
        {
            if (player.hasCraftedBow)
            {
                bowRessourceText.SetActive(false);
                createBowbutton.SetActive(false);
            }
            else
            {
                bowRessourceText.SetActive(false);
                createBowbutton.SetActive(true);
            }
        }

        //Ressources for arrow
        if (player.stoneCount < 3 || player.woodCount < 3 || player.leafCount < 3)
        {
            arrowRessourceText.SetActive(true);
            createArrowbutton.SetActive(false);
        }
        else
        {
            arrowRessourceText.SetActive(false);
            createArrowbutton.SetActive(true);
        }
    }

    public void CreateSword()
    {
        player.ironCount -= 3;
        player.woodCount -= 2;
        player.leafCount -= 2;

        isCrafting = true;
        craftProgressText.text = "Crafting Sword";
    }

    public void CreateBow()
    {
        player.woodCount -= 4;
        player.leafCount -= 4;

        isCrafting = true;
        craftProgressText.text = "Crafting Bow";

    }

    public void CreateArrow()
    {
        player.stoneCount -= 3;
        player.woodCount -= 3;
        player.leafCount -= 3;

        isCrafting = true;
        craftProgressText.text = "Crafting Arrow";

    }

    public void CraftingBar()
    {
        if (isCrafting)
        {
            craftTimer += Time.deltaTime;

            if (craftTimer >= itemCraftTimer)
            {
                isCrafting = false;
                craftTimer = 0f;

                if (craftProgressText.text == "Crafting Sword")
                {
                    player.hasCraftedSword = true;
                    createSwordbutton.SetActive(false);
                    swordRessourceText.SetActive(false);
                    craftProgressText.text = " ";
                }
                else if (craftProgressText.text == "Crafting Bow")
                {
                    player.hasCraftedBow = true;
                    createBowbutton.SetActive(false);
                    bowRessourceText.SetActive(false);
                    craftProgressText.text = " ";
                }
                else if (craftProgressText.text == "Crafting Arrow")
                {
                    player.arrowCount += 6;
                    craftProgressText.text = " ";
                }
            }

            craftProgressBar.fillAmount = craftTimer / itemCraftTimer;
        }
    }
}
