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



    public void SendRPC_SyncPlayerMovement(int _characterID, CustomDataStructures.CellIndex _targetCellIndex)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_SyncPlayerMovement), RpcTarget.AllViaServer, _characterID, _targetCellIndex);
        }
    }

    [PunRPC]
    void RPC_SyncPlayerMovement(int _characterID, CustomDataStructures.CellIndex _targetCellIndex)
    {
        ActionsContainer.OnSyncCharacterMovement(_characterID, _targetCellIndex);
    }



    public void SendRPC_SetDefaultsAfterCharacterMovement(CustomDataStructures.CellIndex _currentCellIndex, CustomDataStructures.CellIndex _targetCellIndex, int _characterIndex)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_SetDefaultsAfterCharacterMovement), RpcTarget.AllViaServer, _currentCellIndex, _targetCellIndex, _characterIndex);
        }
    }

    [PunRPC]
    void RPC_SetDefaultsAfterCharacterMovement(CustomDataStructures.CellIndex _currentCellIndex, CustomDataStructures.CellIndex _targetCellIndex, int _characterIndex)
    {
        ActionsContainer.OnSetDefaultsAfterMovement(_currentCellIndex, _targetCellIndex, _characterIndex);
    }
}