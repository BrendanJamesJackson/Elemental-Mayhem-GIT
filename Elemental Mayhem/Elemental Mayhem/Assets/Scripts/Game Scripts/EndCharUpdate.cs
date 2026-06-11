using UnityEngine;

public class EndCharUpdate : MonoBehaviour
{

    public Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.SetFloat("CharIndex", (PlayerPrefs.GetInt("winner")));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
