using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    // References   
    protected GameMenu m_lastMenu;
    [SerializeField]
    protected List<Button> m_buttons;

    // Properties
    public bool isActive { get; protected set; }

    public virtual void Enable()
    {
        gameObject.SetActive(true);
        EnableButtons();
        isActive = true;
    }

    public virtual void Enable(GameMenu lastMenu)
    {
        m_lastMenu = lastMenu;
        m_lastMenu.Disable();
        Enable();
    }

    public virtual void PartiallyEnable(GameMenu lastMenu)
    {
        m_lastMenu = lastMenu;
        m_lastMenu.PartiallyDisable();
        Enable();
    }

    public virtual void Disable()
    {
        gameObject.SetActive(false);
        isActive = false;
    }

    public virtual void PartiallyDisable()
    {
        DisableButtons();
        isActive = false;
    }

    public virtual void EnableButtons()
    {
        foreach (Button button in m_buttons)
        {
            button.enabled = true;
        }
    }

    public virtual void DisableButtons()
    {
        foreach (Button button in m_buttons)
        {
            button.enabled = false;
        }
    }

    protected virtual void LastMenu()
    {
        if (m_lastMenu != null)
        {
            m_lastMenu.Enable();
        }
        Disable();
    }
}
