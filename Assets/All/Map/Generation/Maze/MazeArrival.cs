using UnityEngine;

public class MazeArrival : MazeCellEdge
{
    public override string ToString()
    {
        return "MazeArrival(" + m_mainCell + ", " + m_direction + ")";
    }

    public void OnTriggerEnter(Collider collider)
    {
        GameManager.instance.FinishGame(Result.FinishLevel);
    }
}
