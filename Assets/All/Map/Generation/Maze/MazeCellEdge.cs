using UnityEngine;

// Classe abstraite MazeCellEdge qui permet de définir les bords d'une cellule
// Comme c'est une classe abstraite on ne peut pas créer d'instance d'elle même mais que de ses enfants
public abstract class MazeCellEdge : MonoBehaviour
{
    // Debug
    protected MyMaze m_maze;
    protected MazeCell m_mainCell;
    protected Direction m_direction;
    protected MazeCell[] m_otherCells;

    // References
    [SerializeField]
    protected GameObject gfx;
    public GameObject GFX { get { return gfx; } }

    public void Initialize(MyMaze maze, MazeCell mainCell, Direction direction)
    {
        m_maze = maze;
        m_mainCell = mainCell;
        m_direction = direction;

        transform.parent = mainCell.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = direction.ToRotation;
        transform.name = ToString();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public int Value
    {
        get
        {
            if (this is MazePassage)
            {
                return 0;
            }
            else if (this is MazeWall)
            {
                return 1;
            }
            else if (this is MazeDeparture)
            {
                return 2;
            }
            else if (this is MazeArrival)
            {
                return 3;
            }
            else if (this is MazePilar)
            {
                return 4;
            }
            else
            {
                return -1;
            }
        }
    }

    public void ChangeMainCell(MazeCell newMainCell)
    {
        m_mainCell = newMainCell;
        m_direction = m_mainCell.GetEdgeDirection(this);

        transform.parent = m_mainCell.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = m_direction.ToRotation;
        transform.name = ToString();
    }

    public void FinishCentralMove()
    {
        gfx.transform.localPosition = new Vector3(-gfx.transform.localPosition.x, gfx.transform.localPosition.y, -gfx.transform.localPosition.z);
        transform.localRotation = m_direction.Opposite.ToRotation;
    }
}
