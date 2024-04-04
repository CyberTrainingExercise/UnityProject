using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleHTTP;
using TMPro;

public class Symposium24Manager : MonoBehaviour
{
    [Header("Text")]
    [SerializeField]
    private string connectionTextString = "Starcom Command Center";
    private string sat0TextString = "Starcom0";
    private string sat1TextString = "Starcom1";
    private string sat2TextString = "Starcom2";

    [Header("Functional")]
    [SerializeField]
    private int apiUpdateDelay = 5;
    private DateTime lastApiUpdate;

    [Header("System - DO NOT EDIT BELOW")]
    [SerializeField]
    private TMP_Text[] statusText;
    [SerializeField]
    private Animator[] dishAnimators;
    [SerializeField]
    private SpriteRenderer[] dishSprites;
    [SerializeField]
    private TMP_Text connectionText;
    [SerializeField]
    private TMP_Text timeUntilText;
    [SerializeField]

    private GameObject warningText;

    private StationData data;

    private DateTime startTime;

    private void Start()
    {
        lastApiUpdate = DateTime.Now;
        startTime = DateTime.Now;
    }
    private void Update()
    {
        if (DateTime.Now > lastApiUpdate.AddSeconds(apiUpdateDelay))
        {
            lastApiUpdate = DateTime.Now;
            StartCoroutine(ApiUpdate());
        }
    }
    private void UpdateConnectionStatus(int timeInPeriod, string connectionStatus, StationData data, int satX)
    {
        // satX
        int satY = (satX + 1) % 3;
        int satZ = (satX + 2) % 3;
        connectionText.text = connectionTextString + ": " + connectionStatus;
        warningText.SetActive(data.IsOnline(satX));
        if (data.IsOnline(satX))
        {
            // looking for offline
            if (!data.IsOnline(satY))
            {
                timeUntilText.text = "Time Until Offline: " + (((10 * satX) + 10) - timeInPeriod);
            } else if (!data.IsOnline(satZ))
            {
                timeUntilText.text = "Time Until Offline: " + (((10 * satX) + 20) - timeInPeriod);
            } else {
                timeUntilText.text = "Time Until Offline: Infinite";
            }
        } else {
            // looking for online
            if (data.IsOnline(satY))
            {
                timeUntilText.text = "Time Until Online: " + (((10 * satX) + 10) - timeInPeriod);
            } else if (data.IsOnline(satZ))
            {
                timeUntilText.text = "Time Until Online: " + (((10 * satX) + 20) - timeInPeriod);
            } else {
                timeUntilText.text = "Time Until Online: Infinite";
            }
        }
    }
    private void UpdateAllConnectionStatus()
    {
        if (data == null) return;
        int timeInPeriod = (DateTime.Now - startTime).Minutes % 30;
        if (timeInPeriod < 10)
        {
            UpdateConnectionStatus(timeInPeriod, data.status0, data, 0);
        } else if (timeInPeriod < 20)
        {
            UpdateConnectionStatus(timeInPeriod, data.status1, data, 1);
        } else {
            UpdateConnectionStatus(timeInPeriod, data.status2, data, 2);
        }
    }
    private void UpdateAllConnectionStatusOLD()
    {
        if (data == null) return;
        int timeInPeriod = (DateTime.Now - startTime).Minutes % 30;
        if (timeInPeriod < 10)
        {
            // sat0
            connectionText.text = connectionTextString + ": " + data.status0;
            warningText.SetActive(data.IsOnline(0));
            if (data.IsOnline(0))
            {
                // looking for offline
                if (!data.IsOnline(1))
                {
                    timeUntilText.text = "Time Until Offline: " + (10 - timeInPeriod);
                } else if (!data.IsOnline(2))
                {
                    timeUntilText.text = "Time Until Offline: " + (20 - timeInPeriod);
                } else {
                    timeUntilText.text = "Time Until Offline: Infinite";
                }
            } else {
                // looking for online
                if (data.IsOnline(1))
                {
                    timeUntilText.text = "Time Until Online: " + (10 - timeInPeriod);
                } else if (data.IsOnline(2))
                {
                    timeUntilText.text = "Time Until Online: " + (20 - timeInPeriod);
                } else {
                    timeUntilText.text = "Time Until Online: Infinite";
                }
            }
        } else if (timeInPeriod < 20)
        {
            // sat1
            connectionText.text = connectionTextString + ": " + data.status1;
            warningText.SetActive(data.IsOnline(1));
            if (data.IsOnline(1))
            {
                // looking for offline
                if (!data.IsOnline(2))
                {
                    timeUntilText.text = "Time Until Offline: " + (20 - timeInPeriod);
                } else if (!data.IsOnline(0))
                {
                    timeUntilText.text = "Time Until Offline: " + (30 - timeInPeriod);
                } else {
                    timeUntilText.text = "Time Until Offline: Infinite";
                }
            } else {
                // looking for online
                if (data.IsOnline(2))
                {
                    timeUntilText.text = "Time Until Online: " + (20 - timeInPeriod);
                } else if (data.IsOnline(0))
                {
                    timeUntilText.text = "Time Until Online: " + (30 - timeInPeriod);
                } else {
                    timeUntilText.text = "Time Until Online: Infinite";
                }
            }
        } else {
            // sat2
            connectionText.text = connectionTextString + ": " + data.status2;
            warningText.SetActive(data.IsOnline(2));
            if (data.IsOnline(2))
            {
                // looking for offline
                if (!data.IsOnline(0))
                {
                    timeUntilText.text = "Time Until Offline: " + (30 - timeInPeriod);
                } else if (!data.IsOnline(1))
                {
                    timeUntilText.text = "Time Until Offline: " + (40 - timeInPeriod);
                } else {
                    timeUntilText.text = "Time Until Offline: Infinite";
                }
            } else {
                // looking for online
                if (data.IsOnline(0))
                {
                    timeUntilText.text = "Time Until Online: " + (30 - timeInPeriod);
                } else if (data.IsOnline(1))
                {
                    timeUntilText.text = "Time Until Online: " + (40 - timeInPeriod);
                } else {
                    timeUntilText.text = "Time Until Online: Infinite";
                }
            }
        }
        // find what satellite is in view
        // if yes, find time till out of view
        // if no, find time till back in view
    }
    private IEnumerator ApiUpdate()
    {
        // Create the request object
        Request request = new Request("http://192.168.1.11:5001/status");

        // Instantiate the client
        Client http = new Client();
        // Send the request
        yield return http.Send(request);

        // Use the response if the request was successful, otherwise print an error
        if (http.IsSuccessful())
        {
            Response resp = http.Response();
            // StatusData dataOut = new StatusData("ok", "ok", "ok");
            // string json = JsonUtility.ToJson(dataOut);
            // print("Json data: " + json);
            data = JsonUtility.FromJson<StationData>(resp.Body());
            print("Json data: " + data);
            statusText[0].text = "Colorado: " + data.status0;
            statusText[1].text = "Africa: " + data.status1;
            statusText[2].text = "Asia: " + data.status2;
            if (data.status0 == "offline") 
            {
                statusText[0].color = Color.red;
                dishAnimators[0].SetBool("Down", true);
                dishSprites[0].color = Color.red;
            } else {
                statusText[0].color = Color.green;
                dishAnimators[0].SetBool("Down", false);
                dishSprites[0].color = Color.green;
            }
            if (data.status1 == "offline")
            {
                statusText[1].color = Color.red;
                dishAnimators[1].SetBool("Down", true);
                dishSprites[1].color = Color.red;
            } else {
                statusText[1].color = Color.green;
                dishAnimators[1].SetBool("Down", false);
                dishSprites[1].color = Color.green;
            }
            if (data.status2 == "offline")
            {
                statusText[2].color = Color.red;
                dishAnimators[2].SetBool("Down", true);
                dishSprites[2].color = Color.red;
            } else {
                statusText[2].color = Color.green;
                dishAnimators[2].SetBool("Down", false);
                dishSprites[2].color = Color.green;
            }
            //UpdateAllConnectionStatus();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());
        } else {
            Debug.Log("error: " + http.Error());
        }
    }
    private void SetStatus(int sat, bool status)
    {
        statusText[sat].color = status ? Color.white : Color.red;
        //dishAnimators[sat].SetBool("Down", status);
        dishSprites[sat].color = status ? Color.white : Color.red;
    }
}
[Serializable]
public class StationData
{
    // in this format for REST request serialization
    public String status0;
    public String status1;
    public String status2;

    public StationData(String status0, String status1, String status2)
    {
        this.status0 = status0;
        this.status1 = status1;
        this.status2 = status2;
    }
    public bool IsOnline(int sat)
    {
        if (sat == 0)
        {
            return status0 == "OPERATIONAL";
        } else if (sat == 1)
        {
            return status1 == "OPERATIONAL";
        } else {
            return status2 == "OPERATIONAL";
        }
    }
}