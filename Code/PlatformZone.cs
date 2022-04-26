using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformZone : MonoBehaviour
{
    public List<BeatPlatform> myPlatforms;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<BeatPlatform>()) return;
        if (!myPlatforms.Contains(other.GetComponent<BeatPlatform>()))
        {
            myPlatforms.Add(other.GetComponent<BeatPlatform>());
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<BeatPlatform>()) return;
        if (myPlatforms.Contains(other.GetComponent<BeatPlatform>()))
        {
            myPlatforms.Remove(other.GetComponent<BeatPlatform>());
        }
    }
}
