using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
   public static Launcher instance;
   private void Awake()
   {
     instance=this;
   }
   [SerializeField] GameObject loadingScreen;
   [SerializeField]GameObject menuButtons;
   [SerializeField]TMP_Text loadingText;
   [SerializeField]GameObject createRoomPanel;
   [SerializeField]TMP_InputField roomInputName;

   [SerializeField]GameObject roomPanel;
   [SerializeField]TMP_Text roomNameText;
   [SerializeField]GameObject errorScreen;
   [SerializeField]TMP_Text errorText;


   void CloseMenu(){
    loadingScreen.SetActive(false);
    menuButtons.SetActive(false);
    createRoomPanel.SetActive(false);
    roomPanel.SetActive(false);
    errorScreen.SetActive(false);
   }
    void Start()
    {
        CloseMenu();
        loadingScreen.SetActive(true);
        loadingText.text="Connecting To Network...";
        PhotonNetwork.ConnectUsingSettings();

    }
    public override void OnConnectedToMaster()
    {
       
        PhotonNetwork.JoinLobby();
        loadingText.text="Joining Lobby";
    }
    public override void OnJoinedLobby()
    {
        CloseMenu();
        menuButtons.SetActive(true);
    }
    //
    public void OpenRoomCreate(){
        CloseMenu();
        createRoomPanel.SetActive(true);
    }
    //creating the room
    public void CreateRoom(){
        if(!string.IsNullOrEmpty(roomInputName.text)){
            RoomOptions options=new RoomOptions();
            options.MaxPlayers=5;
            
            PhotonNetwork.CreateRoom(roomInputName.text,options);
            CloseMenu();
            loadingText.text="Creating Room...";
            loadingScreen.SetActive(true);
        }
    }
    //after creation then join
    public override void OnJoinedRoom()
    {
        CloseMenu();
        roomPanel.SetActive(true);
        roomNameText.text=PhotonNetwork.CurrentRoom.Name;
    }
    //if the room creation is failed
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text="Failed To Create Room:"+ message;
        CloseMenu();
        errorScreen.SetActive(true);

    }
    public void CloseErrorScreen(){
        CloseMenu();
        menuButtons.SetActive(true);
    }
    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom();
        CloseMenu();
        loadingText.text="Leaving Room...";
        loadingScreen.SetActive(true);

    }
    public override void OnLeftRoom()
    {
       CloseMenu();
       menuButtons.SetActive(true);
    }


}
