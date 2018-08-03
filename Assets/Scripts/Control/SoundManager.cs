using UnityEngine;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource effectSource;
    public AudioSource musicSource;
    public AudioSource titleMusic;
    public float volume = 1.0f;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    public Slider volumeSlider;
    public bool created;

    private void Update()
    {
        if (volumeSlider == null) {
            return;
        }
        volume = volumeSlider.value;
        effectSource.volume = volume;
        musicSource.volume = volume;
        titleMusic.volume = volume;
    }
    public void PlaySingle(AudioClip clip)
    {
        effectSource.clip = clip;
        effectSource.Play();
    }

    public void enterGame() {
        if (titleMusic.isPlaying)
        {
            titleMusic.Stop();
        }
        else if (musicSource.isPlaying) {
            return;
        }
        musicSource.Play();
    }

    public void exitGame() {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        else if (titleMusic.isPlaying) {
            return;
        }
        titleMusic.Play();
    }

    public void RandomizeSFX(params AudioClip[] clips)
    {
        int randIndex = Random.Range(0, clips.Length);
        float randPitch = Random.Range(lowPitchRange, highPitchRange);

        effectSource.pitch = randPitch;
        effectSource.clip = clips[randIndex];
        effectSource.Play();
    }
}
