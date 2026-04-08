using UnityEngine;
using UnityEngine.UI;

public class VibrateBtn : MonoBehaviour
{
    // References
    [SerializeField]
    protected Sprite m_vibrationOnSprite;
    [SerializeField]
    protected Sprite m_vibrationOffSprite;
    protected Image m_image;
    protected Button m_button;

    // Attributes
    public bool vibration { get; protected set; }

    public virtual void Awake()
    {
        Settings settings = SaveSystem.GetSettings();

        m_image = GetComponent<Image>();
        m_button = GetComponent<Button>();
        vibration = settings.vibration;

        RefreshImage();
    }

    public void Enable()
    {
        m_button.enabled = true;
    }

    public void Disable()
    {
        m_button.enabled = false;
    }

    public virtual void Clicked()
    {
        Settings settings = SaveSystem.GetSettings();
        vibration = !vibration;
        settings.vibration = vibration;
        SaveSystem.SaveSettings(settings);

        RefreshImage();
    }

    public virtual void Refresh()
    {
        Settings settings = SaveSystem.GetSettings();

        vibration = settings.vibration;

        RefreshImage();
    }

    public virtual void RefreshImage()
    {
        if (vibration)
        {
            m_image.sprite = m_vibrationOnSprite;
        }
        else
        {
            m_image.sprite = m_vibrationOffSprite;
        }
    }
}
