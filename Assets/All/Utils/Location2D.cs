using UnityEngine;

[System.Serializable]
public struct Location2D
{
    [SerializeField]
    private IntVector2 m_firstCoord;
    public IntVector2 firstCoord { get { return m_firstCoord; } set { m_firstCoord = value; } }
    [SerializeField]
    private IntVector2 m_secondCoord;
    public IntVector2 secondCoord { get { return m_secondCoord; } set { m_secondCoord = value; } }

    public IntVector2 Size
    {
        get
        {
            return (m_firstCoord - m_secondCoord).ToPositive();
        }
    }

    public Vector2 Center
    {
        get
        {
            return new Vector2(m_firstCoord.x + (Size.x / 2f), m_firstCoord.y + (Size.y / 2f));
        }
    }

    public Location2D (IntVector2 firstCoord, IntVector2 secondCoord)
    {
        m_firstCoord = IntVector2.Zero;
        m_secondCoord = IntVector2.Zero;

        if (firstCoord.x <= secondCoord.x)
        {
            m_firstCoord.x = firstCoord.x;
            m_secondCoord.x = secondCoord.x;
        }
        else
        {
            m_firstCoord.x = secondCoord.x;
            m_secondCoord.x = firstCoord.x;
        }

        if (firstCoord.y <= secondCoord.y)
        {
            m_firstCoord.y = firstCoord.y;
            m_secondCoord.y = secondCoord.y;
        }
        else
        {
            m_firstCoord.y = secondCoord.y;
            m_secondCoord.y = firstCoord.y;
        }

    }

    public Location2D (Vector2 firstCoord, Vector2 secondCoord)
    {
        m_firstCoord = new IntVector2(firstCoord);
        m_secondCoord = new IntVector2(secondCoord);
    }

    public bool Contain (IntVector2 coord)
    {
        return Contain(coord.x, coord.y);
    }

    public bool Contain(float x, float y)
    {
        return Contain(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }
    public bool Contain(int x, int y)
    {
        return x >= m_firstCoord.x && y >= m_firstCoord.y && x < m_secondCoord.x && y < m_secondCoord.y;
    }

    public float Distance (IntVector2 coord)
    {
        return Distance(coord.x, coord.y);
    }

    public float Distance (int x, int y)
    {
        if (Contain(x, y))
        {
            return 0;
        }
        else
        {
            float distanceX = Mathf.Abs(Center.x - x);
            float distanceY = Mathf.Abs(Center.y - y);
            if (distanceX >= distanceY)
            {
                return distanceX - (Size.x / 2f);
            }
            else
            {
                return distanceY - (Size.y / 2f);
            }
        }
    }

    public static Location2D operator+(Location2D location, IntVector2 addingSize)
    {
        return new Location2D(location.firstCoord - (addingSize / 2f), location.secondCoord + (addingSize / 2f));
    }

    public static Location2D operator *(Location2D location, float multiplicator)
    {
        return new Location2D(location.firstCoord - (location.Size * (multiplicator - 1f) / 2f), location.secondCoord + (location.Size * (multiplicator - 1f) / 2f));
    }

    public Location2D Move(IntVector2 distance)
    {
        return new Location2D(m_firstCoord + distance, m_secondCoord + distance);
    }

    public override string ToString()
    {
        return "(" + m_firstCoord + ", " + m_secondCoord + ")";
    }
}

public static class Location2Ds
{
    public static bool Contain (this Location2D[] array, IntVector2 coord)
    {
        return array.Contain(coord.x, coord.y);
    }
    public static bool Contain (this Location2D[] array, float x, float y)
    {
        foreach (Location2D location in array)
        {
            if (location.Contain(x, y)) return true;
        }
        return false;
    }
}
