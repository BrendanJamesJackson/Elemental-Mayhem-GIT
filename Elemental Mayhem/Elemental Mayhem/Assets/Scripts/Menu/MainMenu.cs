using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int CharSelectionIndex;
    public int ControlsIndex;
    public int HtPIndex;
    public int creditsIndex;

    public void Play()
    {
        SceneManager.LoadScene(CharSelectionIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void HtP()
    {
        SceneManager.LoadScene(ControlsIndex);
    }

    public void Instructions()
    {
        SceneManager.LoadScene(HtPIndex);
    }

    public void Credits()
    {
        SceneManager.LoadScene(creditsIndex);
    }

}
