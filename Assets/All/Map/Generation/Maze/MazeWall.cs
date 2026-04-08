
public class MazeWall : MazeCellEdge
{
    public override string ToString()
    {
        return "MazeWall(" + m_mainCell + ", " + m_direction + ")";
    }
}
