using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class Menu : MonoBehaviour
{
    ///////////////////////////////////// Variable \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    
    public Animator animatorFade;
    public string url;

    [Header("Entry")] 
    public GameObject main;
    public GameObject settings;
    public GameObject sound;
    public GameObject controls;
    public GameObject fade;
    public GameObject tutorial;
    public Button tutorialButton;

    [Header("Button")] 
    public Button buttonMain;
    public Button buttonSettings;
    public Button buttonControls;

    [Header("Slider")] public Slider master;
    [SerializeField] private Slider music;
    [SerializeField] private Slider effects;
    

    ///////////////////////////////////// Events \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    private void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;
        ////Fade
        fade.SetActive(true);
        ////SliderSetup
        SetupSlider(master, "bus:/Master");
        SetupSlider(music, "bus:/Master/Music");
        SetupSlider(effects, "bus:/Master/SFX");
        
        StartCoroutine(CFade()); 
    }
    
    /// Start MainMenu
    IEnumerator CFade()
    {
        yield return new WaitForSeconds(2f); 
        fade.SetActive(false);
    }
    

    ///////////////////////////////////// SoundVolume \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    private void SetupSlider(Slider slider, string busPath) { RuntimeManager.GetBus(busPath).getVolume(out float _volume); slider.value = 0.5f; }
    public void SetMasterVolume() { RuntimeManager.GetBus("bus:/Master").setVolume(master.value); }
    public void SetMusicVolume() { RuntimeManager.GetBus("bus:/Master/Music").setVolume(music.value); }
    public void SetSFXVolume() { RuntimeManager.GetBus("bus:/Master/SFX").setVolume(effects.value); }



    ///////////////////////////////////// Toggle \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
 
    public void ToggleSettings(bool Settings)
    {
        if (Settings == true)
        {
            main.SetActive(false);
            settings.SetActive(true);
            buttonSettings.Select();
        }
        else
        {
            main.SetActive(true);
            settings.SetActive(false);
            buttonMain.Select();
        }
    }
    public void ToggleSound(bool Sound)
    {
        if (Sound == true)
        {
            settings.SetActive(false);
            sound.SetActive(true);
            master.Select();
        }
        else
        {
            settings.SetActive(true);
            sound.SetActive(false);
            buttonSettings.Select();
        }
    }
    public void ToggleControls(bool Controls)
    {
        if (Controls == true)
        {
            settings.SetActive(false);
            controls.SetActive(true);
            buttonControls.Select();
        }
        else
        {
            settings.SetActive(true);
            controls.SetActive(false);
            buttonSettings.Select();
        }
    }


    ///////////////////////////////////// MainMenu \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    /// Play
    public void StartGame() { fade.SetActive(true); animatorFade.Play("FadeOut"); StartCoroutine(CStartGame()); }
    IEnumerator CStartGame() { yield return new WaitForSeconds(2f); SceneManager.LoadScene("3D 1"); }
    /// Credits
    public void OpenWebURL() { Application.OpenURL(url); }
    /// Quit
    public void QuitGame() { fade.SetActive(true); animatorFade.Play("FadeOut"); StartCoroutine(CQuit()); }
    IEnumerator CQuit() { yield return new WaitForSeconds(2f); Application.Quit(); }

    
    ///////////////////////////////////////// Pause \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    
    /// Play
    public void Continue() { GameManager.instance.TogglePause(); }

    /// LeaveGame
    public void MainMenuBack()
    {
        GameManager.instance.playerController.DeactivateInput(); 
        GameManager.instance.TogglePause(); fade.SetActive(true); 
        animatorFade.Play("FadeOut"); 
        StartCoroutine(CMainMenu());
    }
    IEnumerator CMainMenu() { yield return new WaitForSeconds(2); SceneManager.LoadScene("MainMenu"); }

   
}
