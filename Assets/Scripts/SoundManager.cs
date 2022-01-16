using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private float musicVolume = 1f;
    private float SFXVolume = 1f;

    private AudioClip music1, music2, startButton, creditsButton, backButton, exitButton;
    private AudioSource audioSource;

    public static SoundManager instance;

    private void Awake()
    {
        #region Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        #endregion

        audioSource = GetComponent<AudioSource>();
        music1 = Resources.Load<AudioClip>("Song 1");
        music2 = Resources.Load<AudioClip>("Song 2");
        startButton = Resources.Load<AudioClip>("StartButton");
        creditsButton = Resources.Load<AudioClip>("CreditsButton");
        backButton = Resources.Load<AudioClip>("BackButton");
        exitButton = Resources.Load<AudioClip>("ExitButton");
        //musicVolume = PlayerPrefsController.GetMusicVolume();
        //SFXVolume = PlayerPrefsController.GetSFXVolume();
        audioSource.volume = musicVolume;
    }

    public void PlaySound(string sound)
    {
        switch (sound)
        {
            case "Start Button Click":
                audioSource.PlayOneShot(startButton, SFXVolume);
                break;
            case "Credits Button Click":
                audioSource.PlayOneShot(creditsButton, SFXVolume);
                break;
            case "Back Button Click":
                audioSource.PlayOneShot(backButton, SFXVolume);
                break;
            case "Exit Button Click":
                audioSource.PlayOneShot(exitButton, SFXVolume);
                break;
            default:
                break;
        }
    }

    public void ChangeMusic(string song, bool startOver = true)
    {
        AudioClip clipToPlay = null;
        switch (song)
        {
            case "Song 1":
                clipToPlay = music1;
                break;
            case "Song 2":
                clipToPlay = music2;
                break;
            default:
                break;
        }

        if (!startOver && audioSource.clip == clipToPlay)
            return;

        audioSource.clip = clipToPlay;
        audioSource.Play();
    }

    public void ChangeSFXVolume(float value)
    {
        SFXVolume = value;
        //PlayerPrefsController.SetSFXVolume(value);
    }

    public void ChangeMusicVolume(float value)
    {
        audioSource.volume = value;
        //PlayerPrefsController.SetMusicVolume(value);
    }
}
