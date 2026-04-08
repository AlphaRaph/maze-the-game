using UnityEngine;
using UnityEngine.UI;

public class HomeMenu : GameMenu
{
    // References 
    [SerializeField]
    private HomeManager m_homeManager;
    [SerializeField]
    private GameMenu m_settingsMenu;
    [SerializeField]
    private ConfirmMenu m_confirmMenu;
    [SerializeField]
    private InfinityMenu m_infinityMenu;
    [SerializeField]
    private MuteBtn m_muteBtn;
    [SerializeField]
    private Title m_title;
    public Title title { get { return m_title; } }
    [SerializeField]
    private Button m_infinityButton;
    public Button infinityButton { get { return m_infinityButton; } }
    [SerializeField]
    private Button m_storyButton;
    public Button storyButton { get { return m_storyButton; } }
    [SerializeField]
    private PrivacyPolicyMenu m_privacyPolicyMenu;

    public override void Enable()
    {
        base.Enable();

        m_homeManager.updateWithMusic = true;
        m_homeManager.changeThemes = true;
        m_muteBtn.Refresh();

        m_confirmMenu.Disable();
    }

    public override void Disable()
    {
        base.Disable();

        m_homeManager.updateWithMusic = false;
        m_homeManager.changeThemes = false;
    }

    public override void PartiallyDisable()
    {
        base.PartiallyDisable();

        m_homeManager.updateWithMusic = false;
        m_homeManager.updateWithMusic = false;
    }

    public void Infinity()
    {
        MySound.ButtonSound();
        MySceneManager.LoadScene("InfinityMenu");
    }

    public void Story()
    {
        MySound.ButtonSound();
        MySceneManager.LoadScene("StoryMenu");
    }

    public void Settings()
    {
        MySound.ButtonSound();
        m_settingsMenu.PartiallyEnable(this);
    }

    public void Update()
    {
        

        if (isActive && Input.GetKeyDown(KeyCode.Escape))
        {
            m_confirmMenu.mainColor = m_homeManager.currentTheme.fgColor;
            m_confirmMenu.Enable(this, QuitGame);
        }

        if (m_confirmMenu.enabled)
        {
            m_confirmMenu.mainColor = m_homeManager.currentTheme.fgColor;
        }

        if (m_privacyPolicyMenu.enabled)
        {
            m_privacyPolicyMenu.mainColor = m_homeManager.currentTheme.fgColor;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PrivacyPolicyMenu()
    {
        m_privacyPolicyMenu.mainColor = m_homeManager.currentTheme.fgColor;
        m_privacyPolicyMenu.PartiallyEnable(this);
    }
}
