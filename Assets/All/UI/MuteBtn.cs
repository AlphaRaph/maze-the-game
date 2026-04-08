using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MuteBtn : MonoBehaviour
{
    // References
    [SerializeField]
    protected Sprite m_speakerOnSprite;
    [SerializeField]
    protected Sprite m_speakerOffSprite;
    protected Image m_image;
    protected Button m_button;

    // Attributes
    public bool isMuted { get; protected set; }

    public virtual void Awake()
    {
        Settings settings = SaveSystem.GetSettings();

        m_image = GetComponent<Image>();
        m_button = GetComponent<Button>();
        isMuted = settings.isMuted;
        AudioListener.volume = isMuted ? 0 : settings.volume;

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

        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0 : settings.volume;

        settings.isMuted = isMuted;
        SaveSystem.SaveSettings(settings);

        RefreshImage();
    }

    public virtual void Refresh()
    {
        Settings settings = SaveSystem.GetSettings();

        isMuted = settings.isMuted;
        AudioListener.volume = isMuted ? 0 : settings.volume;

        RefreshImage();
    }

    public virtual void RefreshImage()
    {
        if (isMuted)
        {
            m_image.sprite = m_speakerOffSprite;
        }
        else
        {
            m_image.sprite = m_speakerOnSprite;
        }
    }
}
