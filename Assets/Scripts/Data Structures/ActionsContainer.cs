using System;
using UnityEngine;

public class ActionsContainer
{
    #region Local Actions

    public static Action OnGridGenerationCompleted;
    public static Action OnAttack;
    public static Action OnDefend;
    public static Action<CellData> OnWalk;
    public static Action<CellData> OnCellClicked;
    public static Action<CellData> OnBeginWalking;
    public static Action<CustomDataStructures.CellIndex> OnOccupyCell;
    public static Action<int> OnCharacterSelected;
    public static Action<int> OnCharacterDeSelected;

    #endregion


    #region Multiplayer Actions

    public static Action OnCharacterReachedTarget;
    public static Action<PhotonNetworkManager.Player_Identity> OnIdentitySet;
    public static Action OnGameStart;
    public static Action<PhotonNetworkManager.Player_Identity> OnPlayerSideSwitch;
    public static Action<int, Vector3[], int, int, int, int, int> OnSyncCharacterMovement;

    #endregion
}