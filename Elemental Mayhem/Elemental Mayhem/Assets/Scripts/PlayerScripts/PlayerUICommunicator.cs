using UnityEngine;

public class PlayerUICommunicator : MonoBehaviour
{
    public PlayerUIHandler uIHandler;
    public int charIndex;

    public void Sethandler(PlayerUIHandler handler)
    {
        uIHandler = handler;
    }


    public void UpdateDetails(int hearts, float dura, float elem)
    {
        uIHandler.UpdateHearts(hearts);
        uIHandler.UpdateDurability(dura);
        uIHandler.UpdateElemental(elem);
    }

    public void InitialSet(int index)
    {
        charIndex = index;
        uIHandler.SetProfileImage(charIndex);
    }

}
