using UnityEngine ;
using UnityEngine.UI ;
using System.Collections.Generic;

public class Timer : MonoBehaviour
{
    // References
    [Header ("References")]
    [SerializeField]
    private Image m_remainingDurationImage;
    [SerializeField]
    private Text m_remainingDurationText;
    private Image m_durationImage;
    [SerializeField]
    private Image m_infinitySymbol;

    // Settings
    [SerializeField]
    private List<TimerColor> m_colors;
    public List<TimerColor> colors { get { return m_colors; } }

    // Properties
    public int duration { get; private set; }
    public Color durationColor
    {
        get
        {
            return m_durationImage.color;
        }
        set
        {
            m_remainingDurationImage.color = value;
            m_remainingDurationText.color = value;
        }
    }
    public Color remainingColor {
        get
        {
            return m_remainingDurationImage.color;
        }
        private set
        {
            m_durationImage.color = value;
        }
    }

    // Attributes
    private float m_remainingDuration;

    private void Awake ()
    {
        m_durationImage = GetComponent<Image>();
        ResetTimer();
    }

    public Timer SetDuration (int seconds)
    {
        duration = seconds;
        return this ;
    }
    public void SetToInfinity()
    {
        duration = 0;
        m_remainingDurationText.text = "";
        m_remainingDurationImage.fillAmount = 1;
        remainingColor = m_colors[0].remainingColor;
        durationColor = m_colors[0].durationColor;

        m_infinitySymbol.gameObject.SetActive(true);
    }

    public void ResetTimer()
    {
        m_remainingDurationText.text = "00:00";
        m_remainingDurationImage.fillAmount = 0f;

        m_remainingDuration = duration = 0;
    }
    public void UpdateSeconds (float seconds)
    {
        if ((int)m_remainingDuration != (int) seconds)
        {
            m_remainingDuration = seconds;
            m_remainingDurationText.text = string.Format("{0:D2}:{1:D2}", (int)seconds / 60, (int)seconds % 60);
            m_remainingDurationImage.fillAmount = Mathf.InverseLerp(0, duration, seconds);

            UpdateColor();
        }
        else
        {
            m_remainingDuration = seconds;
        }
    }
    public void UpdateLastSeconds (float lastSeconds)
    {
        m_remainingDuration = lastSeconds;

        int seconds = (int)lastSeconds;
        int milliseconds = (int)((lastSeconds - seconds) * 100f);

        m_remainingDurationText.text = string.Format("{0:D2}:{1:D2}", seconds, milliseconds);
        m_remainingDurationImage.fillAmount = Mathf.InverseLerp(0, duration, lastSeconds);

        UpdateColor();
    }

    private TimerColor timerColor, nextTimerColor;
    public void UpdateColor()
    {
        if (m_colors.Count > 0 && 
            !(timerColor != null && ((nextTimerColor == null) || (m_remainingDuration < timerColor.time && m_remainingDuration >= nextTimerColor.time))))
        {
            Color remainingColor = this.remainingColor;
            Color durationColor = this.durationColor;
            for (int i = 0; i < m_colors.Count; i++)
            {
                if (m_remainingDuration < m_colors[i].time)
                {
                    timerColor = m_colors[i];
                    if (i < m_colors.Count - 1)
                    {
                        nextTimerColor = m_colors[i + 1];
                    }
                    else
                    {
                        nextTimerColor = null;
                    }
                    remainingColor = timerColor.remainingColor;
                    durationColor = timerColor.durationColor;
                }
            }

            this.remainingColor = remainingColor;
            this.durationColor = durationColor;
        }
    }
}

[System.Serializable]
public class TimerColor
{
    public string name;
    public Color remainingColor;
    public Color durationColor;
    public float time;
}
