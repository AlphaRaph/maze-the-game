using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    // References
    private PlayerController m_controller;
    public PlayerController controller { get { return m_controller; } }
    [SerializeField]
    private PlayerUI m_ui;
    public PlayerUI UI { get { return m_ui; } }
    private PlayerInput m_input;

    // Attributes
    private int m_currentLevel;
    private int m_coins;
    //private int m_health;

    private void Awake()
    {
        // References
        m_controller = GetComponent<PlayerController>();
        m_input = GetComponent<PlayerInput>();

        // Attributes
        m_coins = 0;
        //m_health = 100;
    }

    public void EnableUI()
    {
        m_ui.Enable();
    }
    
    public void AddCoins(int quantity = 1)
    {
        m_coins += quantity;
        m_ui.UpdateCoins(m_coins);
    }

    public void SetLevel(int level)
    {
        m_currentLevel = level;
        m_ui.UpdateLevel(m_currentLevel);
    }
}
