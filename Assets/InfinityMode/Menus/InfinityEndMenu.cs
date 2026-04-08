using UnityEngine;
using UnityEngine.UI;

public class InfinityEndMenu : GameMenu
{
    // References
    [SerializeField]
    private LanguageText m_replayRemainingTimeText;
    [SerializeField]
    private LanguageText m_wellPlayedText;
    [SerializeField]
    private LanguageText m_scoreText;
    [SerializeField]
    private LanguageText m_bestScoreText;
    [SerializeField]
    private LanguageText m_remainingTimeText;
    [SerializeField]
    private Button m_homeButton;
    [SerializeField]
    private LanguageText m_homeText;
    [SerializeField]
    private Button m_nextLevelButton;
    [SerializeField]
    private LanguageText m_nextLevelText;
    [SerializeField]
    private Color m_mountainColor;
    [SerializeField]
    private Color m_desertColor;
    [SerializeField]
    private ConfirmMenu m_confirmMenu;

    // Attributes
    private Result m_result;
    private int m_currentLevel;
    private int m_bestLevel;
    private float m_gameTime = 0f;
    private Color m_currentColor;
    
    public void Enable (Result result, int currentLevel, int bestLevel, float gameTime = 0f, bool playWithTime = true)
    {
        m_result = result;
        m_currentLevel = currentLevel;
        if (m_currentLevel > bestLevel)
            m_bestLevel = m_currentLevel;
        else
            m_bestLevel = bestLevel;
        m_gameTime = gameTime;

        // Set texts
        if (m_result != Result.FinishLevel)
        {
            m_wellPlayedText.Set("Game over", "Partie terminée");
            m_scoreText.Set("Score : Level " + (m_currentLevel - 1), "Score : Niveau " + m_currentLevel);
            m_bestScoreText.Set("Best Score : Level " + m_bestLevel, "Meilleur Score : Niveau " + m_bestLevel);
            m_remainingTimeText.SetAll("");
            m_nextLevelText.Set("Play again", "Rejouer");
        }
        else
        {
            m_wellPlayedText.Set("Well played !", "Bien joué !");
            m_scoreText.Set("Score : Level " + m_currentLevel, "Score : Niveau " + m_currentLevel);
            m_bestScoreText.Set("Best Score : Level " + m_bestLevel, "Meilleur score : Niveau " + m_bestLevel);
            if (playWithTime)
                m_remainingTimeText.Set("Remaining Time : " + (int)(m_gameTime / 60f) + " min " + ((int)m_gameTime - ((int)(m_gameTime / 60f) * 60)) + " s",
                    "Temps restant : " + (int)(m_gameTime / 60f) + " min " + ((int)m_gameTime - ((int)(m_gameTime / 60f) * 60)) + " s");
            else
                m_remainingTimeText.Set("Elapsed time : " + (int)(m_gameTime / 60f) + " min " + ((int)m_gameTime - ((int)(m_gameTime / 60f) * 60)) + " s",
                    "Temps écoulé : " + (int)(m_gameTime / 60f) + " min " + ((int)m_gameTime - ((int)(m_gameTime / 60f) * 60)) + " s");
            m_homeText.Set("Home", "Menu");
            m_nextLevelText.Set("Next level", "Niveau Suivant");
        }

        // Set Color
        if (GameManager.instance.kindOfLevel == KindOfLevel.Mountain)
            m_currentColor = m_mountainColor;
        else
            m_currentColor = m_desertColor;

        m_replayRemainingTimeText.text.color = m_currentColor;
        m_wellPlayedText.text.color = m_currentColor;
        m_scoreText.text.color = m_currentColor;
        m_bestScoreText.text.color = m_currentColor;
        m_remainingTimeText.text.color = m_currentColor;
        ColorBlock colors = m_homeButton.colors;
        colors.normalColor = m_currentColor;
        m_homeButton.colors = colors;
        m_nextLevelButton.colors = colors;

        m_confirmMenu.mainColor = m_currentColor;
        m_confirmMenu.Disable();

        // Show menu
        Enable();
    }

    public void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Escape))
        {
            Home();
        }
    }

    public void UpdateReplayRemainingTime(float seconds)
    {
        m_replayRemainingTimeText.SetAll(string.Format("{0:D2}:{1:D2}", (int)seconds / 60, (int)seconds % 60));
    }

    public void Home ()
    {
        MySound.ButtonSound();
        if (m_result == Result.FinishLevel)
            m_confirmMenu.Enable(this, GoHome);
        else
            GoHome();
    }

    public void GoHome ()
    {
        MySceneManager.LoadScene("Home");
    }

    public void NextLevel()
    {
        MySound.ButtonSound();
        // Save current level
        //if (m_result == Result.PlayerDie)
        //{
        //    GameManager.instance.ResetPerfs();
        //}
        //else
        //{
        //    GameManager.instance.SavePerfs();
        //}

        // Load next level scene
        if (GameManager.instance.kindOfLevel == KindOfLevel.Mountain)
        {
            MySceneManager.LoadScene("MountainInfinity");
        }
        else if (GameManager.instance.kindOfLevel == KindOfLevel.Desert)
        {
            MySceneManager.LoadScene("DesertInfinity");
        }
        else
        {
            throw new System.InvalidOperationException("Le type de niveau 'volcano' n'est pas encore opérationnel");
        }
    }
}
