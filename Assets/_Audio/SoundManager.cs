using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

    private static SoundManager instance;
    public static SoundManager Instance
    {
        get { return instance; }
    }

    [SerializeField] AudioClip whistle;
    [SerializeField] AudioClip punchHit;

    AudioSource audioSource;

    void OnEnable(){
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(audioSource)
            ChangeSceneBgSound();
    }

    void OnDisable(){
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }


    void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    void Start(){
        audioSource = GetComponent<AudioSource>();
        ChangeSceneBgSound();
    }

    public void ChangeSceneBgSound()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Menu":
                audioSource.volume = 1f;
                break;
            case "Game":
                audioSource.volume = 0.5f;
                break;
        }
    }

    public void PlayWhistleSound()
    {
        audioSource.PlayOneShot(whistle);
    }
    public void PlayPunchSound()
    {
        audioSource.PlayOneShot(punchHit);
    }

}
