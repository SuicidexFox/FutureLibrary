using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class Collectable : MonoBehaviour
{
   private GameState gameState;
   private bool collected;

   private void Start()
   {
      gameState = FindObjectOfType<GameState>();
   }

   public void Collected()
   {
      if (!collected)
      {
         gameState.IncreaseCount();
         collected = true;
         RuntimeManager.PlayOneShot("event:/SFX/Collect/Collect");
      }
   }
}