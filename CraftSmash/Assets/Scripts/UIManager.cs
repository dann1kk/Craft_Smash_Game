using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //Health Bar Player
    [Header("Health Bar Player")]
    [SerializeField] Image healthProgressImagePlayer;
    public float healthBarWidthPlayer;

    //Health Bar Boss
    [Header("Health Bar Boss")]
    [SerializeField] GameObject bossHealthPanel;
    [SerializeField] Image healthProgressImageBoss;
    public float healthBarWidthBoss;

    //Shield Bar Boss
    [Header("Shield Bar Boss")]
    [SerializeField] Image shieldProgressImageBoss;
    public float shieldBarWidthBoss;

    //Ingame Menu
    [Header("Ingame Menu")]
    public GameObject escapeMenuObject;
    [SerializeField]
    Button ResetButton;

    //Tutorials
    [Header("Tutorials")]
    public GameObject introductionTutorialPanel;
    bool introductionTutorial = true;
    bool bossTutorial = false;

    //Crafting
    GameObject CraftingPanel;

    //Win / Death
    [Header("Win / Death Text")]
    [SerializeField]
    Text deathText;
    [SerializeField]
    Text winText;

    //Weapons
    [Header("Weapons")]
    [SerializeField] GameObject swordButton;
    [SerializeField] GameObject swordButtonBorder;
    [SerializeField] GameObject bowButton;
    [SerializeField] GameObject bowButtonBorder;

    [SerializeField] GameObject arrowButton;
    [SerializeField] Text arrowText;
    [SerializeField] GameObject crossHair;
    [SerializeField] GameObject noArrowText;

    //Inventory
    [Header("Inventory")]
    [SerializeField] Text woodCount;
    [SerializeField] Text stoneCount;
    [SerializeField] Text leafCount;
    [SerializeField] Text ironCount;

    //Controller references
    PlayerCharacterController player;
    BossController boss;
    public SoundManager soundManager;
    public Slider musicSlider;
    public float musicVolume;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacterController>();
        boss = GameObject.Find("Boss").GetComponent<BossController>();
        CraftingPanel = GameObject.Find("CraftingUI");

        ResetButton.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
        deathText.gameObject.SetActive(false);

        //Hotbar
        swordButton.SetActive(false);
        swordButtonBorder.SetActive(false);
        bowButton.SetActive(false);
        bowButtonBorder.SetActive(false);

        arrowButton.SetActive(false);
        noArrowText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Health Bar Player
        healthBarWidthPlayer = player.currentHealthPoints / player.maxHealthPoints;
        healthProgressImagePlayer.fillAmount = healthBarWidthPlayer;

        //Health Bar Boss
        healthBarWidthBoss = boss.currentHealthPoints / boss.maxHealthPoints;
        healthProgressImageBoss.fillAmount = healthBarWidthBoss;

        //Shield Bar Boss
        shieldBarWidthBoss = boss.currentShieldPoints / boss.maxShieldPoints;
        shieldProgressImageBoss.fillAmount = shieldBarWidthBoss;


        BossCombat();
        Inventory();
        Death();
        Win();
        EscapeMenu();
        CraftingUI();
        HotBar();
        CrossHair();
        NoArrow();

        IntroductionTutorial();
    }

    public void ResetGame()
    {
        Initiate.Fade("GameWorld", Color.black, 2.0f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        Initiate.Fade("Menu", Color.black, 2.0f);
    }

    public void TakeDamage(float damage)
    {
        player.currentHealthPoints -= damage;
        //SoundManager.instance.PlaySound(0);
    }

    void Inventory()
    {
        woodCount.text = player.woodCount.ToString();
        stoneCount.text = player.stoneCount.ToString();
        leafCount.text = player.leafCount.ToString();
        ironCount.text = player.ironCount.ToString();
    }

    void BossCombat()
    {
        if (boss.isInCombat)
        {
            bossHealthPanel.SetActive(true);
        }
        else
        {
            bossHealthPanel.SetActive(false);
        }
    }

    void Death()
    {
        if (!player.isAlive)
        {
            ResetButton.gameObject.SetActive(true);
            deathText.gameObject.SetActive(true);
        }
    }

    void Win()
    {
        if (!boss.isAlive)
        {
            ResetButton.gameObject.SetActive(true);
            winText.gameObject.SetActive(true);
        }
    }

    void EscapeMenu()
    {
        if (player.escapeMenu && !player.introTutorial)
        {
            escapeMenuObject.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            
            
            player.inputDir = Vector2.zero;
            player.velocity = Vector3.zero;
        }
        else if (player.craftingMenu)
        {
            CraftingPanel.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (player.introTutorial)
        {
            escapeMenuObject.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (player.escapeMenu == false && player.craftingMenu == false && !player.hasWon && player.isAlive)
        {
            escapeMenuObject.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void CraftingUI()
    {
        if (player.craftingMenu)
        {
            CraftingPanel.gameObject.SetActive(true);
            player.craftingMenu = true;
        }
        else
        {
            CraftingPanel.gameObject.SetActive(false);
            player.craftingMenu = false;
        }
    }

    void HotBar()
    {
        if (player.hasCraftedSword)
        {
            swordButton.SetActive(true);
        }

        if (player.hasCraftedBow)
        {
            bowButton.SetActive(true);
        }

        if (player.swordActive)
        {
            swordButtonBorder.SetActive(true);
        }
        else
        {
            swordButtonBorder.SetActive(false);
        }
        if (player.bowActive)
        {
            bowButtonBorder.SetActive(true);
            arrowButton.SetActive(true);
        }
        else
        {
            bowButtonBorder.SetActive(false);
            arrowButton.SetActive(false);
        }

        arrowText.text = player.arrowCount.ToString() + "x";
    }

    void CrossHair()
    {
        if (player.isBowAiming)
        {
            crossHair.SetActive(true);
        }
        else
        {
            crossHair.SetActive(false);
        }
    }

    void NoArrow()
    {
        if (player.noArrowText)
        {
            noArrowText.SetActive(true);
        }
        else
        {
            noArrowText.SetActive(false);
        }
    }

    void IntroductionTutorial()
    {
        if (player.introTutorial)
        {
            introductionTutorialPanel.SetActive(true);
        }
        else
        {
            introductionTutorialPanel.SetActive(false);
        }
    }

    public void CloseIntroduction()
    {
        player.introTutorial = false;
    }
}
