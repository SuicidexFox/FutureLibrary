using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public float typingSpeed = 0.05f;
    public TextMeshProUGUI _text;

    private string fullText;
    private string currentText = "";


    private void Start()
    {
        if (_text == null)
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        fullText = _text.text;
        _text.text = "";
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            _text.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }
        
    }
}
