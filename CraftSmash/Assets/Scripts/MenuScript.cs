using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public void StartGame()
    {
        Initiate.Fade("GameWorld", Color.black, 2.0f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
