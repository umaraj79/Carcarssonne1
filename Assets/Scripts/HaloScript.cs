using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloScript : MonoBehaviour
{
    int xPos, yPos, x1, y1, x2, y2;


    public enum State
    {
        VALID,
        INVALID,
        PENDING
    }

    public State currentState = State.PENDING;

    public void SetPosition(int x, int y)
    {
        this.xPos = x;
        this.yPos = y;
        x1 = x - 1;
        x2 = x + 1;
        y1 = y - 1;
        y2 = y + 1;
    }

    public void SetState(State state)
    {
        currentState = state;
    }

    public int GetXPosition()
    {
        return xPos;
    }

    public int GetYPosition()
    {
        return yPos;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.INVALID: break;
            case State.PENDING: break;
            case State.VALID: break;
        }
    }
}
