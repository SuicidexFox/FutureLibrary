using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject objTransform;
    public GameObject playerController;

    

    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.menu.fade.SetActive(true);
        GameManager.instance.menu.animatorFade.Play("Teleport");
        StartCoroutine(CFadeOut());
        GameManager.instance.playerController.DeactivateInput();
        RuntimeManager.PlayOneShot("event:/SFX/Teleports/Teleport");
    }

    IEnumerator CFadeOut()
    {
        yield return new WaitForSeconds(1);
        playerController.GetComponent<CharacterController>().enabled = false;
        playerController.transform.position = objTransform.transform.position;
        playerController.transform.rotation = objTransform.transform.rotation;
        playerController.GetComponent<CharacterController>().enabled = true;
        StartCoroutine(CFadeIn());
    }

    IEnumerator CFadeIn()
    { 
        yield return new WaitForSeconds(1);   
        GameManager.instance.playerController.ActivateInput();
        GameManager.instance.menu.fade.SetActive(false);
    }
    
}