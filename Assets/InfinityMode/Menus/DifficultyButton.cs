using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    private DifficultyManager m_difficultyManager;
    private InfinityMenu m_menu;
    private KindOfLevel m_kindOfLevel;
    private Difficulty m_difficulty;

    [SerializeField] private LanguageText m_txtDifficulty;
    [SerializeField] private LanguageText m_txtBestScore;
    [SerializeField] private Image m_lockedImage;
    [SerializeField] private Color m_lockedColor;

    public void Initialize(DifficultyManager difficultyManager, InfinityMenu menu, KindOfLevel kindOfLevel, Difficulty difficulty)
    {
        m_difficultyManager = difficultyManager;
        m_menu = menu;
        m_kindOfLevel = kindOfLevel;
        m_difficulty = difficulty;

        m_txtDifficulty.Set(m_difficulty.configuration.en_name, m_difficulty.configuration.fr_name);
        if (difficulty.data.isUnlocked)
        {
            m_txtBestScore.Set("Best Score : Level " + difficulty.data.bestLevel,
                "Meilleur score : Niveau " + difficulty.data.bestLevel);
            m_lockedImage.gameObject.SetActive(false);
            m_txtBestScore.text.color = Color.black;
        }
        else
        {
            m_lockedImage.gameObject.SetActive(true);
            m_txtBestScore.Set(string.Format("Complete the level {0} of the difficulty {1} to unlock this difficulty.",
                difficulty.configuration.unlockCondition.levelNeeded,
                difficultyManager.GetDifficultyWithKolIndex(m_kindOfLevel, difficulty.configuration.unlockCondition.difficultyKolIndexNeeded).configuration.en_name),
                    string.Format("Terminez le niveau {0} de la difficulté {1} pour débloquer cette difficulté.", 
                difficulty.configuration.unlockCondition.levelNeeded,
                difficultyManager.GetDifficultyWithKolIndex(m_kindOfLevel, difficulty.configuration.unlockCondition.difficultyKolIndexNeeded).configuration.fr_name));
            m_txtBestScore.text.color = m_lockedColor;
        }
    }

    public void OnClick()
    {
        if (m_difficulty.data.isUnlocked)
        {
            MySound.ButtonSound();
            m_menu.GoDifficulty(m_difficulty);
        }
    }
}
