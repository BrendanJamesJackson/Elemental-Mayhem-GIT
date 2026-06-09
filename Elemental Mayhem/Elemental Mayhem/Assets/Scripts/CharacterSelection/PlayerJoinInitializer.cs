using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerJoinInitializer : MonoBehaviour
{
    [Header("Scene References")]
    public Image[] characterIcons;
    public Image[] playerHighlights; // one per player color
    public Image previewImage;
    public TMP_Text previewText;

    private int playerCount = 0;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        var selector = playerInput.GetComponent<PlayerCharacterSelector>();

        // Assign unique highlight per player
        Image highlight = playerHighlights[playerCount];

        selector.AssignUI(
            characterIcons,
            highlight,
            previewImage,
            previewText
        );

        selector.selectedIndex = 0;
        selector.OnNavigate(Vector2.zero);

        playerCount++;
    }
}