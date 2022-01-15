using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singleton

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private float musicVolume = 1f;
    private float SFXVolume = 1f;

    private AudioClip mainMusic, startButton, creditsButton, backButton, exitButton;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainMusic = Resources.Load<AudioClip>("Teste 1 Aurora");
        startButton = Resources.Load<AudioClip>("StartButton");
        creditsButton = Resources.Load<AudioClip>("CreditsButton");
        backButton = Resources.Load<AudioClip>("BackButton");
        exitButton = Resources.Load<AudioClip>("ExitButton");
        //musicVolume = PlayerPrefsController.GetMusicVolume();
        //SFXVolume = PlayerPrefsController.GetSFXVolume();
        audioSource.volume = musicVolume;
        ChangeMusic("Main Music");
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

    public void ChangeMusic(string song)
    {
        switch (song)
        {
            case "Main Music":
                audioSource.clip = mainMusic;
                audioSource.Play();
                break;
            default:
                break;
        }
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
