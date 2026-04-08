using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // References
    [SerializeField]
    private Transform m_cameraTransform;
    private CharacterController m_controller;
    private CapsuleCollider m_collider;
    private PlayerInput m_input;
    //[SerializeField]
    //private Transform m_pathParent;
    //[SerializeField]
    //private Brush m_brush;

    // Settings
    [SerializeField]
    private float m_sensitivityFactor = 0.35f;
    private float m_cameraSensitivityX, m_cameraSensitivityY;
    [SerializeField]
    private float m_moveSpeed = 8f;
    public float moveSpeed { get => m_moveSpeed; }
    [SerializeField]
    private float m_moveInputDeadZone = 10f;
    [SerializeField]
    private float m_gravity = -9.81f;

    // Attributes
    private Vector3 m_velocity;
    private float m_cameraPitch;
    private Vector2 m_moveTouchStartPosition;
    public bool CanMove { get; set; }
    //public bool drawWay { get { return m_brush.drawing; } set { m_brush.drawing = value; } }

    // Start is called before the first frame update
    void Start()
    {
        Refresh();

        CanMove = true;
        //drawWay = false;
    }

    public void Refresh()
    {
        // get components
        m_controller = GetComponent<CharacterController>();
        m_collider = GetComponent<CapsuleCollider>();
        m_input = GetComponent<PlayerInput>();

        // get settings
        Settings settings = SaveSystem.GetSettings();
        m_cameraSensitivityX = settings.sensitivityX * m_sensitivityFactor;
        m_cameraSensitivityY = settings.sensitivityY * m_sensitivityFactor;

        // calculate the movement input dead zone
        m_moveInputDeadZone = Mathf.Pow(Screen.height / m_moveInputDeadZone, 2);

        // initialize brush
        //m_brush.Initialize(GameManager.instance.map.maze.playerWayTransform);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            // Look Around
            LookAround();

            // Move
            Move();

            // Gravity
            if (m_controller.isGrounded && m_velocity.y < 0)
                m_velocity.y = -2f;
            m_velocity.y += m_gravity * Time.deltaTime;
            m_controller.Move(m_velocity * Time.deltaTime);
        }
    }

    private void LookAround()
    {
        // vertical (pitch) rotation
        m_cameraPitch = Mathf.Clamp(m_cameraPitch - m_input.LookInput.y * Time.deltaTime * m_cameraSensitivityY, -90f, 90f);
        m_cameraTransform.localRotation = Quaternion.Euler(m_cameraPitch, 0, 0);

        // horizontal (yaw) rotation
        transform.Rotate(transform.up, m_input.LookInput.x * Time.deltaTime * m_cameraSensitivityX);
    }

    private void Move()
    {
        // Multiply the normalized direction by the speed
        float x = m_input.Horizontal;
        float z = m_input.Vertical;

        Vector3 move = transform.right * x + transform.forward * z;

        // Move relatively to the local transform's direction
        m_controller.Move(move * m_moveSpeed * Time.deltaTime);
    }

    //public void ErasePlayerWay()
    //{
    //    m_brush.Erase();
    //}

    public void EnablePhysics()
    {
        CanMove = true;
        m_controller.enabled = true;
        m_collider.enabled = false;
    }

    public void DisablePhysics()
    {
        CanMove = false;
        m_controller.enabled = true;
        m_collider.enabled = false;
    }
}
