
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class MatchManager : MonoBehaviourPunCallbacks,IOnEventCallback
{   
    public static MatchManager instance;
     void Awake()
    {
        instance=this;
    }

    public enum EventCodes:byte{
        NewPlayer,
        ListPlayer,
        UpdateStat
    }

    [SerializeField]List<PlayerInfo>allPlayerInfos=new List<PlayerInfo>();
    private int index;
    [SerializeField]EventCodes theEvent;
    

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code<200){
            EventCodes theEvent=(EventCodes)photonEvent.Code;
            object[] data =(object[])photonEvent.CustomData;
            Debug.Log("Recieved Event");
            switch(theEvent){
                case EventCodes.NewPlayer:
                    NewPlayerReceive(data);
                    break;
                case EventCodes.ListPlayer:
                    ListPlayerRecieve(data);
                    break;
                case EventCodes.UpdateStat:
                    UpdateStatReceive(data);
                    break;
            }
        }
    }
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void NewPlayerSend(string userName){
        object [] package=new object[4];
        package[0]=userName;
        package[1]=PhotonNetwork.LocalPlayer.ActorNumber;
        package[2]=0;
        package[3]=0;


        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions{Receivers =ReceiverGroup.MasterClient},
            new SendOptions{Reliability=true}
            );
    }
    public void NewPlayerReceive(object[] dataRecieved){
        PlayerInfo player=new PlayerInfo((string)dataRecieved[0],(int)dataRecieved[1],(int)dataRecieved[2],(int)dataRecieved[3]);

        allPlayerInfos.Add(player);

        ListPlayerSend();
    }
    public void ListPlayerSend(){
        object []package= new object [allPlayerInfos.Count];

       for (int i=0;i<allPlayerInfos.Count;i++){

            object [] piece =new object[4];

            piece[0]=allPlayerInfos[0].name;
            piece[1]=allPlayerInfos[1].actor;
            piece[2]=allPlayerInfos[2].kills;
            piece[3]=allPlayerInfos[3].deaths;

            package[i]=piece;

        }
         PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ListPlayer,
            package,
            new RaiseEventOptions{Receivers =ReceiverGroup.All},
            new SendOptions{Reliability=true}
            );

       
    }
    public void ListPlayerRecieve(object[] dataRecieved){
        allPlayerInfos.Clear();

        for(int i=0;i<dataRecieved.Length;i++){
            object[] piece=(object [])dataRecieved[i];
            
            PlayerInfo player=new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3]
            );
            allPlayerInfos.Add(player);

            if(PhotonNetwork.LocalPlayer.ActorNumber==player.actor){
                index=i;
            }
        }
    }
    public void UpdateStatSend(){

    }
    public void UpdateStatReceive(object[] dataRecieved){
        
    }

    void Start()
    {
        if(!PhotonNetwork.IsConnected){
            SceneManager.LoadScene(0);
        }else{
            NewPlayerSend(PhotonNetwork.NickName);
        }
    }
}

[System.Serializable]
public class PlayerInfo{
    public string name;
    public int actor,kills,deaths;
    public PlayerInfo(string _name,int _actor,int _kills,int _deaths){
        name=_name;
        actor=_actor;
        kills=_kills;
        deaths=_deaths;
    }
}
