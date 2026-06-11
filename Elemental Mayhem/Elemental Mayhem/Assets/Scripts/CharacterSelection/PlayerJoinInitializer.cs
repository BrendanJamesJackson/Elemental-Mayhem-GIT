using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

public class PlayerJoinInitializer : MonoBehaviour
{
    [Header("Scene References")]
    public Image[] characterIcons;
    public Image[] playerHighlights; // one per player color
    public Image [] previewImage;
    public GameObject[] previewCharacter; 
    public TMP_Text [] previewText;
    public GameObject[] joinPrompt;

    public GameObject[] ConfirmBtn;
    public GameObject[] CancelBtn;


    private int playerCount = 0;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        var selector = playerInput.GetComponent<PlayerCharacterSelector>();

        // Assign unique highlight per player
        Image highlight = playerHighlights[playerCount];

        selector.AssignUI(
            characterIcons,
            highlight,
            previewImage[playerCount],
            previewText[playerCount],
            previewCharacter[playerCount],
            ConfirmBtn[playerCount],
            CancelBtn[playerCount]
        );
        selector.SetIndex(playerCount);

        joinPrompt[playerCount].SetActive(false);

        selector.selectedIndex = 0;
        selector.OnNavigate(Vector2.zero);


        playerCount++;
    }
}