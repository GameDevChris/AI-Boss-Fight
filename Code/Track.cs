using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Track", menuName = "Music/Track", order = 1)]
public class Track : ScriptableObject
{
    public string composer = "Composer name";
    public string refLink = "Reference link";
    public string trackName = "Song Name";
    public AudioClip trackClip = null;
    public int beatsPerMinute = 100;
}
