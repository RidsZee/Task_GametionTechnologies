using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum Game_State
    {
        None,
        ConnectingToServer,
        JoiningRoom,
        WaitingForOtherPlayer,
        GeneratingGrid,
        Idle,
        CharacterSelected,
        MovingCharacter
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