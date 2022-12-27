using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    public static PhotonNetworkManager Instance;
    [SerializeField]
    int RoomFailedAttempt;
    public bool isMaster;

    public enum Player_Identity
    {
        None,
        Player1,
        Player2
    }

    public Player_Identity MyIdentity;
    public Player_Identity CurrentTurnPlayer;

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

    public void Call_ConnectToServer()
    {
        GameStateManager.Instance.GameState = GameStateManager.Game_State.ConnectingToServer;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnected()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        GameStateManager.Instance.GameState = GameStateManager.Game_State.JoiningRoom;
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnCreatedRoom()
    {
        
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if(RoomFailedAttempt > 0)
        {
            GameStateManager.Instance.GameState = GameStateManager.Game_State.JoiningRoom;
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
        }

        GameStateManager.Instance.GameState = GameStateManager.Game_State.Idle;

        ActionsContainer.OnIdentitySet?.Invoke(Player_Identity.Player1);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
    }

    public override void OnLeftRoom()
    {
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        GameStateManager.Instance.GameState = GameStateManager.Game_State.Disconnected;
    }
}