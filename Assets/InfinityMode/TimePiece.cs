using UnityEngine;

public class TimePiece : MonoBehaviour
{
    // Attributes
    private MyMaze m_maze;
    [SerializeField]
    private int m_value = 1;
    [SerializeField]
    private float m_rotateSpeed = 35f;
    [SerializeField]
    private float m_moveSpeed = 0.1f;
    [SerializeField]
    private float m_collisionRadius = 1;
    [SerializeField]
    private float m_heightMultiplicator = 2;
    [SerializeField]
    private AnimationCurve m_heightCurve;

    // Properties
    public int value { get { return m_value; } }
    public int index { get; private set; }
    //public float timeValue { get; set; } // Rien à voir avec la valeur de la pièce
    public float rotateSpeed { get { return m_rotateSpeed; } }
    public float moveSpeed { get { return m_moveSpeed; } }
    public float collisionRadius { get { return m_collisionRadius; } }
    public float heightMultiplicator { get { return m_heightMultiplicator; } }
    public AnimationCurve heightCurve { get { return m_heightCurve; } }

    public void Initialize(MyMaze maze, int index)
    {
        m_maze = maze;
        //this.timeValue = timeValue;
        this.index = index;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Je ne devrais pas être appelé");
        //if (other.tag == "Player") {
        //    ((GameManagerInfinity)GameManager.instance).AddTime(m_value);
        //    m_maze.RemoveTimePiece(this);
        //    Destroy(gameObject);
        //}
    }
}
