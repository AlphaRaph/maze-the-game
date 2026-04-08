using UnityEngine;

public class MazeCell : MonoBehaviour
{
    // References
    private MyMaze m_maze;
    private MazeChunk m_chunk;
    private IntVector2 m_coord;
    private MazeCellEdge[] m_edges = new MazeCellEdge[8];
    public TimePiece timePiece { get; private set; }
    public bool hasTimePiece { get; private set; }

    // Attributes
    private int m_initalizedMainEdgeCount = 0;

    public void Initialize(MyMaze maze, MazeChunk chunk, IntVector2 coord)
    {
        m_maze = maze;
        m_chunk = chunk;
        m_coord = coord;
        hasTimePiece = timePiece != null;

        transform.position = maze.ToRealPosition(coord);
        transform.rotation = Quaternion.identity;
        transform.parent = chunk.transform;
        transform.name = ToString();
    }

    #region Edges

    public MazeCellEdge GetEdge(int index)
    {
        return m_edges[index];
    }
    public Direction GetEdgeDirection(MazeCellEdge edge)
    {
        for (int i = 0; i < CardinalPoints.Count; i++)
        {
            if (m_edges[i] == edge)
            {
                return CardinalPoints.Get(i);
            }
        }

        throw new System.InvalidOperationException("Je ne connais pas ce bord.");
    }
    public void SetEdge(Direction direction, MazeCellEdge edge)
    {
        int iDir = CardinalPoints.ToNumber(direction);

        if (CardinalPoints.IsMainCardinalPoint(direction))
        {
            if (m_edges[iDir] != null)
            {
                if (edge == null)
                    m_initalizedMainEdgeCount--;
            }
            else
            {
                m_initalizedMainEdgeCount++;
            }
        }

        m_edges[iDir] = edge;
    }

    #endregion

    #region TimePieces

    public void SetTimePiece(TimePiece piece)
    {
        timePiece = piece;
        hasTimePiece = true;

        //Debug.Log("J'ai été appelé et heureusement !");
    }
    public void HeightTimePiece (float y)
    {
        timePiece.transform.position = new Vector3(timePiece.transform.position.x, 
            (timePiece.heightCurve.Evaluate(y) - 0.5f) * timePiece.heightMultiplicator, timePiece.transform.position.z);
    }
    public void FixedRotateTimePiece ()
    {
        timePiece.transform.Rotate(new Vector3(0, timePiece.rotateSpeed * Time.fixedDeltaTime, 0));
    }
    public void RemoveTimePiece()
    {
        timePiece = null;
        hasTimePiece = false;
        Debug.LogError("J'ai suis là pour vous hanter !");
    }
    public void DestroyTimePiece()
    {
        if (timePiece != null)
            Destroy(timePiece.gameObject);
        timePiece = null;
        hasTimePiece = false;
    }
    public bool IsPlayerTouchingPiece(Vector2 playerPosition)
    {
        if (!hasTimePiece)
            return false;

        float playerRadius = 0.5f;
        if (timePiece == null) Debug.Log("He he he, je suis null !");

        float radius = timePiece.collisionRadius;
        Vector3 position = timePiece.transform.position;

        //Debug.Log("playerPosition : " + playerPosition);
        //Debug.Log("piecePosition : " + position);

        return position.y + radius >= 0 &&
            playerPosition.x + playerRadius >= position.x - radius && playerPosition.x - playerRadius <= position.x + playerRadius &&
            playerPosition.y + playerRadius >= position.z - radius && playerPosition.y - playerRadius <= position.z + playerRadius;
    }
    #endregion

    public override string ToString()
    {
        if (m_maze == null)
            return base.ToString();
        else 
            return "MazeCell" + m_coord;
    }
}
