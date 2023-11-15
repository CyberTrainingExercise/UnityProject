using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

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
    private SpriteRenderer boltSprite;
    [SerializeField]
    private TMP_Text generatorCharge;
    [SerializeField]
    private TMP_Text generatorTemp;
    [SerializeField]
    private TMP_Text logs;
    private int lineCount;
    private void Start(){
        lineCount = 12;
    }
    private void Update()
    {
        if (Input.GetKeyDown("1")) {
            TurnOn();
        }
        if (Input.GetKeyDown("2")) {
            TurnOff();
        }
        if (Input.GetKeyDown("5")) {
            CommsOff();
        }
        if (Input.GetKeyDown("6")) {
            CommsOn();
        }
        if (Input.GetKeyDown("8")) {
            DefenseDown();
        }
        if (Input.GetKeyDown("9")) {
            DefenseUp();
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
        if (lineCount < 24){
            logs.text += now + eventName;
            lineCount++;
        }
        else{
            string[] tmp = logs.text.Split('\n').Skip(1).ToArray();
            logs.text = String.Join('\n', tmp) + now + eventName;
        }
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
    private void CommsOff(){
        AddLogEvent("<color=red>FOB COMMS OFFLINE<color=white>");
    }
    private void CommsOn(){
        AddLogEvent("<color=green>FOB COMMS ONLINE<color=white>");
    }
    private void DefenseDown(){
        AddLogEvent("<color=green>AIR DEFENSES OFFLINE<color=white>");
    }
    private void DefenseUp(){
        AddLogEvent("<color=red>AIR DEFENSES ONLINE<color=white>");
    }

    private void DisableGenerators(){
        boltSprite.color = Color.red;
        generatorCharge.text = "Backup Generator Charge: 0 MWH";
        generatorTemp.text = "Backup Generator Temp: Unknown";
    }
}
