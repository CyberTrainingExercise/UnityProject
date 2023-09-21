using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleHTTP;
using TMPro;

namespace BoNY {
public class BoNYManager : MonoBehaviour
{
    [Header("Text")]
    [SerializeField]
    private string connectionTextString = "BoNY Command Center";
    private string sat0TextString = "BoNY0";
    private string sat1TextString = "BoNY1";
    private string sat2TextString = "BoNY2";

    [Header("Functional")]
    [SerializeField]
    private int activeSats = 30;

    [SerializeField]
    private int apiUpdateDelay = 5;
    private DateTime lastApiUpdate;

    [SerializeField]
    private string statusServer = "http://localhost:8000/ui";

    [Header("System - DO NOT EDIT BELOW")]
    [SerializeField]
    private GameObject adminControls;
    [SerializeField]
    private GameObject[] sats;
    [SerializeField]
    private Animator[] satAnimators;
    [SerializeField]
    private SpriteRenderer[] satSprites;
    [SerializeField]
    private TMP_Text connectionText;
    [SerializeField]
    private TMP_Text timeUntilText;
    [SerializeField]
    private GameObject warningText;
    [SerializeField]
    private TMP_Text leaderboardText;

    private StatusData data;

    private DateTime startTime;

    [SerializeField]
    private Transform[] satSpinners;

    [SerializeField]
    private float[] satSpinnerSpeeds;

    private float[] teamScores;
    private DateTime[] teamFinishes = new DateTime[6];
    private float[] highestScore = new float[6];
    private DateTime defaultTime = new DateTime();
    [SerializeField]
    private string[] teamNames;

    private void Start()
    {
        lastApiUpdate = DateTime.Now;
        startTime = DateTime.Now;
        for (int i = 0; i < 30; i++) {
            satAnimators[i] = sats[i].GetComponentInChildren<Animator>();
            satSprites[i] = sats[i].GetComponentInChildren<SpriteRenderer>();
        }
        SetActiveSats(activeSats.ToString());
    }
    private void Update()
    {
        if (DateTime.Now > lastApiUpdate.AddSeconds(apiUpdateDelay))
        {
            lastApiUpdate = DateTime.Now;
            StartCoroutine(ApiUpdate());
        }
        for (int i = 0; i < satSpinners.Length; i++) {
            satSpinners[i].RotateAround(Vector3.zero, Vector3.forward, Time.deltaTime * satSpinnerSpeeds[i]);
        }
        if (Input.GetKeyDown(KeyCode.A) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) {
            adminControls.SetActive(!adminControls.activeSelf);
        }
    }
    public void SetActiveSats(string input)
    {
        for (int i = 0; i < 30; i++) {
            sats[i].SetActive(true);
            if (i >= int.Parse(input)) {
                sats[i].SetActive(false);
            }
        }
    }
    public void UpdateConnectIP(string input)
    {
        statusServer = input;
        print("Update status server:" + input);
    }

    private long DateTimeToUnix(DateTime MyDateTime)
    {
        TimeSpan timeSpan = MyDateTime - new DateTime();

        return (long)timeSpan.TotalSeconds;
    }

    private void UpdateLeaderboardText () {
        // find rankings by creating alphabetical strings for each satellite
        // those strings are then sorted via arrays.sort() and recreated as useful information on the leaderboard
        // it's jank, but it works well enough for a one off event
        // rank first by number of satellite takedowns, then by timestamp (older if better)
        string[] ranks = new string[6];
        for (int i = 0; i < 6; i++)
        {
            ranks[i] = (6 - teamScores[i]) + "$" + DateTimeToUnix(teamFinishes[i]) + "#" + teamNames[i] + " Team";
        }
        Array.Sort(ranks);
        // update UI
        leaderboardText.text = "Satellite Takedown Leaderboard:\n";
        for (int i = 0; i < 6; i++)
        {
            //leaderboardText.text += teamNames[i] + " Team:\t" + teamScores[i] + "\n";
            int score = Int32.Parse(ranks[i].Split("$")[0]);
            leaderboardText.text += (i + 1) + ". " + ranks[i].Split("#")[1] + " - " + (6 - score) + "\n";
            print("Rank: " + ranks[i]);
        }
    }

    private void UpdateScore(int sat) {
        // 0-4,5-9,10-14,15-20,etc.
        int i = 0;
        if (sat < 5) {
            i = 0;
        } else if (sat < 10) {
            i = 1;
        } else if (sat < 15) {
            i = 2;
        } else if (sat < 20) {
            i = 3;
        } else if (sat < 25) {
            i = 4;
        } else if (sat < 30) {
            i = 5;
        }
        teamScores[i] += 1;
        // if this is the first time to a new score, save their time
        if (teamScores[i] > highestScore[i])
        {
            teamFinishes[i] = DateTime.Now;
            highestScore[i] = teamScores[i];
        }
    }

    private void SetSat(int sat, string status) {
        if (status == "offline")
        {
            satAnimators[sat].SetBool("IsOffline", true);
            satSprites[sat].color = Color.red;
            UpdateScore(sat);
        } else if (status == "sleep")
        {
            satAnimators[sat].SetBool("IsOffline", true);
            satSprites[sat].color = Color.blue;
        } else {
            satAnimators[sat].SetBool("IsOffline", false);
            satSprites[sat].color = Color.white;
        }
    }
    private IEnumerator ApiUpdate()
    {
        // Create the request object
        Request request = new Request(statusServer);

        // Instantiate the client
        Client http = new Client();
        print("Sending request to " + statusServer);
        // Send the request
        yield return http.Send(request);
        print("Got request");
        print("Request: " + request);

        // Use the response if the request was successful, otherwise print an error
        if (http.IsSuccessful())
        {
            Response resp = http.Response();
            // StatusData dataOut = new StatusData("ok", "ok", "ok");
            // string json = JsonUtility.ToJson(dataOut);
            print("Resp data: " + resp);
            print("Raw:" + resp.RawBody());
            print("Body:" + resp.Body());
            data = JsonUtility.FromJson<StatusData>(resp.Body());
            if (resp.Status().ToString() == "200")
            {
                connectionText.text = connectionTextString + ": " + DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + " EST";
            }
            print("Json data: " + data);
            // satText[0].text = "Sat0: " + data.status0;
            // satText[1].text = "Sat1: " + data.status1;
            // satText[2].text = "Sat2: " + data.status2;
            teamScores = new float[6];
            SetSat(0, data.status0);
            SetSat(1, data.status1);
            SetSat(2, data.status2);
            SetSat(3, data.status3);
            SetSat(4, data.status4);
            SetSat(5, data.status5);
            SetSat(6, data.status6);
            SetSat(7, data.status7);
            SetSat(8, data.status8);
            SetSat(9, data.status9);
            SetSat(10, data.status10);
            SetSat(11, data.status11);
            SetSat(12, data.status12);
            SetSat(13, data.status13);
            SetSat(14, data.status14);
            SetSat(15, data.status15);
            SetSat(16, data.status16);
            SetSat(17, data.status17);
            SetSat(18, data.status18);
            SetSat(19, data.status19);
            SetSat(20, data.status20);
            SetSat(21, data.status21);
            SetSat(22, data.status22);
            SetSat(23, data.status23);
            SetSat(24, data.status24);
            SetSat(25, data.status25);
            SetSat(26, data.status26);
            SetSat(27, data.status27);
            SetSat(28, data.status28);
            SetSat(29, data.status29);
            UpdateLeaderboardText();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());
        } else {
            Debug.Log("error: " + http.Error());
        }
    }
    private void SetStatus(int sat, bool status)
    {
        //satText[sat].color = status ? Color.white : Color.red;
        satAnimators[sat].SetBool("IsOffline", status);
        satSprites[sat].color = status ? Color.white : Color.red;
    }
}
[Serializable]
public class StatusData
{
    // in this format for REST request serialization
    public String status0;
    public String status1;
    public String status2;
    public String status3;
    public String status4;
    public String status5;
    public String status6;
    public String status7;
    public String status8;
    public String status9;
    public String status10;
    public String status11;
    public String status12;
    public String status13;
    public String status14;
    public String status15;
    public String status16;
    public String status17;
    public String status18;
    public String status19;
    public String status20;
    public String status21;
    public String status22;
    public String status23;
    public String status24;
    public String status25;
    public String status26;
    public String status27;
    public String status28;
    public String status29;

    public StatusData(String status0, String status1, String status2, String status3, String status4,
    String status5, String status6, String status7, String status8, String status9,
    String status10, String status11, String status12, String status13, String status14,
    String status15, String status16, String status17, String status18, String status19,
    String status20, String status21, String status22, String status23, String status24,
    String status25, String status26, String status27, String status28, String status29
    )
    {
        this.status0 = status0;
        this.status1 = status1;
        this.status2 = status2;
        this.status3 = status3;
        this.status4 = status4;
        this.status5 = status5;
        this.status6 = status6;
        this.status7 = status7;
        this.status8 = status8;
        this.status9 = status9;
        this.status10 = status10;
        this.status11 = status11;
        this.status12 = status12;
        this.status13 = status13;
        this.status14 = status14;
        this.status15 = status15;
        this.status16 = status16;
        this.status17 = status17;
        this.status18 = status18;
        this.status19 = status19;
        this.status20 = status20;
        this.status21 = status21;
        this.status22 = status22;
        this.status23 = status23;
        this.status24 = status24;
        this.status25 = status25;
        this.status26 = status26;
        this.status27 = status27;
        this.status28 = status28;
        this.status29 = status29;
    }
    public bool IsOnline(int sat)
    {
        if (sat == 0)
        {
            return status0 == "ok";
        } else if (sat == 1)
        {
            return status1 == "ok";
        } else if (sat == 2)
        {
            return status2 == "ok";
        } else if (sat == 3)
        {
            return status3 == "ok";
        } else if (sat == 4)
        {
            return status4 == "ok";
        } else if (sat == 5)
        {
            return status5 == "ok";
        } else if (sat == 6)
        {
            return status6 == "ok";
        } else if (sat == 7)
        {
            return status7 == "ok";
        } else if (sat == 8)
        {
            return status8 == "ok";
        } else if (sat == 9)
        {
            return status9 == "ok";
        } else if (sat == 10)
        {
            return status10 == "ok";
        } else if (sat == 11)
        {
            return status11 == "ok";
        } else if (sat == 12)
        {
            return status12 == "ok";
        } else if (sat == 13)
        {
            return status13 == "ok";
        } else if (sat == 14)
        {
            return status14 == "ok";
        } else if (sat == 15)
        {
            return status15 == "ok";
        } else if (sat == 16)
        {
            return status16 == "ok";
        } else if (sat == 17)
        {
            return status17 == "ok";
        } else if (sat == 18)
        {
            return status18 == "ok";
        } else if (sat == 19)
        {
            return status19 == "ok";
        } else if (sat == 20)
        {
            return status20 == "ok";
        } else if (sat == 21)
        {
            return status21 == "ok";
        } else if (sat == 22)
        {
            return status22 == "ok";
        } else if (sat == 23)
        {
            return status23 == "ok";
        } else if (sat == 24)
        {
            return status24 == "ok";
        } else if (sat == 25)
        {
            return status25 == "ok";
        } else if (sat == 26)
        {
            return status26 == "ok";
        } else if (sat == 27)
        {
            return status27 == "ok";
        } else if (sat == 28)
        {
            return status28 == "ok";
        } else if (sat == 29)
        {
            return status29 == "ok";
        }
        return false;
    }
}
}