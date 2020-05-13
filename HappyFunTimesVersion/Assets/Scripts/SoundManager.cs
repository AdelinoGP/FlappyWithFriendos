using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private AudioClip birdDie;
    [SerializeField] private AudioClip birdWing;
    [SerializeField] private AudioClip gotCoin;
    private AudioSource audioSource;

    private static SoundManager instance;
    public static SoundManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        instance = this;
        audioSource = (AudioSource) GetComponent<AudioSource>();
    }

    public void SoundBirdDied()
    {
        audioSource.PlayOneShot(birdDie, 1f);
    }

    public void SoundBirdWing()
    {
        audioSource.PlayOneShot(birdWing, 1f);
    }

    public void SoundGotCoin()
    {
        audioSource.PlayOneShot(gotCoin, 1f);
    }
}
