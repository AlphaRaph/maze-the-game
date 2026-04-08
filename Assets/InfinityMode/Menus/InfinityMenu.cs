using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfinityMenu : GameMenu
{
    // Difficulties
    private DifficultyManager m_difficultyManager;

    // References
    [SerializeField]
    private InfinityPanel m_mountainPanel;
    [SerializeField]
    private InfinityPanel m_desertPanel;
    private InfinityPanel m_currentPanel;
    [SerializeField]
    private RectTransform m_panels;
    [SerializeField]
    private float m_switchSeconds;
    //[SerializeField]
    //private ButtonSwitchPanel m_lastKOLButton;
    //[SerializeField]
    //private ButtonSwitchPanel m_nextKOLButton;
    //public ButtonSwitchPanel currentBtnSwitchPanel { get; private set; }
    [SerializeField]
    private ChoiceMenu m_continueOrRestartMenu;

    public void Enable(DifficultyManager difficultyManager)
    {
        m_difficultyManager = difficultyManager;
        m_continueOrRestartMenu.Disable();
        m_mountainPanel.Initialize(m_difficultyManager, this);
        m_desertPanel.Initialize(m_difficultyManager, this);
        //currentBtnSwitchPanel = m_nextKOLButton;

        base.Enable();

        SwitchToMountainImmediately();

        Time.timeScale = 1;
    }

    public void LastKindOfLevel()
    {
        MySound.ButtonSound();
        if (m_currentPanel != m_mountainPanel)
        {
            StartCoroutine(SwitchToMountain());
        }
    }
    public void NextKindOfLevel()
    {
        MySound.ButtonSound();
        if (m_currentPanel != m_desertPanel)
        {
            StartCoroutine(SwitchToDesert());
        }
    }

    private IEnumerator SwitchToMountain()
    {
        m_currentPanel = m_mountainPanel;
        float elapsedTime = 0f;

        Vector2 startingAnchorMin = m_panels.anchorMin;
        Vector2 startingAnchorMax = m_panels.anchorMax;
        Vector2 endAnchorMin = new Vector2(0, 0);
        Vector2 endAnchorMax = new Vector2(2, 1);

        while (elapsedTime < m_switchSeconds)
        {
            m_panels.anchorMin = Vector2.Lerp(startingAnchorMin, endAnchorMin, elapsedTime / m_switchSeconds);
            m_panels.anchorMax = Vector2.Lerp(startingAnchorMax, endAnchorMax, elapsedTime / m_switchSeconds);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        m_panels.anchorMin = endAnchorMin;
        m_panels.anchorMax = endAnchorMax;
    }
    private IEnumerator SwitchToDesert()
    {
        m_currentPanel = m_desertPanel;
        float elapsedTime = 0f;

        Vector2 startingAnchorMin = m_panels.anchorMin;
        Vector2 startingAnchorMax = m_panels.anchorMax;
        Vector2 endAnchorMin = new Vector2(-1, 0);
        Vector2 endAnchorMax = new Vector2(1, 1);

        while (elapsedTime < m_switchSeconds)
        {
            m_panels.anchorMin = Vector2.Lerp(startingAnchorMin, endAnchorMin, elapsedTime / m_switchSeconds);
            m_panels.anchorMax = Vector2.Lerp(startingAnchorMax, endAnchorMax, elapsedTime / m_switchSeconds);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        m_panels.anchorMin = endAnchorMin;
        m_panels.anchorMax = endAnchorMax;
    }
    private void SwitchToMountainImmediately()
    {
        m_panels.anchorMin = new Vector2(0, 0);
        m_panels.anchorMax = new Vector2(2, 1);
        m_currentPanel = m_mountainPanel;

        //m_lastKOLButton.gameObject.SetActive(false);
        //m_nextKOLButton.gameObject.SetActive(true);
    }
    private void SwitchToDesertImmediately()
    {
        Debug.Log("SwitchToDesertImmediately");

        m_panels.anchorMin = new Vector2(-1, 0);
        m_panels.anchorMax = new Vector2(1, 1);
        m_currentPanel = m_desertPanel;

        //m_lastKOLButton.gameObject.SetActive(true);
        //m_nextKOLButton.gameObject.SetActive(false);
    }

    public void GoHome()
    {
        MySound.ButtonSound();
        MySceneManager.LoadScene("Home");
    }

    public void GoDifficulty(Difficulty difficulty)
    {
        if (difficulty.data.continueLastGame || difficulty.data.currentLevel > 1)
        {
            m_continueOrRestartMenu.mainColor = m_currentPanel.mainColor;
            m_continueOrRestartMenu.Enable(this, m_difficultyManager.RestartDifficulty, m_difficultyManager.ContinueDifficulty, difficulty);
        }
        else
        {
            m_difficultyManager.RestartDifficulty(difficulty);
        }
    }
}
