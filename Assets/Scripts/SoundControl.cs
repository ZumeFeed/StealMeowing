using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{

    public enum audioName
    { 
        hatMove, hitWall, hitBucket, scarecrowScream, catMeowing, dogBark
    }

    AudioSource audioSource;
    [SerializeField] List<AudioClip> sounds;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void playSound(audioName an)
    {
        audioSource.clip = sounds[(int)an];
        audioSource.Play();
    }
}
