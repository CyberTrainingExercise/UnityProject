using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TypingText : MonoBehaviour
{
    [SerializeField]
    private String textToDisplay;
    [SerializeField]
    private float timeToWait;
    private float timeToStart;
    [SerializeField]
    private bool stopTyping = false;
    [SerializeField]
    private float waitBetweenChars = 0.2f;

    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        timeToStart = Time.time + timeToWait;
        text.text = "";
    }
    private void Update()
    {
        if (Time.time >= timeToStart)
        {
            StopCoroutine("PrintText");
            StartCoroutine("PrintText");
            timeToStart = float.MaxValue;
        }
        if (Input.GetKey("space"))
        {
            StopCoroutine("PrintText");
            timeToStart = Time.time + timeToWait;
            text.text = "";
        }
    }
    private IEnumerator PrintText()
    {
        int i;
        for (i = 0; i < textToDisplay.Length + 1; i++)
        {
            text.text = textToDisplay.Substring(0, i);
            if (i % 2 == 0)
            {
                text.text = text.text + "|";
            }
            yield return new WaitForSeconds(waitBetweenChars);
        }
        while (!stopTyping)
        {
            if (i++ % 2 == 0)
            {
                text.text = textToDisplay + "|";
            } else {
                text.text = textToDisplay;
            }
            yield return new WaitForSeconds(waitBetweenChars);
        }
        text.text = textToDisplay;
    }
}
