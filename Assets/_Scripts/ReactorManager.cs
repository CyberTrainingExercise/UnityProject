using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ReactorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject off;
    [SerializeField]
    private GameObject on;
    [SerializeField]
    private SpriteRenderer statusSprite;
    [SerializeField]
    private Sprite statusOnSprite;
    [SerializeField]
    private Sprite statusOffSprite;
    [SerializeField]
    private SpriteRenderer powerSprite;
    [SerializeField]
    private Sprite powerOnSprite;
    [SerializeField]
    private Sprite powerOffSprite;
    [SerializeField]
    private TMP_Text logs;

    private void Update()
    {
        if (Input.GetKeyDown("1")) {
            TurnOn();
        }
        if (Input.GetKeyDown("2")) {
            TurnOff();
        }
    }
    private void AddLogEvent(string eventName)
    {
        string now = "\n";
        if (DateTime.Now.Hour < 10) {
            now += "0";
        }
        now += "" + DateTime.Now.Hour;
        if (DateTime.Now.Minute < 10) {
            now += "0";
        }
        now += DateTime.Now.Minute + ": ";
        logs.text += now + eventName;
    }

    private void TurnOff()
    {
        on.SetActive(false);
        off.SetActive(true);
        statusSprite.sprite = statusOffSprite;
        powerSprite.sprite = powerOffSprite;
        AddLogEvent("Powering off...");
    }
    private void TurnOn()
    {
        on.SetActive(true);
        off.SetActive(false);
        statusSprite.sprite = statusOnSprite;
        powerSprite.sprite = powerOnSprite;
        AddLogEvent("Powering on...");
    }
}
