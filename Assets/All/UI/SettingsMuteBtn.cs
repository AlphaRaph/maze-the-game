using UnityEngine.UI;

public class SettingsMuteBtn : MuteBtn
{
    public override void Awake()
    {
        // Rien car on veut juste empcher la méthode parente de faire quelque chose.
    }

    public void Enable(SettingsMenu settingsMenu)
    {
        m_image = GetComponent<Image>();
        m_button = GetComponent<Button>();
        isMuted = settingsMenu.settings.isMuted;

        RefreshImage();
    }

    public override void Clicked()
    {
        isMuted = !isMuted;

        RefreshImage();
    }
}
