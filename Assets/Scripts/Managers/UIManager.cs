/// <Sumery>
/// This class is responsible for:
/// 1. Showing all UI on screen
/// 2. This class does use singleton, it does not receive any Actions nor any RPCs
/// </Summery>

using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;

public class UIManager : MonoBehaviour
{
    #region Variables

    public static UIManager Instance;
    public CustomDataStructures.GameStatus gameStatus;
    
    [SerializeField] TextMeshProUGUI T_GameStatus;
    [SerializeField] TextMeshProUGUI T_MultiplayerLogs;
    [SerializeField] TextMeshProUGUI T_Warning;

    [SerializeField] int MaxMultiplayerLogs;
    [SerializeField] public float WarningDuration;

    float WarningTimer;
    bool isWarningActive;
    int CurrentLogs;
    
    StringBuilder stringBuilderLogs;
    StringBuilder stringBuilderWarning;
    
    const string NewLine = "\n";
    const string LogWarning = "Warning : \n";
    const string MultiplayerLogsTitle = "Multiplayer logs: \n\n";
    const string GameStatsTitle = "Game stats: \n\n";
    const string Separator = " | ";

    Coroutine WarningCoroutine;

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
            DontDestroyOnLoad(gameObject);
        }

        gameStatus.Initialize();
        stringBuilderWarning = new StringBuilder();
        stringBuilderLogs = new StringBuilder();
    }

    void Start()
    {
        UpdateGameStatsInUI();
        stringBuilderLogs.Append(MultiplayerLogsTitle);
    }

    #endregion


    #region Update UI On Screen

    public void UpdateGameStatsInUI()
    {
        T_GameStatus.text = gameStatus.ShowCombinedGameStatus(GameStatsTitle);
    }

    public void UpdateMultiplayerLogs(string _newLog)
    {
        CurrentLogs++;

        stringBuilderLogs.Append(CurrentLogs.ToString()).Append(Separator).Append(_newLog).Append(NewLine);
        T_MultiplayerLogs.text = stringBuilderLogs.ToString();
        
        if(CurrentLogs > MaxMultiplayerLogs)
        {
            CurrentLogs = 0;
            stringBuilderLogs.Clear();
            stringBuilderLogs.Append(MultiplayerLogsTitle);
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

    #endregion
}