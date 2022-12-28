using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public CustomDataStructures.GameStatus gameStatus;
    
    [SerializeField]
    int MaxMultiplayerLogs;

    [Header("UI References")]

    [SerializeField]
    TextMeshProUGUI T_GameStatus;

    [SerializeField]
    TextMeshProUGUI T_MultiplayerLogs;

    [SerializeField]
    TextMeshProUGUI T_Warning;

    [SerializeField]
    public float WarningDuration;
    float WarningTimer;
    Coroutine WarningCoroutine;
    bool isWarningActive;

    int CurrentLogs;
    StringBuilder stringBuilder;
    const string NewLine = "\n";
    const string MultiplayerLogsTitle = "Multiplayer logs: \n\n";
    const string GameStatsTitle = "Game stats: \n\n";

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

    void Start()
    {
        T_Warning.text = MultiplayerLogsTitle;
        stringBuilder = new StringBuilder();
        gameStatus.Initialize();
    }

    public void UpdateGameStatsInUI()
    {
        T_GameStatus.text = gameStatus.ShowCombinedGameStatus(GameStatsTitle);
    }

    public void UpdateMultiplayerLogs(string _newLog)
    {
        stringBuilder.Append(_newLog).Append(NewLine);
        CurrentLogs++;

        if(CurrentLogs > MaxMultiplayerLogs)
        {
            CurrentLogs = 0;
            stringBuilder.Clear();
        }
    }

    public void ShowWarning(string _warning)
    {
        if(isWarningActive)
        {
            StopCoroutine(WarningCoroutine);
        }

        T_Warning.text = _warning;
        WarningCoroutine = StartCoroutine(Coroutine_WarningTimer());
    }


    IEnumerator Coroutine_WarningTimer()
    {
        isWarningActive = true;
        WarningTimer = WarningDuration;

        while(WarningTimer > 0)
        {
            WarningTimer -= Time.deltaTime;
            yield return null;
        }

        T_Warning.text = MultiplayerLogsTitle;
        isWarningActive = false;
    }
}
