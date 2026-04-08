using UnityEngine;

public class Coin : MonoBehaviour
{
    // Refrences
    private MyMaze m_maze;
    
    // Attributes
    //[SerializeField]
    //private int m_value = 1;
    [SerializeField]
    private float m_rotateSpeed = 35f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, m_rotateSpeed * Time.deltaTime, 0));
    }

    public void Initialize(MyMaze maze)
    {
        m_maze = maze;
    }

    private void OnTriggerEnter(Collider other)
    {
        //GameManager.instance.AddCoinsToPlayer(m_value);
        //m_maze.RemoveCoin(this);
        //Destroy(gameObject);
    }
}
