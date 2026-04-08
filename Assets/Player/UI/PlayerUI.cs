using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUI : GameMenu
{
    // References
    [SerializeField]
    private Joystick m_joysitck;
    public Joystick Joystick { get { return m_joysitck; } }
    [SerializeField]
    private Text m_txtCoinQuantity;
    [SerializeField]
    private Timer m_timer;
    [SerializeField]
    private LanguageText m_txtAddTime;
    [SerializeField]
    private LanguageText m_txtLevel;
    [SerializeField]
    private GameMenu m_menuPause;
    [SerializeField]
    private Button m_btnPause;
    [SerializeField]
    private LanguageText m_txtFPS;
    [SerializeField]
    private LanguageText m_moveWallsSecondsTxt;
    [SerializeField]
    private LanguageText m_moveWallsTxt;
    [SerializeField]
    private Image m_abandonPanel;
    [SerializeField]
    private Image m_timeOutPanel;
    [SerializeField]
    private LanguageText m_txtDifficulty;

    // Attributes
    private Settings m_settings;
    private float m_deltaTime;
    public Coroutine addingTimeCoroutine { get; set; }
    private bool isAddingTime;

    public override void Enable()
    {
        base.Enable();

        m_btnPause.enabled = true;

        m_deltaTime = 0.0f;

        m_settings = SaveSystem.GetSettings();

        GameManager.instance.RestartPlayer();

        m_txtAddTime.SetAll("");
        m_moveWallsSecondsTxt.SetAll("");
        m_timeOutPanel.gameObject.SetActive(false);
    }

    public override void PartiallyDisable()
    {
        base.PartiallyDisable();

        GameManager.instance.StopPlayer();
        //m_showFPS = false;
    }

    public void UpdateCoins(int coins)
    {
        if (coins <= 1)
        {
            m_txtCoinQuantity.text = coins + " coin";
        }
        else
        {
            m_txtCoinQuantity.text = coins + " coins";
        }
    }

    public void UpdateTime(float seconds)
    {
        if (seconds > m_timer.duration)
        {
            m_timer.SetDuration((int)seconds);
        }

        if (seconds >= 10f)
        {
            m_timer.UpdateSeconds(seconds);
        }
        else if (seconds > 0f && seconds < 10f)
        {
            m_timer.UpdateLastSeconds(seconds);
        }
        else
        {
            m_timer.ResetTimer();
        }
    }

    public IEnumerator AddTime(int seconds, float wait)
    {
        if (isAddingTime && addingTimeCoroutine != null)
            StopCoroutine(addingTimeCoroutine);

        isAddingTime = true;

        m_txtAddTime.SetAll("+ " + seconds + " s");
        m_txtAddTime.text.color = m_timer.remainingColor;

        yield return new WaitForSeconds(wait);

        m_txtAddTime.SetAll("");

        isAddingTime = false;
    }

    public void SetDuration(int seconds)
    {
        m_timer.SetDuration(seconds);
    }
    public void TimerToInfinity()
    {
        m_timer.SetToInfinity();
    }

    public void UpdateLevel(int level)
    {
        m_txtLevel.Set("Level " + level, "Niveau " + level);
    }

    public void BtnPause ()
    {
        MySound.ButtonSound();
        m_menuPause.PartiallyEnable(this);
    }

    private void Update()
    {
        if (!isActive) return;
        
        m_deltaTime += (Time.unscaledDeltaTime - m_deltaTime) * 0.1f;
        if (m_settings.showFPS)
        {
            float msec = m_deltaTime * 1000.0f;
            float fps = 1.0f / m_deltaTime;
            m_txtFPS.SetAll(string.Format("{0:0.0} ms \n ({1:0.} fps)", msec, fps));
        }
        else
        {
            m_txtFPS.SetAll("");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BtnPause();
        }
    }

    public void UpdateMovesTime(float seconds)
    {
        m_moveWallsSecondsTxt.Set("The walls move in " + (int)seconds + " seconds", 
            "Les murs bougent dans " + (int)seconds + " secondes");
        m_moveWallsSecondsTxt.text.color = MoveTimeColor(seconds);

        m_moveWallsTxt.gameObject.SetActive(false);
    }

    public void MoveWalls()
    {
        m_moveWallsSecondsTxt.Set("The walls move.", "Les murs bougent.");
        m_moveWallsSecondsTxt.text.color = MoveTimeColor(0f);

        m_moveWallsTxt.gameObject.SetActive(true);
    }

    public Color MoveTimeColor(float seconds)
    {
        if (seconds > 20)
        {
            return m_timer.colors[0].durationColor;
        }
        else if (seconds > 10)
        {
            return m_timer.colors[1].durationColor;
        }
        else if (seconds > 5)
        {
            return m_timer.colors[2].durationColor;
        }
        else
        {
            return m_timer.colors[3].durationColor;
        }
        // |-> Bon, pas très bien codé mais c'est pg
    }

    public void TimeOut()
    {
        m_timeOutPanel.gameObject.SetActive(true);
        m_moveWallsTxt.gameObject.SetActive(false);
    }
    public void BtnTimeOut()
    {
        MySound.ButtonSound();
        PlayerDieEndMenu(Result.TimeOut);
    }
    public void Abandon()
    {
        m_abandonPanel.gameObject.SetActive(true);
        m_moveWallsTxt.gameObject.SetActive(false);
    }
    public void BtnAbandon()
    {
        MySound.ButtonSound();
        PlayerDieEndMenu(Result.Abandon);
    }

    public void PlayerDieEndMenu(Result result)
    {

        m_timeOutPanel.gameObject.SetActive(false);
        GameManager.instance.SwitchToEndMenu(result);
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        m_txtDifficulty.Set(difficulty.configuration.en_name, difficulty.configuration.fr_name);
        m_txtDifficulty.text.color = difficulty.configuration.color;
    }
    public void SetStoryLabel()
    {
        m_txtDifficulty.Set("Story", "Histoire");
        m_txtDifficulty.text.color = Color.red;
    }
}
