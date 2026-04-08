using UnityEngine;

public class MazeChunk : MonoBehaviour
{
    private MyMaze m_maze;
    private IntVector2 m_coord;
    private int m_removeAdvancement;
    [SerializeField]
    private int m_lifeTime = 10;
    private bool m_stayActiveForEver;

    public void Initialize(MyMaze maze, int x, int y)
    {
        m_maze = maze;
        m_coord = new IntVector2(x, y);
        gameObject.name = ToString();
        m_stayActiveForEver = true;
    }
    public void StayActive()
    {
        m_stayActiveForEver = false;
        m_removeAdvancement = m_maze.piecesAdvancement + m_lifeTime;
    }
    public void StayActiveForEver()
    {
        m_stayActiveForEver = true;
    }
    public void Disable()
    {
        m_stayActiveForEver = false;
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (m_stayActiveForEver) return;
        if (m_maze.piecesAdvancement >= m_removeAdvancement)
            Disable();
    }

    public override string ToString()
    {
        if (m_maze == null)
            return base.ToString();
        else
            return "MazeChunk" + m_coord;
    }
}
