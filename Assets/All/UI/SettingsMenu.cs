using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : GameMenu
{
    // References
    [Header("Game")]
    [SerializeField]
    private Slider m_sensitivityXSlider;
    [SerializeField]
    private Text m_sensitivityXValue;
    [SerializeField]
    private Slider m_sensitivityYSlider;
    [SerializeField]
    private Text m_sensitivityYValue;
    [SerializeField]
    private Toggle m_showFPSToggle;

    [Header("Sound")]
    [SerializeField]
    private Slider m_volumeSlider;
    [SerializeField]
    private Text m_volumeValue;
    [SerializeField]
    private SettingsMuteBtn m_muteBtn;
    [SerializeField]
    private Toggle vibrationToggle;
    [SerializeField]
    private Toggle m_buttonSoundToggle;

    [Header("Languages")]
    [SerializeField]
    private Dropdown m_languageDropDown;

    [Header("Menu")]
    [SerializeField]
    private ConfirmMenu m_confirmMenu;

    // Attributes
    private Settings m_settings;
    public Settings settings { get => m_settings; }

    public override void Enable()
    {
        base.Enable();

        m_settings = SaveSystem.GetSettings();

        m_sensitivityXSlider.value = m_settings.sensitivityX / 100f;
        m_sensitivityYSlider.value = m_settings.sensitivityY / 100f;
        m_showFPSToggle.isOn = m_settings.showFPS;
        m_volumeSlider.value = m_settings.volume;

        m_muteBtn.Enable(this);
        vibrationToggle.isOn = m_settings.vibration;
        m_buttonSoundToggle.isOn = m_settings.buttonSound;

        m_languageDropDown.value = (int)m_settings.language;

        m_confirmMenu.Disable();
    }

    public override void Disable()
    {
        base.Disable();
    }

    private void Update()
    {
        m_settings.sensitivityX = (int)(m_sensitivityXSlider.value * 100f);
        m_settings.sensitivityY = (int)(m_sensitivityYSlider.value * 100f);
        m_settings.showFPS = m_showFPSToggle.isOn;

        m_settings.vibration = vibrationToggle.isOn;
        m_settings.volume = m_volumeSlider.value;
        m_settings.isMuted = m_muteBtn.isMuted;
        m_settings.buttonSound = m_buttonSoundToggle.isOn;

        m_settings.language = (Language)m_languageDropDown.value;

        m_sensitivityXValue.text = "" + m_settings.sensitivityX;
        m_sensitivityYValue.text = "" + m_settings.sensitivityY;
        m_volumeValue.text = "" + (int)(m_settings.volume * 100);

        AudioListener.volume = m_settings.isMuted ? 0 : settings.volume;

        if (isActive && Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    public void Save()
    {
        SaveSystem.SaveSettings(m_settings);
        Languages.SetCurrentOne(m_settings.language);

        MySound.ButtonSound();
        LastMenu();
    }

    public void Quit()
    {
        MySound.ButtonSound();
        Settings savedSettings = SaveSystem.GetSettings();
        if (m_settings == savedSettings)
        {
            LastMenu();
        }
        else
        {
            m_confirmMenu.Enable(this, LastMenu);
        }
    }
}
