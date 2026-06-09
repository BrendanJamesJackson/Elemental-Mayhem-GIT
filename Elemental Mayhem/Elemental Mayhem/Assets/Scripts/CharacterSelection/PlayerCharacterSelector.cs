using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCharacterSelector : MonoBehaviour
{
    [Header("Selection Data")]
    public int selectedIndex = 0;

    [Header("UI References (Injected on Join)")]
    public Image[] characterIcons; // 10 icons
    public Image selectionHighlight; // this player's cursor/highlight

    [Header("Preview UI")]
    public Image previewImage;
    public TMP_Text previewName;


    private bool isMoving;
    private float moveCooldown = 0.2f;
    private float cooldownTimer;

    private void Start()
    {
        UpdateSelectionVisuals();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Stick input detected");
        OnNavigate(context.ReadValue<Vector2>());
    }


    // Called by input system (left stick / dpad)
    public void OnNavigate(Vector2 input)
    {
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

        // OPTIONAL: preview update hook (you'll wire this to ScriptableObjects later)
        if (previewName != null)
            previewName.text = $"Character {selectedIndex}";
    }

    public void AssignUI(Image[] icons, Image highlight, Image previewImg, TMP_Text previewTxt)
    {
        characterIcons = icons;
        selectionHighlight = highlight;
        previewImage = previewImg;
        previewName = previewTxt;

        UpdateSelectionVisuals();
    }
}