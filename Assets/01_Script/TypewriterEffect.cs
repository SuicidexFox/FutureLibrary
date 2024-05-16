using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class TypewriterEffect : MonoBehaviour
{
    public float typingSpeed = 0.05f;
    public TextMeshProUGUI _text;
    public Button _button;

    private string fullText;
    private string currentText = "";


    private void Start()
    {
        if (_text == null) { _text = GetComponent<TextMeshProUGUI>(); }
        fullText = _text.text;
        _text.text = "";
        StartCoroutine(ShowText());
        StartCoroutine(ButtonSelect());
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            _text.text = currentText;
            RuntimeManager.PlayOneShot("event:/SFX/Typewriter/typewriter 2");
            yield return new WaitForSeconds(typingSpeed);
        }
    }
    
    

    IEnumerator ButtonSelect()
    {
        yield return new WaitForSeconds(2);
        _button.gameObject.SetActive(true);
        _button.Select();
    }
}
