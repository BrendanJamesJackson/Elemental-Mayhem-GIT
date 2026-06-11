using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCharacterSelector : MonoBehaviour
{
    [Header("Selection Data")]
    public int selectedIndex = 0;
    public int playerIndex;

    [Header("UI References (Injected on Join)")]
    public Image[] characterIcons; // 10 icons
    public Image selectionHighlight; // this player's cursor/highlight

    [Header("Preview UI")]
    public Image previewImage;
    public TMP_Text previewName;
    public GameObject previewCharacter;
    public GameObject ConfirmBtn;
    public GameObject CancelBtn;

    public string[] charNames;
    public Sprite[] charSprites; 


    private bool isMoving;
    private float moveCooldown = 0.2f;
    private float cooldownTimer;

    public PlayerInput playerInput;


    private bool lockedIn = false;


    private void Start()
    {
        UpdateSelectionVisuals();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Stick input detected");
        OnNavigate(context.ReadValue<Vector2>());
    }


    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lockedIn = true;
            CharacterSelectionManager.instance.AddPlayer(playerIndex, selectedIndex, playerInput.devices[0]);
            Debug.Log(playerInput.devices[0]);
            ConfirmBtn.SetActive(false);
            CancelBtn.SetActive(true);
        }
    }

    public void Cancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lockedIn = false;
            CharacterSelectionManager.instance.ChangeSelection(playerIndex);
            ConfirmBtn.SetActive(true);
            CancelBtn.SetActive(false);
        }
    }

    public void BeginGame(InputAction.CallbackContext context)
    {
        CharacterSelectionManager.instance.StartGame();

    }


    // Called by input system (left stick / dpad)
    public void OnNavigate(Vector2 input)
    {

        if (lockedIn)
            return;

        if (isMoving)
            return;

        if (input.magnitude < 0.5f)
            return;

        int previousIndex = selectedIndex;

        int row = selectedIndex / 2;
        int col = selectedIndex % 2;

        if (input.y > 0.5f)
            row--;
        else if (input.y < -0.5f)
            row++;

        if (input.x > 0.5f)
            col++;
        else if (input.x < -0.5f)
            col--;

        row = Mathf.Clamp(row, 0, 4);
        col = Mathf.Clamp(col, 0, 1);

        selectedIndex = row * 2 + col;

        if (selectedIndex != previousIndex)
        {
            UpdateSelectionVisuals();
            StartCoroutine(MoveCoolDownRoutine());
        }
    }

    private IEnumerator MoveCoolDownRoutine()
    {
        isMoving = true;
        yield return new WaitForSeconds(0.15f);
        isMoving = false;
    }

    private void UpdateSelectionVisuals()
    {
        // Move THIS player's highlight to correct icon
        if (characterIcons != null && selectedIndex < characterIcons.Length)
        {
            selectionHighlight.transform.position =
                characterIcons[selectedIndex].transform.position;
        }

        previewImage.sprite = charSprites[selectedIndex];
        previewName.text = charNames[selectedIndex];
        previewCharacter.GetComponent<Animator>().SetFloat("CharIndex", (selectedIndex));
    }

    public void SetIndex(int index)
    {
        playerIndex = index;
    }

    public void AssignUI(Image[] icons, Image highlight, Image previewImg, TMP_Text previewTxt, GameObject previewChar, GameObject confirm, GameObject cancel)
    {
        characterIcons = icons;
        selectionHighlight = highlight;
        selectionHighlight.gameObject.SetActive(true);
        previewImage = previewImg;
        previewName = previewTxt;
        previewCharacter = previewChar;
        ConfirmBtn = confirm;
        CancelBtn = cancel;

        previewImage.gameObject.SetActive(true);

        UpdateSelectionVisuals();
    }
}