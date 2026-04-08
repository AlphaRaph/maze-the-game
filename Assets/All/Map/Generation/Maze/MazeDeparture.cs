using UnityEngine;

public class MazeDeparture : MazeCellEdge
{
    public override string ToString()
    {
        return "MazeDeparture(" + m_mainCell + ", " + m_direction + ")";
    }

    public void OnTriggerEnter(Collider other)
    {
        GameManager.instance.EnterTheMaze();
    }
}
