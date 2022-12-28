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
        GetComponent<PhotonView>();
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
}