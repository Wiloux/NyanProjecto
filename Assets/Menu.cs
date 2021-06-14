using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public int sceneID;

    private void Start()
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void PlayTheGame()
    {
        SceneManager.LoadScene(sceneID);
    }

    public void OpenControlsPanel(GameObject optionPanel)
    {
        optionPanel.SetActive(true);
    }

    public void CloseControlsPanel(GameObject optionPanel)
    {
        optionPanel.SetActive(false);
    }

    public void MyQuit()
    {
        Application.Quit();
    }
}
