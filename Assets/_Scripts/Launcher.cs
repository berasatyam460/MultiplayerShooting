using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using System;

public class Launcher : MonoBehaviourPunCallbacks
{
   public static Launcher instance;
   private void Awake()
   {
     instance=this;
   }
   [Header("Loading Var")]
   [SerializeField] GameObject loadingScreen;
   [SerializeField]TMP_Text loadingText;

    [SerializeField]GameObject menuButtons;
   [SerializeField]GameObject createRoomPanel;
   [SerializeField]TMP_InputField roomInputName;

   [SerializeField]GameObject roomPanel;
   [SerializeField]TMP_Text roomNameText;
   [SerializeField]GameObject errorScreen;
   [SerializeField]TMP_Text errorText;
   [SerializeField]TMP_Text  playerNameLabel;
   private List<TMP_Text>allPlayerNameLabel=new List<TMP_Text>();
    [SerializeField]Transform playerNameContainer;

   [SerializeField]GameObject roomBrowserScreen;
   [SerializeField]RoomButton theroomButton;
   [SerializeField]Transform roomBtnContainer;

   private List<RoomButton>allRoomButtons=new List<RoomButton>();
   [SerializeField]TMP_InputField nickNameInput;
   [SerializeField]GameObject nickNamePanel;
   [SerializeField]bool hasSetTheNickName;

   void CloseMenu(){
    loadingScreen.SetActive(false);
    menuButtons.SetActive(false);
    createRoomPanel.SetActive(false);
    roomPanel.SetActive(false);
    errorScreen.SetActive(false);
    roomBrowserScreen.SetActive(false);
    nickNamePanel.SetActive(false);
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
        
        PhotonNetwork.AutomaticallySyncScene=true;
        loadingText.text="Joining Lobby";
    }
    public override void OnJoinedLobby()
    {
        CloseMenu();
        menuButtons.SetActive(true);
        if(!hasSetTheNickName){
            CloseMenu();
            nickNamePanel.SetActive(true);
            if(PlayerPrefs.HasKey("PlayernickName")){
                nickNameInput.text=PlayerPrefs.GetString("PlayernickName");
            }
        }else{
            PhotonNetwork.NickName=PlayerPrefs.GetString("PlayernickName");
               
        }
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
        ListAllPlayer();
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
    public void OpenRoomBrowser(){
        CloseMenu();
        roomBrowserScreen.SetActive(true);
    }
    public void CloseRoomBrower(){
        CloseMenu();
        menuButtons.SetActive(true);

    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
       foreach(RoomButton roomButton in allRoomButtons){
        Destroy(roomButton.gameObject);
        Debug.Log("Destroying BTN");
       }
       allRoomButtons.Clear();
       theroomButton.gameObject.SetActive(false);
       for(int i=0;i<roomList.Count;i++){
            if(roomList[i].PlayerCount!=roomList[i].MaxPlayers && !roomList[i].RemovedFromList){
                RoomButton newButton=Instantiate(theroomButton,roomBtnContainer);
                newButton.SetButtonDetails(roomList[i]);
                Debug.Log(roomList[i].Name);
                newButton.gameObject.SetActive(true);
                allRoomButtons.Add(newButton);
            }
       }
    }
    public void JoinRoom(RoomInfo inputInfo){
        PhotonNetwork.JoinRoom(inputInfo.Name);
        CloseMenu();
        loadingText.text="Joining Room...";
        loadingScreen.SetActive(true);
    }
    public void QuitGame(){
        Application.Quit();
    }

    private void ListAllPlayer(){
        foreach (TMP_Text playerName in allPlayerNameLabel){
            Destroy(playerName.gameObject);
        }
        allPlayerNameLabel.Clear();
        playerNameLabel.gameObject.SetActive(false);
        Player[] players=PhotonNetwork.PlayerList;
        for(int i=0;i<players.Length;i++){
            TMP_Text newplayerLabel=Instantiate(playerNameLabel,playerNameContainer);
            newplayerLabel.text=players[i].NickName;
            newplayerLabel.gameObject.SetActive(true);
            allPlayerNameLabel.Add(newplayerLabel);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newplayerLabel=Instantiate(playerNameLabel,playerNameContainer);
            newplayerLabel.text=newPlayer.NickName;
            newplayerLabel.gameObject.SetActive(true);
            allPlayerNameLabel.Add(newplayerLabel);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayer();
    }

    public void SetNickName(){
        if(!string.IsNullOrEmpty(nickNameInput.text)){
            PhotonNetwork.NickName=nickNameInput.text;
            PlayerPrefs.SetString("PlayernickName",nickNameInput.text);
            CloseMenu();
            menuButtons.SetActive(true);
            hasSetTheNickName=true;
        }
    }
    public void StartGame(String LeveltoGo){
        PhotonNetwork.LoadLevel(LeveltoGo);
    }


}
