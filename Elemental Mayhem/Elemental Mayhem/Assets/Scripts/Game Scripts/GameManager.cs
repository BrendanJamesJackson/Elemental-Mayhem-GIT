using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<FighterState> fighters = new List<FighterState>();


    private void Awake()
    {
        instance = this;
    }

    public void KnockOut(FighterState fighter)
    {
        fighter.stocks--;

        Debug.Log(fighter.name + " was KO'd!");

        if (fighter.stocks <= 0)
        {
            Eliminate(fighter);
        }
        else
        {
            Respawn(fighter);
        }

        CheckWinner();


    }

    void Respawn(FighterState fighter)
    {

    }

    void Eliminate(FighterState fighter)
    {
        fighter.gameObject.SetActive(false);
    }


    void CheckWinner()
    {
        int alive = 0;
        FighterState winner = null;

        foreach (var fighter in fighters)
        {
            if (fighter.stocks > 0)
            {
                alive++;
                winner = fighter;
            }
        }

        if (alive == 1)
        {
            Debug.Log(winner.name + " Wins!");
        }
    }


}
