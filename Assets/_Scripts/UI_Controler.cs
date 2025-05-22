using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controler : MonoBehaviour
{
    public static UI_Controler instance;
    [Header("Elements")]
    [SerializeField]GameObject deathScreen;
     [SerializeField]TMP_Text deathText;
     [SerializeField]Slider healthSlider;
    [SerializeField]TMP_Text killText;
    [SerializeField]TMP_Text deathLabelText;

    [Header("Leaderboard")]
    public GameObject leaderBroad;
    public LeaderBroadPlayer leaderbroadPlayer;

    [Header("EndScreen")]
    [SerializeField] public GameObject endScreen;

    void Awake()
    {
        instance=this;

    }
    public void ShowDeathScreen(string damager,bool isActive){
        deathText.text ="You are killed by "+damager;
        deathScreen.SetActive(isActive);
    }
    public void OnHealthChanged(int currentHealth){
        healthSlider.value=currentHealth;
    }
    public void setMaxValue(int maxHealth){
        healthSlider.maxValue=maxHealth;
    }

    public TMP_Text GetKillText(){
        return killText;
    }
    public TMP_Text GetDeathLabel(){
        return deathLabelText;
    }

    
}
