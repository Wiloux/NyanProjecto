using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{

    public int sceneID;
    public bool hasEnded;
    public GameObject EndingUI;


    public void StartEnding()
    {
        if (!hasEnded)
        {
            StartCoroutine(StartEndingCoro());
        }

    }

    IEnumerator StartEndingCoro()
    {

        hasEnded = true;
        EndingUI.SetActive(true);
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene(sceneID);
        Cursor.lockState = CursorLockMode.None;
    }
}
