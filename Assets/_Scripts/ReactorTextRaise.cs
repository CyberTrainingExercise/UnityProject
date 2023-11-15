using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ReactorTextRaise : MonoBehaviour
{
    [SerializeField]
    private String prefix;
    [SerializeField]
    private String postfix;
    [SerializeField]
    private int low;
    [SerializeField]
    private int high;
    [SerializeField]
    private float volitility = 1;
    [SerializeField]
    private bool shouldRise = false;
    [SerializeField]
    private float riseRate = 8;
    [SerializeField]
    private bool cutoff = false;
    [SerializeField]
    private TMP_Text lastUpdate;

    private float time;
    private int num;
    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        num = UnityEngine.Random.Range(low, high);
        text.text = prefix + num + postfix;
        if (shouldRise) {
            num = low;
            text.text = prefix + num + postfix;
        }
        time = Time.time + 5;
    }

    private void OnEnable()
    {
        // every time a "shouldFall" is enabled, it resets to where it should fall from
        text = GetComponent<TMP_Text>();
        if (shouldRise) {
            num = low;
            text.text = prefix + num + postfix;
        }
    }

    void Update()
    {
        if (Time.time >= time)
        {
            num = (int)(num + UnityEngine.Random.Range(-1, 1) * volitility);
            if (shouldRise && num < high - ((high - low) / 8)) {
                // if in the upper 7/8ths raise pseudoexponentially
                num += (int)((high - num) / riseRate);
            } else if (cutoff) {
                num = low;
            }
            if (num < low) {
                num = low;
            }
            if (num > high) {
                num = high;
            }
            text.text = prefix + num + postfix;
            time = Time.time + 5;
            if (lastUpdate != null) {
                string now = "Last Update: ";
                if (DateTime.Now.Hour < 10) {
                    now += "0";
                }
                now += DateTime.Now.Hour + ":";
                if (DateTime.Now.Minute < 10) {
                    now += "0";
                }
                now += DateTime.Now.Minute + ".";
                if (DateTime.Now.Second < 10) {
                    now += "0";
                }
                now += DateTime.Now.Second + " EST";
                lastUpdate.text = now;
            }
        } 
    }
}
