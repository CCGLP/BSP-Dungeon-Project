using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct PositiveVector2 {
    [SerializeField]
    private uint x, y; 
	public PositiveVector2(uint x, uint y)
    {
        this.x = x;
        this.y = y; 
    }

    public PositiveVector2(int x, int y)
    {
        this.x = (uint) Mathf.Abs(x);
        this.y = (uint) Mathf.Abs(y); 
    }

    public PositiveVector2(PositiveVector2 other)
    {
        this.x = other.x;
        this.y = other.y; 
    }

    public static PositiveVector2 operator / (PositiveVector2 value, float div)
    {
        return new PositiveVector2((int)(value.x / div), (int)(value.y / div));
    }
   



    public uint uX
    {
        get
        {
            return x; 
        }

        set
        {
            x = value; 
        }
    }

    public uint uY
    {
        get
        {
            return y; 
        }

        set
        {
            y = value; 
        }
    }

    public int X
    {
        get
        {
            return (int) x;
        }

        set
        {
            x = (uint) Mathf.Abs(value);
        }
    }

    public int Y
    {
        get
        {
            return (int) y;
        }

        set
        {
            y = (uint) Mathf.Abs(value);
        }
    }
}
