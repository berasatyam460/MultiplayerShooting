using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class MatchManager : MonoBehaviour
{
    // Start is called before the first frame up
    public static MatchManager instance;
    void Awake()
    {
        instance=this;
    }
    void Start()
    {
        if(!PhotonNetwork.IsConnected){
            SceneManager.LoadScene(0);
        }
    }
}
