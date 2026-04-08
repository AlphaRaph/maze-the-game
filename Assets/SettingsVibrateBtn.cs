using UnityEngine.UI;

public class SettingsVibrateBtn : VibrateBtn
{
    public override void Awake()
    {
        // Rien car on veut juste empcher la méthode parente de faire quelque chose.
    }

    public void Enable(SettingsMenu settingsMenu)
    {
        m_image = GetComponent<Image>();
        m_button = GetComponent<Button>();
        vibration = settingsMenu.settings.vibration;

        RefreshImage();
    }

    public override void Clicked()
    {
        vibration = !vibration;

        RefreshImage();
    }
}
