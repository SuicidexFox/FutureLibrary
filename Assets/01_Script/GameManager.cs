using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
    public PlayerController playerController;
    public Menu menu;
    private Interactable currentLines;
    
    private void Awake() { instance = this; }

    void Start()
    {
	    inUI = true;
	    menu.tutorial.SetActive(true);
	    menu.tutorialButton.Select();
	    playerController.DeactivateInput();
    }


    ///////////////////////////////////// DialogUI \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    [Header("Dialog")] 
    public bool inUI;
    [SerializeField] private GameObject dialogUI;
    [SerializeField] private TextMeshProUGUI textDialog;
    [SerializeField] private Button buttonDialog;
    
    public void ShowDialogUI(Interactable interactable)
    {
	    playerController.DeactivateInput();
	    dialogUI.SetActive(true);
	    inUI = true;
	    RuntimeManager.PlayOneShot(interactable.sound);
	    textDialog.SetText(interactable.Itemtext);
	    StartCoroutine(SelectButton());
    }
    IEnumerator SelectButton() { yield return new WaitForSeconds(1); buttonDialog.Select(); }
    public void CloseDialogeUI()
    {
	    playerController.ActivateInput();
	    dialogUI.SetActive(false);
	    buttonDialog.gameObject.SetActive(false);
	    inUI = false;
	    if (currentLines == null) { return; }
	    currentLines._lineEvent.Invoke();
    }
   
    
    
    ///////////////////////////////////// Pause \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    [Header("Pause")] 
    public bool pause;
    [SerializeField] private GameObject pauseUI;
    
    
    public void TogglePause() 
    {
	    if (inUI == true) { return; }
	    pause = !pause;
	    pauseUI.SetActive(pause);
            
	    if (pause)
	    { 
		    playerController.DeactivateInput();
		    EventSystem.current.SetSelectedGameObject(menu.buttonMain.gameObject);
            RuntimeManager.PlayOneShot("event:/SFX/ui-sci-fi-bubble-approve/ui-sci-fi-bubble-approve-05");
		    Time.timeScale = 0.0f;
	    }
	    else
	    { 
		    playerController.ActivateInput();
		    menu.main.SetActive(true);
		    menu.settings.SetActive(false);
		    menu.sound.SetActive(false);
		    menu.controls.SetActive(false);
		    RuntimeManager.PlayOneShot("event:/SFX/ui-sci-fi-bubble-approve/ui-sci-fi-bubble-approve-06");
		    Time.timeScale = 1.0f;
	    } 
    }
    
    
    
}
