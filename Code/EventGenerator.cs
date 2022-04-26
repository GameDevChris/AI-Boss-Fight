using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventGenerator : MonoBehaviour
{
    public BossAI ai;
    public float bossHealthLostPercent;
    public List<FuzzyEventSO> eventSOs;

    public List<BeatPlatform> platforms;

    public float eventCooldown = 5;
    
    private void Start()
    {
        foreach (var plat in FindObjectsOfType<BeatPlatform>())
        {
            platforms.Add(plat);
        }
    }
    
    private void Update()
    {
        bossHealthLostPercent = (ai.MaxHealth - ai.CurrentHealth)/ai.MaxHealth;
    }

    public void EventDo(BeatPlatform platChoice)
    {
        foreach(var ev in eventSOs)
        {
            bool success = ev.TryAction(bossHealthLostPercent);

            if(success)
            {
                if(ev.isRandom)
                {
                    platChoice = platforms[UnityEngine.Random.Range(0, platforms.Count)];
                }

                switch (ev.fuzzyAction)
                {
                    case FuzzyEventSO.Action.GrowPlatform:
                        if (platChoice.CurrentEffect == BeatPlatform.Effect.None)
                        {
                            platChoice.CurrentEffect = BeatPlatform.Effect.Grow;
                            platChoice.StartCoroutine(platChoice.Countdown(5));
                        }
                        break;
            
                    case FuzzyEventSO.Action.HidePlatform:
                        if (platChoice.CurrentEffect == BeatPlatform.Effect.None)
                        {
                            platChoice.CurrentEffect = BeatPlatform.Effect.Hide;
                            platChoice.StartCoroutine(platChoice.Countdown(5));
                        }
                        break;
            
                    case FuzzyEventSO.Action.ZapPlatform:
                        
                        if (platChoice.CurrentEffect == BeatPlatform.Effect.None)
                        {
                            platChoice.CurrentEffect = BeatPlatform.Effect.Spike;
                            platChoice.StartCoroutine(platChoice.Countdown(5));
                        }
                        break;
                 }
            }    
        }
    }
}
