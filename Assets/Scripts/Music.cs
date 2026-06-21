using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour
{
    public AudioClip[] audioClips;
    public Biome[] biomes;

    private Dictionary<Biome, AudioClip> music;
    private AudioSource audioSource;

    private bool isPlaying;
    private float timer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        music = new();
        
        for (int i = 0; i < biomes.Length; i++)
        {
            music.Add(biomes[i], audioClips[i]);
        }
    }

    private void Update()
    {
        if (timer < Time.time)
        {
            timer = Time.time + 1;
            if (Random.value > 0.95f && !isPlaying)
            {
                StartCoroutine(PlayMusic());
            }
        }
    }

    private IEnumerator PlayMusic()
    {
        AudioClip clip;
        if (!Nature.IsDay()) clip = audioClips[^1];
        else clip = music[Generation.CurrentBiome];

        if (clip == null) yield break;

        float length = clip.length;

        audioSource.clip = clip;
        audioSource.Play();
        isPlaying = true;

        yield return new WaitForSeconds(length);
        audioSource.Stop();
        isPlaying = false;
    }
}