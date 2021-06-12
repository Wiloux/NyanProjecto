using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;
    private void Awake()
    {
        instance = this;
    }

    public static bool isPaused = false;
    public static bool enableControls = true;

    public static void SetPause(bool pause)
    {
        isPaused = pause;

        Time.timeScale = pause ? 0 : 1;
    }

    public void WaitForInput(float delay, Action onInput)
    {
        StartCoroutine(WaitingForInput(delay, onInput));
    }

    private IEnumerator WaitingForInput(float delay, Action onInput)
    {
        yield return new WaitForSecondsRealtime(delay);
        while (true)
        {
            if (Input.anyKeyDown) break;
            yield return null;
        }

        onInput?.Invoke();
    }
}
