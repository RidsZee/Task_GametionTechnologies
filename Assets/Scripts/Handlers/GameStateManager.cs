using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum Game_State
    {
        None,
        GeneratingGrid,
        ConnectingToServer,
        JoiningRoom,
        WaitingForOtherPlayer,
        Idle,
        CharacterSelected,
        MovingCharacter,
        OtherPlayerTurn,
        Disconnected
    }

    public Game_State GameState;

    public static GameStateManager Instance;

    void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}