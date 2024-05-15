using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    
    
    ///////////////////////////////////// Variable \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public UnityEvent _onInteract;
    
    /// Sound
    public EventReference sound;
    
    /// Quest Dialog
    [TextArea(1, 3)]public string Itemtext;
    public UnityEvent _lineEvent;
    
    /// Billboard
    public SpriteRenderer billboard;
    private Camera cam;

    ///////////////////////////////////// Event \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public void Start() { cam = Camera.main; billboard.enabled = false; }
    
    public void ShowDialog() { GameManager.instance.ShowDialogUI(this); }

    private void LateUpdate()
    {
        if (billboard == null) { return; }
        billboard.transform.rotation = cam.transform.rotation;
    }
    private void OnTriggerEnter(Collider other) { if (other.GetComponent<PlayerController>() == null) { return; } billboard.enabled = true; }
    private void OnTriggerExit(Collider other) { if (other.GetComponent<PlayerController>() == null) { return; } billboard.enabled = false;}


    public void DestroyBillboard() { GetComponent<Collider>().enabled =false; Destroy(billboard.gameObject); }
    public void Destroy() { Destroy(this); }
}
