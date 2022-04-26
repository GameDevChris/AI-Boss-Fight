using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeatSync : MonoBehaviour
{
    private enum Step
    {
        Hover,
        Slap,
        Pause
    };
    
    //Serialized private variables to be viewed in inspector
    [SerializeField]
    private ParticleSystem _bgParticles;
    
    [SerializeField]
    private int _musicStarDelay = 4;
    
    [SerializeField]
    private List<Track> _musicTracks;
    
    [SerializeField]
    private int _trackChoice = 1;
        
    //Private only variables
    private AudioSource _bgMusic;
    
    private float _songSpeed = 1f;
    private float _originalBPM = 75f;
    private float _calculatedBPM = 75f;
    private float _timer = 0f;
    
    private Step _previousStep;
    private Step _currentStep = Step.Hover;

    private BossAI _boss;
    
    void Start ()
    {
        _boss = FindObjectOfType<BossAI>();
        _bgMusic = GetComponent<AudioSource>();
    }
    
    void Update ()
    {
        //If track changes
        if (_bgMusic.clip != _musicTracks[_trackChoice].trackClip)
        {
            _bgMusic.clip = _musicTracks[_trackChoice].trackClip;
            _originalBPM = _musicTracks[_trackChoice].beatsPerMinute;
            _bgMusic.Play();
        }
        
        //Calculate song speed with boss health and apply
        _songSpeed = 1 + ((_boss.MaxHealth - _boss.CurrentHealth) / 100);
        _calculatedBPM = _originalBPM * _songSpeed;
        _bgMusic.pitch = _songSpeed;

        //bpm timer
        _timer += Time.deltaTime;
 
        if (_timer >= (60f/_calculatedBPM)) 
        {
            //Start intro and delay
            if (_musicStarDelay > 0)
            {
                if (!_boss.isIntro)
                {
                    _boss.StartIntro((60f/_calculatedBPM)*_musicStarDelay);
                }
                _musicStarDelay--;
            }
            
            else
            {
                if (_boss.CurrentHealth < (_boss.MaxHealth/10)*6)
                {
                    _bgParticles.Play();
                }
                
                if (_currentStep == Step.Hover)
                {
                    _boss.Hover(60f/_calculatedBPM);
                    _previousStep = _currentStep;
                    _currentStep = Step.Pause;
                }

                else if (_currentStep == Step.Slap)
                {
                    _boss.Slap(60f/_calculatedBPM);
                    _previousStep = _currentStep;
                    _currentStep = Step.Pause;
                }

                else if (_currentStep == Step.Pause)
                {
                    if (_previousStep == Step.Hover)
                    {
                        _currentStep = Step.Slap;
                    }
                    
                    else if (_previousStep == Step.Slap)
                    {
                        _boss.Event();
                        _currentStep = Step.Hover;
                    }

                    _previousStep = _currentStep;
                }
            }
            _timer -= (60f/_calculatedBPM);
        }
    }
}
