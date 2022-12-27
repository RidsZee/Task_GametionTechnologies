using System;

public class ActionsContainer
{
    public static Action OnGridGenerationCompleted;
    public static Action OnAttack;
    public static Action OnDefend;
    public static Action<CellData> OnWalk;
    public static Action<CellData> OnCellClicked;
    public static Action<CellData> OnBeginWalking;
    public static Action<CustomDataStructures.CellIndex> OnOccupyCell;
    public static Action<Character> OnCharacterSelected;
    public static Action<Character> OnCharacterDeSelected;
    public static Action OnAllCharactersDeSelected;
    public static Action OnCharacterReachedTarget;

    // Multiplayer

    public static Action OnConnectedToNetwork;
    public static Action<PhotonNetworkManager.Player_Identity> OnIdentitySet;
    public static Action OnGameStart;

    // RPCs
}