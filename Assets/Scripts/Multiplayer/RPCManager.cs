using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class RPCManager : MonoBehaviour
{
    public static RPCManager Instance;
    PhotonView photonView;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }



    public void SendRPC_GameStart()
    {
        if(PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_GameStart), RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    void RPC_GameStart()
    {
        ActionsContainer.OnGameStart?.Invoke();
    }



    public void SendRPC_SideSwitch(PhotonNetworkManager.Player_Identity _currnetPlayer)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_SideSwitch), RpcTarget.AllViaServer, _currnetPlayer);
        }
    }

    [PunRPC]
    void RPC_SideSwitch(PhotonNetworkManager.Player_Identity _currnetPlayer)
    {
        ActionsContainer.OnPlayerSideSwitch?.Invoke(_currnetPlayer);
    }



    public void SendRPC_SyncPlayerMovement(int _characterID, Vector3[] _pathPoints, int _cellDistance)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_SyncPlayerMovement), RpcTarget.Others, _characterID, _pathPoints, _cellDistance);
        }
    }

    [PunRPC]
    void RPC_SyncPlayerMovement(int _characterID, Vector3[] _pathPoints, int _cellDistance)
    {
        ActionsContainer.OnSyncCharacterMovement(_characterID, _pathPoints, _cellDistance);
    }



    public void SendRPC_SetDefaultsAfterCharacterMovement(int _currentCellIndex_H, int _currentCellIndex_V, int _targetCellIndex_H, int _targetCellIndex_V, int _characterIndex)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_SetDefaultsAfterCharacterMovement), RpcTarget.Others, _currentCellIndex_H, _currentCellIndex_V, _targetCellIndex_H, _targetCellIndex_V, _characterIndex);
        }
    }

    [PunRPC]
    void RPC_SetDefaultsAfterCharacterMovement(int _currentCellIndex_H, int _currentCellIndex_V, int _targetCellIndex_H, int _targetCellIndex_V, int _characterIndex)
    {
        ActionsContainer.OnSetDefaultsAfterMovement(_currentCellIndex_H, _currentCellIndex_V, _targetCellIndex_H, _targetCellIndex_V, _characterIndex);
    }
}