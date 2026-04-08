using UnityEngine;

public class MazeMove
{
    public KindOfMove kindOfMove;
}
public class MazeMoveCentral : MazeMove
{
    public MazeCell cell;
    public MazeCellEdge edge;
    public Vector3 startingPos;
    public Vector3 endPos;

    public MazeMoveCentral(MazeCell cell, MazeCellEdge edge)
    {
        this.kindOfMove = KindOfMove.Central;
        this.cell = cell;
        this.edge = edge;
        startingPos = edge.GFX.transform.localPosition;
        endPos = new Vector3(-startingPos.x, startingPos.y, -startingPos.z);
    }
}
