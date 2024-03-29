/// <Sumery>
/// This class is responsible for:
/// 1. Multiplayer connection, room and RPC management
/// 2. Processing game states connected with multiplayer activities
/// 3. Game logic for switching player sides
/// </Summery>

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    public enum Player_Identity
    {
        None,
        Player1,
        Player2
    }


    #region Variables

    public static PhotonNetworkManager Instance;
    public bool isMaster;
    [SerializeField] int RoomFailedAttempt;

    const string LogConnecting = "Connecting to server...";
    const string LogJoining = "Connected. Joining Room...";
    const string LogWaiting = "Room Joined. Waiting for the opponent...";
    const string LogJoinFailed = "Room join failed";
    const string LogCreateFailed = "Room create failed";
    const string LogGameStart = "Game Started";
    const string LogOpponentDisconnected = "Opponent disconnected";
    const string LogOpponentJoined = "Opponent joined";
    const string LogDisconnected = "Disconnected from server";

    public Player_Identity MyIdentity;
    public Player_Identity CurrentTurnPlayer;

    #endregion


    #region Initialization

    void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion


    #region Server Connectivity

    public void Call_ConnectToServer()
    {
        UIManager.Instance.UpdateMultiplayerLogs(LogConnecting);
        GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.ConnectingToServer);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        UIManager.Instance.UpdateMultiplayerLogs(LogJoining);
        GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.JoiningRoom);
        PhotonNetwork.JoinRandomOrCreateRoom();

        UIManager.Instance.gameStatus.Update_isConnected(true);
        UIManager.Instance.UpdateGameStatsInUI();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        UIManager.Instance.UpdateMultiplayerLogs(LogCreateFailed);

        if (RoomFailedAttempt > 0)
        {
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.JoiningRoom);
            PhotonNetwork.JoinRandomOrCreateRoom();
            RoomFailedAttempt--;
        }
    }

    public override void OnJoinedRoom()
    {
        isMaster = PhotonNetwork.IsMasterClient;

        if(MyIdentity == Player_Identity.None)
        {
            if (isMaster)
            {
                MyIdentity = Player_Identity.Player1;
            }
            else
            {
                MyIdentity = Player_Identity.Player2;
            }

            UIManager.Instance.gameStatus.Update_YourIdentity(MyIdentity);
        }

        UIManager.Instance.gameStatus.Update_PlayerCount((int)PhotonNetwork.CurrentRoom.PlayerCount);
        UIManager.Instance.UpdateGameStatsInUI();

        ActionsContainer.OnIdentitySet?.Invoke(MyIdentity);

        UpdateGameStatus();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        UIManager.Instance.UpdateMultiplayerLogs(LogJoinFailed);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UIManager.Instance.UpdateMultiplayerLogs(LogOpponentJoined);
        UpdateGameStatus();

        UIManager.Instance.gameStatus.Update_PlayerCount(PhotonNetwork.CurrentRoom.PlayerCount);
        UIManager.Instance.UpdateGameStatsInUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UIManager.Instance.UpdateMultiplayerLogs(LogOpponentDisconnected);
        GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.OpponentDisconnected);

        UIManager.Instance.gameStatus.Update_PlayerCount(PhotonNetwork.CurrentRoom.PlayerCount);
        UIManager.Instance.UpdateGameStatsInUI();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.Disconnected);
        UIManager.Instance.UpdateMultiplayerLogs(LogDisconnected);

        UIManager.Instance.gameStatus.Update_isConnected(false);
        UIManager.Instance.UpdateGameStatsInUI();
    }

    #endregion


    void UpdateGameStatus()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            GameStateManager.Instance.UpdateGameState(GameStateManager.Game_State.WaitingForOpponent);
            UIManager.Instance.UpdateMultiplayerLogs(LogWaiting);
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            UIManager.Instance.UpdateMultiplayerLogs(LogGameStart);

            if (isMaster && GameStateManager.Instance.GameState == GameStateManager.Game_State.WaitingForOpponent)
            {
                RPCManager.Instance.SendRPC_GameStart();
            }
        }
    }

    public void Call_SwitchSides()
    {
        if (CurrentTurnPlayer == Player_Identity.Player1)
        {
            CurrentTurnPlayer = Player_Identity.Player2;
        }
        else
        {
            CurrentTurnPlayer = Player_Identity.Player1;
        }

        RPCManager.Instance.SendRPC_SideSwitch(CurrentTurnPlayer);
    }
}