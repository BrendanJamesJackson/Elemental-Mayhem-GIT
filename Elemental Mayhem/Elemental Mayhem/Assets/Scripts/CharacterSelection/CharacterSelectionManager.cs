using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager instance;

    [System.Serializable]
    public class PlayerSelectionData
    {
        public int playerIndex;
        public int characterIndex;
        public InputDevice inputDevice;
        public bool lockedIn = false;
    }

    public int gameSceneIndex;

    public List<PlayerSelectionData> Players = new();

    public int TotalPlayers => Players.Count;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ClearSelections()
    {
        Players.Clear();
    }

    public void ChangeSelection(int index)
    {
        PlayerSelectionData existingPlayer =
            Players.Find(p => p.playerIndex == index);

        if (existingPlayer != null)
        {
            existingPlayer.lockedIn = false;
        }
    }

    public void AddPlayer(int playerindex, int charindex, InputDevice inputdevice)
    {
        PlayerSelectionData existingPlayer =
            Players.Find(p => p.playerIndex == playerindex);

        if (existingPlayer != null)
        {
            existingPlayer.characterIndex = charindex;
            existingPlayer.inputDevice = inputdevice;
            existingPlayer.lockedIn = true;
        }
        else
        {
            Players.Add(new PlayerSelectionData
            {
                playerIndex = playerindex,
                characterIndex = charindex,
                inputDevice = inputdevice,
                lockedIn = true
            });
        }
    }

    public void StartGame()
    {
        bool temp = true;

        foreach (var player in Players)
        {
            temp = player.lockedIn;
        }

        if (temp)
        {
            SceneManager.LoadScene(gameSceneIndex);
        }


    }

    public PlayerSelectionData GetPlayer(int playerindex)
    {
        return Players.Find(p=>p.playerIndex == playerindex);
    }

}
