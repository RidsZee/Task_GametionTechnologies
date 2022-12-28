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
    StringBuilder stringBuilderLogs;
    StringBuilder stringBuilderWarning;
    const string NewLine = "\n";
    const string LogWarning = "Warning : \n";
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

        gameStatus.Initialize();
        stringBuilderWarning = new StringBuilder();
        stringBuilderLogs = new StringBuilder();
    }

    void Start()
    {
        UpdateGameStatsInUI(); ;
    }

    public void UpdateGameStatsInUI()
    {
        T_GameStatus.text = gameStatus.ShowCombinedGameStatus(GameStatsTitle);
    }

    public void UpdateMultiplayerLogs(string _newLog)
    {
        stringBuilderLogs.Append(_newLog).Append(NewLine);
        T_MultiplayerLogs.text = stringBuilderLogs.ToString();
        
        CurrentLogs++;

        if(CurrentLogs > MaxMultiplayerLogs)
        {
            CurrentLogs = 0;
            stringBuilderLogs.Clear();
        }
    }

    public void ShowWarning(string _warning)
    {
        if(isWarningActive)
        {
            StopCoroutine(WarningCoroutine);
        }

        stringBuilderWarning.Clear();
        T_Warning.text = stringBuilderWarning.Append(LogWarning).Append(_warning).ToString();
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

        T_Warning.text = LogWarning;
        isWarningActive = false;
    }
}