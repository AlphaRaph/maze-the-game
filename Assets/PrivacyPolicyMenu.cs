using UnityEngine;
using UnityEngine.UI;


public class PrivacyPolicyMenu : GameMenu
{
    [SerializeField]
    private Text m_titleText;
    [SerializeField]
    private Text m_messageText;
    [SerializeField]
    private Image m_background;
    [SerializeField]
    private Text m_privacyPolicyBtnTxt;
    [SerializeField]
    private Text m_okayBtnTxt;
    public Color mainColor
    {
        get
        {
            return m_background.color;
        }
        set
        {
            m_background.color = value;
            m_privacyPolicyBtnTxt.color = value;
            m_okayBtnTxt.color = value;
        }
    }

    public void Okay()
    {
        MySound.ButtonSound();
        LastMenu();
    }
}
