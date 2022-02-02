using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Resume()
    {
        int scene = PlayerPrefs.GetInt("Scene");
        SceneManager.LoadScene(scene);
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteKey("Scene");
        PlayerPrefs.SetInt("Scene", 1);
        SceneManager.LoadScene(1);
    }

    public void Credits()
    {
        Debug.Log("Comiing Sooon????");
    }

    public void Exit()
    {
        Debug.Log("It will work in the Android Version");
        Application.Quit();
    }
}
