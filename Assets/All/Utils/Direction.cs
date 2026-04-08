using System;
using UnityEngine;

/// <summary>
/// Utilitaire
/// </summary>

// Enumaration de 8 point cardinaux (Nord, Nord-Est, Est, Sud-Est, Sud, Sud-Ouest, Ouest, Nord-Ouest)
// Cela permet de choisir l'une de c'est 8 direction
[System.Serializable]
public class Direction
{
    private Vector2 m_vector2;
    public Vector2 ToVector2 {
        get
        {
            return m_vector2;
        }
    }
    public IntVector2 ToIntVector2 {
        get
        {
            return new IntVector2(Mathf.RoundToInt(m_vector2.x), Mathf.RoundToInt(m_vector2.y));
        }
    }
    public Direction Opposite {
        get
        {
            return new Direction(-m_vector2);
        }
    }
    public Quaternion ToRotation {
        get {
            // For that operation, I'm using the scalaire product, I just reduce the operations in the same line
            // https://www.editions-petiteelisabeth.fr/calculs_angle_vecteurs.php for more informations
            if (m_vector2.x < 0)
                return Quaternion.Euler(0, -Mathf.Rad2Deg * Mathf.Acos(m_vector2.y / Mathf.Sqrt(m_vector2.x * m_vector2.x + m_vector2.y * m_vector2.y)), 0);
            else
                return Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Acos(m_vector2.y / Mathf.Sqrt(m_vector2.x * m_vector2.x + m_vector2.y * m_vector2.y)), 0);
        }
    }

    public Direction(Vector2 direction)
    {
        this.m_vector2 = direction;
    }

    public static bool operator ==(Direction a, Direction b)
    {
        return Mathf.Approximately(a.m_vector2.x, b.m_vector2.x) && Mathf.Approximately(a.m_vector2.y, b.m_vector2.y);
    }

    public static bool operator !=(Direction a, Direction b)
    {
        return  !(a==b);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        if (CardinalPoints.IsCardinalPoint(this))
            return CardinalPoints.Name(this) + m_vector2;
        else
            return "Direction(" + m_vector2 + ")";
    }
}

// Classe statique MazeDirections qui représente les directions et qui permet de les convertir en IntVector2 ou Quaternion
public static class CardinalPoints
{
    public static readonly Direction North = new Direction(new Vector2(0, 1));
    public static readonly Direction NorthEast = new Direction(new Vector2(1, 1));
    public static readonly Direction East = new Direction(new Vector2(1, 0));
    public static readonly Direction SouthEast = new Direction(new Vector2(1, -1));
    public static readonly Direction South = new Direction(new Vector2(0, -1));
    public static readonly Direction SouthWest = new Direction(new Vector2(-1, -1));
    public static readonly Direction West = new Direction(new Vector2(-1, 0));
    public static readonly Direction NorthWest = new Direction(new Vector2(-1, 1));

    private static readonly Direction[] cardinalPoints = new Direction[8]
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    };
    public const int MainCount = 4;
    public const int Count = 8;

    public static Direction Get(int index)
    {
        while (index >= Count)
        {
            index -= Count;
        }
        while (index < 0)
        {
            index += Count;
        }
        return cardinalPoints[index];
    }

    public static Direction GetMain(int index)
    {
        index *= 2;
        while (index >= Count)
        {
            index -= Count;
        }
        while (index < 0)
        {
            index += Count;
        }
        return cardinalPoints[index];
    }

    public static int ToNumber(Direction direction)
    {
        for (int i = 0; i < Count; i++)
        {
            if (cardinalPoints[i] == direction)
                return i;
        }
        return -1;
    }

    public static bool IsCardinalPoint(Direction direction)
    {
        foreach (Direction dir in cardinalPoints)
        {
            if (dir == direction)
                return true;
        }
        return false;
    }

    public static bool IsMainCardinalPoint(Direction direction)
    {
        for (int i = 0; i < Count; i+=2)
        {
            if (cardinalPoints[i] == direction)
                return true;
        }
        return false;
    }

    public static bool IsMainIndexOfCardinalPoint(int index)
    {
        if (index < 0 || index >= Count)
            throw new InvalidOperationException("Index of the direction is less than 0 or greater than CardinalPoints.Count");

        return index % 2 == 0;
    }

    public static Direction Last(Direction direction, int i = 1)
    {
        return Get(ToNumber(direction) - i);
    }

    public static Direction Next(Direction direction, int i = 1)
    {
        return Get(ToNumber(direction) + i);
    }

    public static string Name(Direction direction)
    {
        if (!IsCardinalPoint(direction))
            throw new Exception("Name() : La direction n'est pas un point cardinaux.");

        if (direction == North)
            return "North";
        else if (direction == NorthEast)
            return "NorthEast";
        else if (direction == East)
            return "East";
        else if (direction == SouthEast)
            return "SouthEast";
        else if (direction == South)
            return "South";
        else if (direction == SouthWest)
            return "SouthWest";
        else if (direction == West)
            return "West";
        else
            return "NorthWest";
    }

    public static int ReverseIndexOfDirection(int index)
    {
        if (index < 0 || index >= Count)
            throw new InvalidOperationException("Index of the direction is less than 0 or greater than CardinalPoints.Count");
        return (index - 4 < 0) ? index + 4 : index - 4;
    }

    public static int ReverseIndexOfMainDirection(int index)
    {
        if (index < 0 || index >= MainCount)
            throw new InvalidOperationException("Index of the direction is less than 0 or greater than CardinalPoints.MainCount");
        return (index - 2 < 0) ? index + 2 : index - 2;
    }
}
