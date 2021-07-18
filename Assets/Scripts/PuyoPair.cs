using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuyoPair
{
    PuyoObject[] puyos = new PuyoObject[2];



    public Vector2 Left
    {
        get
        {
            if (puyos[0] == null)
                return puyos[1].transform.position;
            else if (puyos[1] == null)
                return puyos[0].transform.position;


            if (puyos[0].transform.position.x < puyos[1].transform.position.x)
                return puyos[0].transform.position;
            else if (puyos[0].transform.position.x > puyos[1].transform.position.x)
            {
                return puyos[1].transform.position;
            }
            else
            {
                return puyos[0].transform.position;
            }
        }
    }


    public Vector2 Right
    {
        get
        {
            if (puyos[0] == null)
                return puyos[1].transform.position;
            else if (puyos[1] == null)
                return puyos[0].transform.position;


            if (puyos[0].transform.position.x < puyos[1].transform.position.x)
                return puyos[1].transform.position;
            else if (puyos[0].transform.position.x > puyos[1].transform.position.x)
            {
                return puyos[0].transform.position;
            }
            else
            {
                return puyos[0].transform.position;
            }
        }
    }

    public Vector2 Up
    {
        get
        {
            if (puyos[0] == null)
                return puyos[1].transform.position;
            else if (puyos[1] == null)
                return puyos[0].transform.position;


            if (Mathf.Abs(puyos[0].transform.position.y - puyos[1].transform.position.y) <= 0.2f)
                return puyos[0].transform.position;

            if (puyos[0].transform.position.y > puyos[1].transform.position.y)
                return puyos[1].transform.position;
            else if (puyos[0].transform.position.y < puyos[1].transform.position.y)
            {
                return puyos[0].transform.position;
            }
            else
            {
                return puyos[0].transform.position;
            }
        }
    }


    public Vector2 Down
    {
        get
        {
            if (puyos[0] == null)
                return puyos[1].transform.position;
            else if (puyos[1] == null)
                return puyos[0].transform.position;

            if (Mathf.Abs(puyos[0].transform.position.y - puyos[1].transform.position.y) <= 0.2f)
                return puyos[0].transform.position;

            if (puyos[0].transform.position.y < puyos[1].transform.position.y)
                return puyos[1].transform.position;
            else if (puyos[0].transform.position.y > puyos[1].transform.position.y)
            {
                return puyos[0].transform.position;
            }
            else
            {
                return puyos[0].transform.position;
            }
        }
    }




    public PuyoObject Puyo1
    {
        get
        {
            return puyos[0];
        }
    }


    public PuyoObject Puyo2
    {
        get
        {
            return puyos[1];
        }
    }

    public void Init(PuyoObject puyo1, PuyoObject puyo2)
    {
        puyos[0] = puyo1;
        puyos[1] = puyo2;
    }

    public void Move(float direction)
    {
        Vector2 position = Vector2.zero;
        position.x += direction;
        //position.y = puyos[0].transform.position.y;

        if (puyos[0] != null)
            puyos[0].transform.Translate(position);

        //position.y = puyos[1].transform.position.y;
        if (puyos[1] != null)
            puyos[1].transform.Translate(position);
    }

    public bool IsBelow(PuyoObject puyo, out PuyoObject other)
    {
        other = null;

        //One of them is empty
        if (puyos[0] == null)
            return false;
        else if (puyos[1] == null)
            return false;

        if (puyos[0] == puyo)
        {
            if (Mathf.Abs(puyos[0].transform.position.y - puyos[1].transform.position.y) >= 0.5f)
            {
                other = puyos[1];
                return true;
            }
        }
        else if (puyos[1] == puyo)
        {
            if (Mathf.Abs(puyos[1].transform.position.y - puyos[0].transform.position.y) >= 0.5f)
            {
                other = puyos[0];
                return true;
            }
        }

        return false;
    }

    public bool IsValidPuyo(PuyoObject puyo)
    {
        if (puyos[0] == puyo || puyos[1] == puyo)
            return true;

        return false;
    }

    public PuyoObject ReturnOther(PuyoObject puyo)
    {
        if (puyos[0] != puyo)
            return puyos[0];
        else
            return puyos[1];
    }

    public bool DeletePuyo(PuyoObject puyo)
    {
        bool isEmpty = false;

        if (puyos[0] == puyo)
        {
            puyos[0] = null;
        }
        else if (puyos[1] == puyo)
        {
            puyos[1] = null;
        }

        if (puyos[0] == null && puyos[1] == null)
            isEmpty = true;

        return isEmpty;
    }

    public void Pause(bool value)
    {
        if (value)
        {
            if (puyos[0] != null)
                puyos[0].movement.StopMoving();
            if (puyos[1] != null)
                puyos[1].movement.StopMoving();
        }
        else
        {
            if (puyos[0] != null)
                puyos[0].movement.StartMoving();
            if (puyos[1] != null)
                puyos[1].movement.StartMoving();
        }
    }
}