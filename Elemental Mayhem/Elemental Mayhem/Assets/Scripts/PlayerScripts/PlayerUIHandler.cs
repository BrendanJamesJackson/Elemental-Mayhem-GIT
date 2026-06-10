using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    public Image profileImage;
    public Image HeartsScale;
    public Image durabilityScale;
    public Image elementalScale;


    public Sprite[] charImages;
    
    public Sprite hearts4;
    public Sprite hearts3;
    public Sprite hearts2;
    public Sprite hearts1;
    public Sprite hearts0;

    public void UpdateHearts(int hearts)
    {
        switch (hearts)
        {
            case 0:
                HeartsScale.sprite = hearts0;
                break;
            case 1:
                HeartsScale.sprite = hearts1;
                break;
            case 2:
                HeartsScale.sprite = hearts2;
                break;
            case 3:
                HeartsScale.sprite = hearts3;
                break;
            case 4:
                HeartsScale.sprite = hearts4;
                break;

        }
    }

    public void UpdateDurability(float ratio)
    {
        durabilityScale.fillAmount = ratio;
    }

    public void UpdateElemental(float ratio)
    {
        elementalScale.fillAmount = ratio;
    }

    public void SetProfileImage(int index)
    {
        profileImage.sprite = charImages[index];
    }



}
