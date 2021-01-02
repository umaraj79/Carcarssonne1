using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnScript : MonoBehaviour
{
    int nbrOfplayers;
    int turns;
    int iterator = 0;
    void Start()
    {
        nbrOfplayers = PlayerPrefs.GetInt("PlayerCount");
        turns = nbrOfplayers;
    }

    public int currentPlayer()
    {
        return iterator;
    }


    public int newTurn()
    {
        if (iterator+1 == nbrOfplayers)
        {
            return iterator = 0;
        }
        else
        {
            iterator += 1;
            return iterator;
        }
        
    }
 
}
