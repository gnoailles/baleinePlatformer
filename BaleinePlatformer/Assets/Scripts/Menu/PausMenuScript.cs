using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausMenuScript : MonoBehaviour {
  
    [SerializeField] private GameObject menuUI;
    [SerializeField] private string MenuName = "Menu";
    private bool isPaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;

            if (isPaused)
                Pause();
            else
                Continue();
        }     
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        menuUI.SetActive(true);
    }

    public void Continue()
    {
        
        menuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(MenuName, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
