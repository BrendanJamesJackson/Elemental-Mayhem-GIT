using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStarter : MonoBehaviour
{
    public PlayerInputManager playerInputManager;

    public GameObject[] prefabs;

    public GameObject[] characterProfileBoxes;

    public CinemachineTargetGroup targetGroup;

    public GameManager gameManager;


    void Start()
    {
        for (int i = 0; i < CharacterSelectionManager.instance.TotalPlayers; i++)
        {
            var playerData =
                CharacterSelectionManager.instance.Players[i];

            Debug.Log(
                $"Spawn Player {playerData.playerIndex} " +
                $"as Character {playerData.characterIndex}"
            );


            //playerInputManager.playerPrefab = prefabs[playerData.characterIndex];

            GameObject temp = PlayerInput.Instantiate(
                prefabs[playerData.characterIndex],
                playerData.playerIndex,
                null,
                -1,
                playerData.inputDevice
            ).gameObject;

            characterProfileBoxes[playerData.playerIndex].SetActive( true );

            temp.GetComponent<PlayerUICommunicator>().Sethandler(characterProfileBoxes[playerData.playerIndex].GetComponent<PlayerUIHandler>());
            temp.GetComponent<PlayerUICommunicator>().InitialSet(playerData.characterIndex);

            gameManager.SetSpawn(temp.transform, playerData.playerIndex);

            targetGroup.AddMember(temp.transform.Find("TargetCameraFollow") , 1, 5);

        }
    }

   
}
