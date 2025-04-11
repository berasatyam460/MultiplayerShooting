using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

public class PlayerSpwaner : MonoBehaviour
{   
    public static PlayerSpwaner instance;
    [SerializeField]GameObject playerPrefab;
    [SerializeField]GameObject deathFx;
    private GameObject player;

    [SerializeField]List<Transform>spwanPointList=new List<Transform>();


    private  void Awake()
    {
        instance=this;
    }
    void Start()
    {
        if(PhotonNetwork.IsConnected){
            SpwanPlayer();

        }
    }
   public void SpwanPlayer(){
        Transform spwanPoint=GetSpwanPoint();
       player= PhotonNetwork.Instantiate(playerPrefab.name,spwanPoint.position,spwanPoint.rotation);

   }

   private Transform GetSpwanPoint(){
      return spwanPointList[Random.Range(0,spwanPointList.Count)];
   }
   public void Die(){
        PhotonNetwork.Instantiate(deathFx.name,player.transform.position,Quaternion.identity);
        PhotonNetwork.Destroy(player);
        SpwanPlayer();
   }
}
