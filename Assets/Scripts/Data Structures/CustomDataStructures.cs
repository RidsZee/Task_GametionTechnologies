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

        StringBuilder stringBuilder;
        const string NewLine = "\n";

        const string name_isConnected = "Is Connected : ";
        const string name_PlayerCount = "# of Players : ";
        const string name_YourIdentity = "You are : ";
        const string name_CurrentPlayer = "Current Turn : ";
        const string name_SelectedCharacter = "Selected Character : ";

        public void Initialize()
        {
            stringBuilder = new System.Text.StringBuilder();

            isConnected = "False";
            PlayerCount = "None";
            YourIdentity = "None";
            CurrentPlayer = "None";
            SelectedCharacter = "None";
        }
        public void Update_isConnected(bool _value)
        {
            isConnected = _value.ToString();
        }

        public void Update_PlayerCount(int _value)
        {
            isConnected = _value.ToString();
        }

        public void Update_YourIdentity(PhotonNetworkManager.Player_Identity _value)
        {
            isConnected = _value.ToString();
        }

        public void Update_CurrentPlayer(PhotonNetworkManager.Player_Identity _value)
        {
            isConnected = _value.ToString();
        }

        public void Update_SelectedCharacter(CharacterProperties.Character_Type _value)
        {
            isConnected = _value.ToString();
        }

        public string ShowCombinedGameStatus(string _title)
        {
            return stringBuilder
                .Append(_title)
                .Append(name_isConnected).Append(isConnected).Append(NewLine)
                .Append(name_PlayerCount).Append(PlayerCount).Append(NewLine)
                .Append(name_YourIdentity).Append(YourIdentity).Append(NewLine)
                .Append(name_CurrentPlayer).Append(CurrentPlayer).Append(NewLine)
                .Append(name_SelectedCharacter).Append(SelectedCharacter).Append(NewLine)
                .ToString();
        }
    }
}