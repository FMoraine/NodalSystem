using UnityEngine;
using System;

[Serializable]
public struct Vector2Integer
{
    public Vector2Integer(int pX, int pY)
    {
        x = pX;
        y = pY;
    }

    public static implicit operator Vector2(Vector2Integer d)
    {
        return new Vector2(d.x, d.y);
    }

    public static implicit operator Vector2Integer(Vector2 d)
    {
        return new Vector2Integer((int)d.x, (int)d.y);
    }

    public int x;
    public int y;
}