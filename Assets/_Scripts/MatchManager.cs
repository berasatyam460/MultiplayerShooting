
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Collections;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;
    void Awake()
    {
        instance = this;
    }

    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayer,
        UpdateStat
    }


    public enum GameState
    {
        Waiting,
        Playing,
        Ending
    }
    private int killsToWin = 3;
    [SerializeField] public Transform mapCamPoint;
    public GameState gameState = GameState.Waiting;
    public float waitAfterEnding = 5f;

    [SerializeField] List<PlayerInfo> allPlayerInfos = new List<PlayerInfo>();
    private int index;
    [SerializeField] EventCodes theEvent;

    private List<LeaderBroadPlayer> lBoardPlayer = new List<LeaderBroadPlayer>();
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;
            Debug.Log("Recieved Event");
            switch (theEvent)
            {
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && gameState != GameState.Ending)
        {
            if (UI_Controler.instance.leaderBroad.activeInHierarchy)
            {
                UI_Controler.instance.leaderBroad.SetActive(false);
            }
            else
            {
                ShowLeaderBoard();
            }
        }
    }

    public void NewPlayerSend(string userName)
    {
        object[] package = new object[4];
        package[0] = userName;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;
        package[3] = 0;


        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );
    }
    public void NewPlayerReceive(object[] dataRecieved)
    {
        PlayerInfo player = new PlayerInfo((string)dataRecieved[0], (int)dataRecieved[1], (int)dataRecieved[2], (int)dataRecieved[3]);

        allPlayerInfos.Add(player);

        ListPlayerSend();
    }
    public void ListPlayerSend()
    {
        object[] package = new object[allPlayerInfos.Count + 1];
        package[0] = gameState;
        for (int i = 0; i < allPlayerInfos.Count; i++)
        {
            object[] piece = new object[4];

            piece[0] = allPlayerInfos[i].name;
            piece[1] = allPlayerInfos[i].actor;
            piece[2] = allPlayerInfos[i].kills;
            piece[3] = allPlayerInfos[i].deaths;

            package[i + 1] = piece;
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ListPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }
    public void ListPlayerRecieve(object[] dataRecieved)
    {
        allPlayerInfos.Clear();
        gameState = (GameState)dataRecieved[0];
        for (int i = 1; i < dataRecieved.Length; i++)
        {
            object[] piece = (object[])dataRecieved[i];

            PlayerInfo player = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3]
            );
            allPlayerInfos.Add(player);

            if (PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
            {
                index = i - 1;
            }
        }

        StateCheck();
    }
    public void UpdateStatSend(int actorSending, int statToUpdate, int amountToChange)
    {
        object[] package = new object[]{
            actorSending,statToUpdate,amountToChange
        };

        PhotonNetwork.RaiseEvent(
           (byte)EventCodes.UpdateStat,
           package,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }
           );
    }
    public void UpdateStatReceive(object[] dataRecieved)
    {
        int actor = (int)dataRecieved[0];
        int statType = (int)dataRecieved[1];
        int amount = (int)dataRecieved[2];

        for (int i = 0; i < allPlayerInfos.Count; i++)
        {
            if (allPlayerInfos[i].actor == actor)
            {
                switch (statType)
                {
                    case 0: //kills
                        allPlayerInfos[i].kills += amount;
                        Debug.Log("Player" + allPlayerInfos[i].name + ": kills" + allPlayerInfos[i].kills);
                        break;
                    case 1: //death
                        allPlayerInfos[i].deaths += amount;
                        Debug.Log("Player" + allPlayerInfos[i].name + ": kills" + allPlayerInfos[i].deaths);
                        break;
                }
                if (i == index)
                {
                    UpdateStatDisplay();
                }

                if (UI_Controler.instance.leaderBroad.activeInHierarchy)
                {
                    ShowLeaderBoard();
                }
                break;
            }
        }

        ScoreCheck();
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            NewPlayerSend(PhotonNetwork.NickName);
            gameState = GameState.Playing;
        }
    }

    public void UpdateStatDisplay()
    {
        if (allPlayerInfos.Count > index)
        {
            UI_Controler.instance.GetKillText().text = "Kills :" + allPlayerInfos[index].kills;
            UI_Controler.instance.GetDeathLabel().text = "Deaths :" + allPlayerInfos[index].deaths;
        }
        else
        {
            UI_Controler.instance.GetKillText().text = "Kills : 0";
            UI_Controler.instance.GetDeathLabel().text = "Deaths : 0";
        }

    }

    void ShowLeaderBoard()
    {
        UI_Controler.instance.leaderBroad.SetActive(true);

        foreach (LeaderBroadPlayer lp in lBoardPlayer)
        {
            Destroy(lp.gameObject);
        }
        lBoardPlayer.Clear();

        UI_Controler.instance.leaderbroadPlayer.gameObject.SetActive(false);
        List<PlayerInfo> sorted = SortPlayer(allPlayerInfos);
        foreach (PlayerInfo player in sorted)
        {
            LeaderBroadPlayer newPlayerDisplay = Instantiate(UI_Controler.instance.leaderbroadPlayer, UI_Controler.instance.leaderbroadPlayer.transform.parent);

            newPlayerDisplay.SetDetails(player.name, player.kills, player.deaths);

            newPlayerDisplay.gameObject.SetActive(true);

            lBoardPlayer.Add(newPlayerDisplay);
        }
    }

    private List<PlayerInfo> SortPlayer(List<PlayerInfo> players)
    {
        List<PlayerInfo> sortedPlayerList = new List<PlayerInfo>();

        while (sortedPlayerList.Count < players.Count)
        {
            int highestKill = -1;
            PlayerInfo selectedPlayer = players[0];
            foreach (PlayerInfo player in players)
            {
                if (!sortedPlayerList.Contains(player))
                {
                    if (player.kills > highestKill)
                    {
                        selectedPlayer = player;
                        highestKill = player.kills;
                    }
                }

            }

            sortedPlayerList.Add(selectedPlayer);
        }
        return sortedPlayerList;
    }


    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        SceneManager.LoadScene(0);
    }

    void ScoreCheck()
    {
        bool winnerFound = false;
        foreach (PlayerInfo player in allPlayerInfos)
        {
            if (player.kills >= killsToWin && killsToWin > 0)
            {
                winnerFound = true;
                break;
            }
        }

        if (winnerFound)
        {
            if (PhotonNetwork.IsMasterClient && gameState != GameState.Ending)
            {
                gameState = GameState.Ending;

                ListPlayerSend();
            }
        }

    }

    void StateCheck()
    {
        if (gameState == GameState.Ending)
        {
            EndGame();
        }
    }
    void EndGame()
    {
        gameState = GameState.Ending;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();

        }
        UI_Controler.instance.endScreen.SetActive(true);
        ShowLeaderBoard();


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(WaitForEnd());

    }

    IEnumerator WaitForEnd()
    {
        yield return new WaitForSeconds(waitAfterEnding);

        PhotonNetwork.AutomaticallySyncScene = false;

        PhotonNetwork.LeaveRoom();
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor, kills, deaths;
    public PlayerInfo(string _name, int _actor, int _kills, int _deaths)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
    }
    

}
