using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Fighters in Match")]
    public List<PlayerManager> fighters = new List<PlayerManager>();

    [Header("Respawn Delay")]
    public float koPauseTime = 0.5f;

    public Transform[] spawnPoints; 


    private void Awake()
    {
        instance = this;
    }

    public void KnockOut(PlayerManager player)
    {
        if (player == null || player.isEliminated)
        {
            return;
        }

        player.LoseHeart();

        Debug.Log(player.name + " was KO'd!");

        if (player.lastAttacker != null)
        {
            PlayerManager attacker = player.lastAttacker.GetComponent<PlayerManager>();

            if (attacker != null)
            {
                Debug.Log(attacker.name + " got the KO!");
            }
        }

        CheckWinner();


    }




    void CheckWinner()
    {
        PlayerManager lastAlive = null;

        int aliveCount = 0;

        foreach(var fighter in fighters)
        {
            if (fighter != null && !fighter.isEliminated)
            {
                aliveCount++;
                lastAlive = fighter;
            }
        }

        if (aliveCount == 1 && lastAlive != null)
        {
            Debug.Log("Winner: " + lastAlive.name);
            EndMatch(lastAlive);
        }

    }

    private void EndMatch(PlayerManager winner)
    {
        Debug.Log(winner.name + " wins the match!");

        Time.timeScale = 0f;
    }

    public void RegisterFighter(PlayerManager player)
    {
        if (!fighters.Contains(player))
        {
            fighters.Add(player);
        }
    }

}
