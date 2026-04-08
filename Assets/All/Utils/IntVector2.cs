using UnityEngine;

[System.Serializable]
public struct IntVector2
{
    public static IntVector2 Zero { get { return new IntVector2(0, 0); } }
    public static IntVector2 One { get { return new IntVector2(1, 1); } }
    public static IntVector2 OneX { get { return new IntVector2(1, 0); } }
    public static IntVector2 OneY { get { return new IntVector2(0, 1); } }
    public static IntVector2 NegativeOne { get { return new IntVector2(-1, -1); } }
    public static IntVector2 NegativeOneX { get { return new IntVector2(-1, 0); } }
    public static IntVector2 NegativeOneY { get { return new IntVector2(0, -1); } }

    [SerializeField]
    private int m_x; // Needing for the inspector
    [SerializeField]
    private int m_y; // Needing for the inspector
    public int x { get { return m_x; } set { m_x = value; } }
    public int y { get { return m_y; } set { m_y = value; } }

    public IntVector2(int x, int y)
    {
        m_x = x;
        m_y = y;
    }

    public IntVector2(float x, float y)
    {
        m_x = Mathf.RoundToInt(x);
        m_y = Mathf.RoundToInt(y);
    }

    public IntVector2(Vector2 vector2)
    {
        m_x = Mathf.RoundToInt(vector2.x);
        m_y = Mathf.RoundToInt(vector2.y);
    }

    public static IntVector2 operator+(IntVector2 a, IntVector2 b)
    {
        a.x += b.x;
        a.y += b.y;
        return a;
    }

    public static IntVector2 operator-(IntVector2 a, IntVector2 b)
    {
        a.x -= b.x;
        a.y -= b.y;
        return a;
    }

    public static IntVector2 operator*(IntVector2 a, int b)
    {
        a.x *= b;
        a.y *= b;
        return a;
    }

    public static IntVector2 operator/(IntVector2 a, int b)
    {
        a.x /= b;
        a.y /= b;
        return a;
    }

    public static IntVector2 operator+(IntVector2 a, Vector2 b)
    {
        a.x += Mathf.RoundToInt(b.x);
        a.y += Mathf.RoundToInt(b.y);
        return a;
    }

    public static IntVector2 operator-(IntVector2 a, Vector2 b)
    {
        a.x -= Mathf.RoundToInt(b.x);
        a.y -= Mathf.RoundToInt(b.y);
        return a;
    }

    public static IntVector2 operator*(IntVector2 a, float b)
    {
        a.x = Mathf.RoundToInt(a.x * b);
        a.y = Mathf.RoundToInt(a.y * b);
        return a;
    }

    public static IntVector2 operator/(IntVector2 a, float b)
    {
        a.x = Mathf.RoundToInt(a.x / b);
        a.y = Mathf.RoundToInt(a.y / b);
        return a;
    }

    public static bool operator==(IntVector2 a, IntVector2 b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator!=(IntVector2 a, IntVector2 b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is IntVector2)
        {
            return this == (IntVector2)obj;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

    public IntVector2 ToPositive()
    {
        return new IntVector2(Mathf.Abs(x), Mathf.Abs(y));
    }

    public Vector2 ToVector2
    {
        get
        {
            return new Vector2(x, y);
        }       
    }
}
