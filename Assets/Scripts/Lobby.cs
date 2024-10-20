using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{

    public Button StartButton;
    public Button QuitButton;
    public Button backButton;
    public Button Level1;
    public Button Level2;
    public GameObject LevelSelector;
    public GameObject LobbyMenu;

    public AudioSource AudioSource;
    public AudioClip StartSound;
    public AudioClip ExitSound;
    public AudioClip LevelSound;
    public AudioClip backSound;

    private void Start()
    {
        StartButton.onClick.AddListener(LevelSelect);
        QuitButton.onClick.AddListener(QuitGame);
        Level1.onClick.AddListener(PlayLevel1);
        Level2.onClick.AddListener(PlayLevel2);
        LevelSelector.SetActive(false);
        backButton.onClick.AddListener(backSelect);
       
    
    }
    

    public void QuitGame()
    {
        AudioSource.PlayOneShot(ExitSound);
        Application.Quit();
    }

    public void LevelSelect()
    {
        AudioSource.PlayOneShot(StartSound);
        LobbyMenu.SetActive(false);
        LevelSelector.SetActive(true);
        backButton.gameObject.SetActive(true);
        Level1.onClick.AddListener(PlayLevel1);
        Level2.onClick.AddListener(PlayLevel2);
    }

    public void PlayLevel1()
    {
        AudioSource.PlayOneShot(LevelSound);
        SceneManager.LoadScene("Level1");
    }

    public void PlayLevel2()
    {
        AudioSource.PlayOneShot(LevelSound);
        SceneManager.LoadScene("Level2");
    }

    public void backSelect()
    {
        AudioSource.PlayOneShot(backSound);
        LobbyMenu.SetActive(true);
        LevelSelector.SetActive(false);
        backButton.gameObject.SetActive(false);
    }
}
