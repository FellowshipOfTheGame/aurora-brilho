using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private float musicVolume = 1f;
    private float SFXVolume = 1f;

    private AudioClip startButton, creditsButton, backButton, exitButton, music1, music2, hit, heal, bounce, potion, jump;
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
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        music1 = Resources.Load<AudioClip>("Song 1");
        music2 = Resources.Load<AudioClip>("Song 2");
        startButton = Resources.Load<AudioClip>("StartButton");
        creditsButton = Resources.Load<AudioClip>("CreditsButton");
        backButton = Resources.Load<AudioClip>("BackButton");
        exitButton = Resources.Load<AudioClip>("ExitButton");
        hit = Resources.Load<AudioClip>("Hit1" );
        heal = Resources.Load<AudioClip>("Heal");
        bounce = Resources.Load<AudioClip>("Boing");
        potion = Resources.Load<AudioClip>("Potion");
        jump = Resources.Load<AudioClip>("Jump");
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
            case "Hit":
                audioSource.PlayOneShot(hit, SFXVolume * 2f);
                break;
            case "Heal":
                audioSource.PlayOneShot(heal, SFXVolume * 0.5f);
                break;
            case "Bounce":
                audioSource.PlayOneShot(bounce, SFXVolume);
                break;
            case "Potion":
                audioSource.PlayOneShot(potion, SFXVolume);
                break;
            case "Jump":
                audioSource.PlayOneShot(jump, SFXVolume);
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
