using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Audio
{
    [HideInInspector] public AudioSource source;
    public string name;
    public AudioClip clip;
    public float volume;
}
