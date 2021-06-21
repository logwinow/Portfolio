using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Custom.Extensions.Linq;
using Custom.Patterns;
using Custom.SmartCoroutines;
using UnityEngine;
using UnityEngine.Audio;

public partial class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private List<NamedAudioClip> _clips;
    [SerializeField] private AudioMixerSnapshot _drillSnapshotOn;
    [SerializeField] private AudioMixerSnapshot _drillSnapshotOff;
    [SerializeField] private AudioSource _drillAudioSource;
    [SerializeField] private AudioMixerSnapshot _jetpackSnapshotOn;
    [SerializeField] private AudioMixerSnapshot _jetpackSnapshotOff;
    [SerializeField] private AudioSource _jetpackAudioSource;
    [SerializeField] private AudioSource _oneShotAudioSource;
    [SerializeField] private float _fadeTime = 0.1f;
    [SerializeField] private AudioSource[] _ambientSources;

    private SmartWaitingCoroutine _drillWaitingCor;
    private SmartWaitingCoroutine _jetpackWaitingCor;
    
    protected override void Init()
    {
        _drillWaitingCor = new SmartWaitingCoroutine(this);
        _jetpackWaitingCor = new SmartWaitingCoroutine(this);
        
        DontDestroyOnLoad(this);
    }
    
    public void PlayOneShot(string name, bool isVariable = false)
    {
        if (!isVariable)
        {
            var nclip = GetNamedClip(name);

            _oneShotAudioSource.PlayOneShot(nclip.Clip, nclip.Volume);
            
            return;
        }

        var rclip = GetNamedClips(name).Random();
        _oneShotAudioSource.PlayOneShot(rclip.Clip, rclip.Volume);
    }

    public void PlayAmbient(int sceneIndex)
    {
        foreach (var a in _ambientSources)
        {
            a.Stop();
            a.clip = null;
        }
        
        switch (sceneIndex)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                Play(DIVING_MUSIC_AMBIENT, _ambientSources[0]);
                Play(DIVING_BREATING, _ambientSources[1]);
                break;
            case -1:
                Play(SPACESHIP_DISCOVERED, _ambientSources[0]);
                break;
        }
    }

    public void PlayDrill()
    {
        PlayInteraptable(DRILL, _drillAudioSource, _drillSnapshotOn, _drillWaitingCor);
    }

    public void StopDrill()
    {
         StopInteraptable(_drillAudioSource, _drillSnapshotOff, _drillWaitingCor);
    }
    
    public void PlayJetpack()
    {
        PlayInteraptable(JETPACK, _jetpackAudioSource, _jetpackSnapshotOn, _jetpackWaitingCor);
    }

    public void StopJetpack()
    {
        StopInteraptable(_jetpackAudioSource, _jetpackSnapshotOff, _jetpackWaitingCor);
    }

    private void PlayInteraptable(string name, AudioSource source,
        AudioMixerSnapshot snapshotOn, SmartWaitingCoroutine wCor)
    {
        wCor.Stop();
        
        Play(name, source);
        snapshotOn.TransitionTo(_fadeTime);
    }
    
    private void StopInteraptable(AudioSource source, AudioMixerSnapshot snapshotOff,
        SmartWaitingCoroutine wCor)
    {
        snapshotOff.TransitionTo(_fadeTime);
        wCor.Start(_fadeTime + 0.1f, 
            methodAfter: () => Stop(source));
    }

    private void Play(string name, AudioSource source)
    {
        source.clip = GetClip(name);
        source.Play();
    }

    private void Stop(AudioSource source)
    {
        source.Stop();
    }

    private AudioClip GetClip(string name)
    {
        return GetNamedClip(name).Clip;
    }

    private NamedAudioClip GetNamedClip(string name)
    {
        return _clips.Find(a => a.Name == name);
    }
    
    private List<NamedAudioClip> GetNamedClips(string name)
    {
        return _clips.FindAll(a => a.Name == name);
    }

    [Serializable]
    private struct NamedAudioClip
    {
        [SerializeField] private string _name;
        [SerializeField] private AudioClip _clip;
        [SerializeField] private float _volume;

        public string Name => _name;
        public AudioClip Clip => _clip;
        public float Volume => _volume;
    }
}

public partial class AudioManager
{
    public const string DRILL = "Drill";
    public const string TAKE = "Take";
    public const string ATTACKED = "Attacked";
    public const string DEATH_BY_SUFFOCATED = "Suffocated";
    public const string DEATH_BY_GET_HURT = "Killed";
    public const string LANDED = "Landed";
    public const string JUMPED = "Jumped";
    public const string WALK = "Walk";
    public const string JELLYFISH_ATTACK = "Jellyfish attack";
    public const string HEDGEHOG_ATTACK = "Hedgehog attack";
    public const string JETPACK = "Jetpack";
    public const string SUIT_PUT_ON = "Suit put on";
    public const string DIVING_MUSIC_AMBIENT = "Diving music";
    public const string DIVING_BREATING = "Diving breathing";
    public const string FORGE_REMELTED = "Remelted";
    public const string FORGE_OPENED = "Forge opening";
    public const string CRAFTED = "Crafted";
    public const string BUTTON_HIGHLIGHTED = "Button highlighted";
    public const string BUTTON_CLICKED = "Button clicked";
    public const string SPACESHIP_DISCOVERED = "Spaceship discovered";
}
