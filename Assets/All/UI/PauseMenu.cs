using UnityEngine;

public class PauseMenu : GameMenu
{
    [SerializeField]
    private GameMenu m_settingsMenu;
    [SerializeField]
    private ConfirmMenu m_abandonConfirmMenu;
    [SerializeField]
    private ConfirmMenu m_continueLaterConfirmMenu;

    public override void Enable()
    {
        base.Enable();

        GameManager.instance.PauseTime();
        m_continueLaterConfirmMenu.Disable();
    }

    public override void Disable()
    {
        base.Disable();
    }

    public void BtnResume()
    {
        MySound.ButtonSound();
        LastMenu();
        GameManager.instance.RestartTime();
    }

    public void BtnSettings()
    {
        MySound.ButtonSound();
        m_settingsMenu.Enable(this);
    }

    public void BtnAbandon()
    {
        MySound.ButtonSound();
        m_abandonConfirmMenu.Enable(this, Abandon);
    }

    public void Abandon()
    {
        LastMenu();
        GameManager.instance.FinishGame(Result.Abandon);
    }

    public void BtnMenu()
    {
        MySound.ButtonSound();
        m_continueLaterConfirmMenu.Enable(this, BtnContinueLater);
    }

    public void BtnContinueLater()
    {
        Debug.Log("Hey !!!");
        MySound.ButtonSound();
        GameManager.instance.RestartTime();
        GameManager.instance.FinishGame(Result.ContinueLater);
    }

    public void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Escape))
        {
            BtnMenu();
        }
    }
}
