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
[Serializable] public class ClipsVolumes
{
    public ClipVolume[] clipsVolumes;

    public ClipVolume PickRandomClipVolume()
    {
        return clipsVolumes[UnityEngine.Random.Range(0, clipsVolumes.Length)];
    }

    public void Play(AudioSource audioSource, bool looping = false, bool stopping = false, bool debug = false)
    {
        if (clipsVolumes.Length == 0) return;
        PickRandomClipVolume().Play(audioSource, looping, stopping, debug);
    }
}
[System.Serializable] public class ClipVolume
{
    public AudioClip clip;
    [Range(0,1)] public float volume = 1f;

    public void Play(AudioSource audioSource, bool looping = false, bool stopping = false, bool debug = false)
    {
        if (clip == null || audioSource == null)
        {
            if(debug) Debug.Log("<color=red>Audio Source or Audio Clip was null</color>");
            return;
        }

        //if (audioSource.isPlaying) audioSource.Stop();
        if (stopping)
        {
            if(debug) Debug.Log("stopping");
            audioSource.Stop();
            audioSource.volume = 1f;
        }

        audioSource.loop = looping;
        if (looping)
        {
            if(debug) Debug.Log("Normal play");
            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }
        else
        {
            if(debug) Debug.Log("Onsehot play");
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
