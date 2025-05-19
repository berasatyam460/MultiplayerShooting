using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBroadPlayer : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] TMP_Text playerNameLabel;
    [SerializeField] TMP_Text killsText;
    [SerializeField] TMP_Text deathText;


    public void SetDetails(string name, int kills, int deaths)
    {
        playerNameLabel.text = name;
        killsText.text = kills.ToString();
        deathText.text = deaths.ToString();
    }
}
