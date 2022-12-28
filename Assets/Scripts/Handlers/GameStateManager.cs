/// <Sumery>
/// This class is responsible for:
/// 1. Containing and updating game state
/// </Summery>

using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum Game_State
    {
        None,
        GeneratingGrid,
        ConnectingToServer,
        JoiningRoom,
        WaitingForOpponent,
        OpponentDisconnected,
        Idle,
        CharacterSelected,
        MovingCharacter,
        OtherPlayerTurn,
        Disconnected
    }

    public Game_State GameState { get; private set; }

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

    public void UpdateGameState(Game_State _newState)
    {
        GameState = _newState;
        UIManager.Instance.gameStatus.Update_GameState(_newState);
        UIManager.Instance.UpdateGameStatsInUI();
    }
}