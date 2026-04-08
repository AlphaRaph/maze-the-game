using UnityEngine;
using UnityEngine.UI;


public delegate void Func();


public class ConfirmMenu : GameMenu
{
    [SerializeField]
    private Text m_titleText;
    [SerializeField]
    private Text m_messageText;
    [SerializeField]
    private Image m_background;
    [SerializeField]
    private Button m_quitBtn;
    [SerializeField]
    private Text m_cancelBtnTxt;
    [SerializeField]
    private Text m_confirmBtnTxt;
    public Color mainColor
    {
        get
        {
            return m_background.color;
        }
        set
        {
            m_background.color = value;
            m_quitBtn.image.color = value;
            m_cancelBtnTxt.color = value;
            m_confirmBtnTxt.color = value;
        }
    }

    private Func m_confirmFuncPointer = null;

    public void Enable(GameMenu lastMenu, Func confirmFuncPointer)
    {
        m_confirmFuncPointer = confirmFuncPointer;
        PartiallyEnable(lastMenu);
    }

    public void SetTitleText(string text)
    {
        m_titleText.text = text;
    }

    public void SetMessageText(string text)
    {
        m_messageText.text = text;
    }

    public void Confirm()
    {
        MySound.ButtonSound();
        m_confirmFuncPointer?.Invoke(); // Call the function if it is not null
    }

    public void Cancel()
    {
        MySound.ButtonSound();
        LastMenu();
    }

    public void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }
}
