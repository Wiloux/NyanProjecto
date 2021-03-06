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
    public static Action onTp;
    public Voicelines onTpVoicelines;

    public static Action onBossLinked;
    public Voicelines onBossLinkedVoicelines;

    public static Action onHittingBoss;
    public Voicelines onHittingBossVoicelines;

    public static Action onBossNewPhase;
    public Voicelines onBossNewPhaseVoicelines;

    public static Action onBossKilled;
    public Voicelines onBossKilledVoicelines;

    public static Action onVictory;
    public Voicelines onVictoryVoicelines;

    private void Awake()
    {
        onTp = () => PlayVoicelineIfNeeded(onTpVoicelines); 
        onBossLinked = () => PlayVoicelineIfNeeded(onBossLinkedVoicelines);
        onHittingBoss = () => PlayVoicelineIfNeeded(onHittingBossVoicelines);
        onBossNewPhase = () => ForceVoicelinePlay(onBossNewPhaseVoicelines);
        onBossKilled = () => ForceVoicelinePlay(onBossKilledVoicelines);
        onVictory = () => ForceVoicelinePlay(onVictoryVoicelines);
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasGeneralCooldown) generalCooldown -= Time.deltaTime;

        onTpVoicelines.ReduceCooldown();
        onBossLinkedVoicelines.ReduceCooldown();
        onHittingBossVoicelines.ReduceCooldown();
        onBossNewPhaseVoicelines.ReduceCooldown();
        onBossKilledVoicelines.ReduceCooldown();
        onVictoryVoicelines.ReduceCooldown();
    }

    private void PlayVoicelineIfNeeded(Voicelines voicelines)
    {
        if (voicelines.HasCoolDown && HasGeneralCooldown)
        {
            voicelines.clipsVolumes.Play(generalAudioSource);
            voicelines.ResetCooldown();
            generalCooldown = generalCooldownDuration;
        }
    }

    private void ForceVoicelinePlay(Voicelines voicelines)
    {
        if (voicelines.HasCoolDown)
        {
            voicelines.clipsVolumes.Play(generalAudioSource);
            voicelines.ResetCooldown();
        }
    }
}
[Serializable] public class Voicelines
{
    public ClipsVolumes clipsVolumes;
    public float cooldownDuration;
    private float cooldown;

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
