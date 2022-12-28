using System;
using System.Text;

public class CustomDataStructures
{
    [Serializable]
    public struct CellIndex
    {
        public int Horizontal;
        public int Vertical;
    }

    [Serializable]
    public struct GameStatus
    {
        public string isConnected;
        public string PlayerCount;
        public string YourIdentity;
        public string CurrentPlayer;
        public string SelectedCharacter;
        public string GameState;

        StringBuilder stringBuilder;
        const string NewLine = "\n";

        const string name_isConnected = "Is Connected : ";
        const string name_PlayerCount = "# of Players : ";
        const string name_YourIdentity = "You are : ";
        const string name_CurrentPlayer = "Current Turn : ";
        const string name_SelectedCharacter = "Selected Character : ";
        const string name_GameState = "State : ";

        public void Initialize()
        {
            stringBuilder = new StringBuilder();

            isConnected = "False";
            PlayerCount = "None";
            YourIdentity = "None";
            CurrentPlayer = "None";
            SelectedCharacter = "None";
            GameState = "None";
        }
        public void Update_isConnected(bool _value)
        {
            isConnected = _value.ToString();
        }

        public void Update_PlayerCount(int _value)
        {
            PlayerCount = _value.ToString();
        }

        public void Update_YourIdentity(PhotonNetworkManager.Player_Identity _value)
        {
            YourIdentity = _value.ToString();
        }

        public void Update_CurrentPlayer(PhotonNetworkManager.Player_Identity _value)
        {
            CurrentPlayer = _value.ToString();
        }

        public void Update_SelectedCharacter(CharacterProperties.Character_Type _value)
        {
            SelectedCharacter = _value.ToString();
        }

        public void Update_GameState(GameStateManager.Game_State _value)
        {
            GameState = _value.ToString();
        }

        public string ShowCombinedGameStatus(string _title)
        {
            if(stringBuilder.Length != 0)
            {
                stringBuilder.Clear();
            }

            return stringBuilder
                .Append(_title)
                .Append(name_isConnected).Append(isConnected).Append(NewLine)
                .Append(name_PlayerCount).Append(PlayerCount).Append(NewLine)
                .Append(name_YourIdentity).Append(YourIdentity).Append(NewLine)
                .Append(name_CurrentPlayer).Append(CurrentPlayer).Append(NewLine)
                .Append(name_SelectedCharacter).Append(SelectedCharacter).Append(NewLine)
                .Append(name_GameState).Append(GameState).Append(NewLine)
                .ToString();
        }
    }
}