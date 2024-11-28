using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance = null;

    public string Manual;
    public string Stage1;
    public string Ametalkclub;
    public string GameClear;
    public string GameOver;
    public string StartScene;

    public void LoadManual()
    {
        SceneManager.LoadScene(Manual);
    }

    public void LoadStage1()
    {
        SceneManager.LoadScene(Stage1);
    }

    public void LoadAmetalkclubScreen()
    {
        SceneManager.LoadScene(Ametalkclub);
    }

    public void LoadClearScreen()
    {
        SceneManager.LoadScene(GameClear);
    }
    public void LoadGameOverScreen()
    {
        SceneManager.LoadScene(GameOver);
    }
    public void LoadStartScreen()
    {
        SceneManager.LoadScene(StartScene);
    }

    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
