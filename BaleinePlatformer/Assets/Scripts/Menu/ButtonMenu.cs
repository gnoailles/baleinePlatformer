using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonMenu : MonoBehaviour {

    [SerializeField] private string nameOfGameScene = "Game";
    [SerializeField] private GameObject MenuGameObject;
    [SerializeField] private GameObject OptionsGameObject;
    [SerializeField] private Text CheckBoxFullScreen;

    public void Update()
    {
        if (Screen.fullScreen)
        {
            CheckBoxFullScreen.color = new Color(125f / 255f, 231f / 255f, 165f / 255f, 255);
            CheckBoxFullScreen.text = "V";
        }
        else
        {
            CheckBoxFullScreen.color = new Color(231f / 255f, 125f / 255f, 125f / 255f, 255);
            CheckBoxFullScreen.text = "X";
        }
    }

    public void Start()
    {
   //    SceneManager.LoadScene(nameOfGameScene, LoadSceneMode.Single);
    }
    public void Menu()
    {
        MenuGameObject.SetActive(false);
        OptionsGameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back()
    {
        MenuGameObject.SetActive(true);
        OptionsGameObject.SetActive(false);
    }
    public void SetFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
