using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Audio[] audios;

    public void playSound(string name)
    {
        foreach (var item in audios)
        {
            if (item.name == name)
            {
                if (item.source == null)
                    item.source = gameObject.AddComponent<AudioSource>();

                item.source.clip = item.clip;
                item.source.volume = item.volume;
                
                item.source.Play();
                break;
            }
        }
    }
}
