using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicelinesManager : MonoBehaviour
{
    [SerializeField] private AudioSource generalAudioSource;

    public float generalCooldownDuration;
    private float generalCooldown;
    private bool HasGeneralCooldown => generalCooldown <= 0;

    [Space(20)]
    [Header("Voicelines")]
    public static Action onBossLinked;
    public Voicelines onBossLinkedVoicelines;

    public static Action onHittingBoss;
    public Voicelines onHittingBossVoicelines;

    public static Action onBossKilled;
    public Voicelines onBossKilledVoicelines;

    public static Action onVictory;
    public Voicelines onVictoryVoicelines;

    private void Awake()
    {
        onBossLinked = () => PlayVoicelineIfNeeded(onBossLinkedVoicelines);
        onHittingBoss = () => PlayVoicelineIfNeeded(onHittingBossVoicelines);
        onBossKilled = () => PlayVoicelineIfNeeded(onBossKilledVoicelines);
        onVictory = () => PlayVoicelineIfNeeded(onVictoryVoicelines);
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasGeneralCooldown) generalCooldown -= Time.deltaTime;

        onBossLinkedVoicelines.ReduceCooldown();
        onHittingBossVoicelines.ReduceCooldown();
        onBossKilledVoicelines.ReduceCooldown();
        onVictoryVoicelines.ReduceCooldown();
    }

    private void PlayVoicelineIfNeeded(Voicelines voicelines)
    {
        if (voicelines.HasCoolDown && HasGeneralCooldown)
        {
            voicelines.clipsVolumes.Play(generalAudioSource);
        }
    }
}
[Serializable] public class Voicelines
{
    public ClipsVolumes clipsVolumes;
    public float cooldownDuration;
    public float cooldown;

    #region Cooldown gestion
    public bool HasCoolDown => cooldown <= 0;
    public void ReduceCooldown()
    {
        if (!HasCoolDown) cooldown -= Time.deltaTime;
    }
    public void ResetCooldown()
    {
        cooldown = cooldownDuration;
}
    #endregion
}
