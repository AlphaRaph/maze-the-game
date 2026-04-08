using UnityEngine;
using UnityEngine.UI;

public delegate void Func1(Difficulty difficulty);

public class ChoiceMenu : GameMenu
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
    private Text m_leftChoiceTxt;
    [SerializeField]
    private Text m_rightChoiceTxt;
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
            m_leftChoiceTxt.color = value;
            m_rightChoiceTxt.color = value;
        }
    }

    private Func1 m_leftChoicePtr = null;
    private Func1 m_rightChoicePtr = null;
    private Difficulty m_difficulty = null;

    public void Enable(GameMenu lastMenu, Func1 leftChoicePtr, Func1 rightChoicePtr, Difficulty difficulty)
    {
        m_leftChoicePtr = leftChoicePtr;
        m_rightChoicePtr = rightChoicePtr;
        m_difficulty = difficulty;
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

    public void LeftChoice()
    {
        MySound.ButtonSound();
        m_leftChoicePtr?.Invoke(m_difficulty); // Call the function if it is not null
    }

    public void RightChoice()
    {
        MySound.ButtonSound();
        m_rightChoicePtr?.Invoke(m_difficulty); // Call the function it it's not null
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
            LastMenu();
        }
    }
}
