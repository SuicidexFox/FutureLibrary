using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public int currentCount = 0;
    public int targetCount = 9; 
    public GameObject activateObject;

    private bool activated;

    private void Start()
    {
        if (activateObject != null)
        {
            activateObject.SetActive(false);
        }
    }

    public void IncreaseCount()
    {
        if (activated) return;
        currentCount++;
            if (currentCount >= targetCount)
            {
               activateObject.SetActive(true);
               activated = true;
            }
    }
}