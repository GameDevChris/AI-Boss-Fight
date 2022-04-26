using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FuzzyEvent", menuName = "Fuzzy/Event", order = 1)]
public class FuzzyEventSO : ScriptableObject
{
    public bool isRandom;
    
    public enum Action
    {
        GrowPlatform,
        HidePlatform,
        ZapPlatform
    };

    public Action fuzzyAction;
    public float successChance = 0;
    public float diceRoll = 0;
    public AnimationCurve probabilityCurve;
    
    
    public bool TryAction(float chance)
    {        
        successChance = probabilityCurve.Evaluate(chance);
        diceRoll = Random.Range(0f, 1f);


        if (diceRoll < successChance)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    
}
