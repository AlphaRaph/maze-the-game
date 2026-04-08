using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : GameMenu
{
    [SerializeField] private LanguageText m_titleLevel;
    [SerializeField] private LanguageText m_txtMazeSize;
    [SerializeField] private Image m_star1;
    [SerializeField] private LanguageText m_txtStar1Time;
    [SerializeField] private Image m_star2;
    [SerializeField] private LanguageText m_txtStar2Time;
    [SerializeField] private Image m_star3;
    [SerializeField] private LanguageText m_txtStar3Time;
    [SerializeField] private LanguageText m_txtLocked;
    [SerializeField] private LanguageText m_txtEndOfTheStoryMode;
    [SerializeField] private LanguageText m_txtTime;
    [SerializeField] private LanguageText m_txtBestTime;
    [SerializeField] private RectTransform m_txtBestTimeTransform;
    [SerializeField] private Vector2 m_bestTimePositionMiddle;
    [SerializeField] private Vector2 m_bestTimePositionRight;

    private LevelManager m_levelsManager;
    private Level m_level;

    public void Enable(LevelManager levelsManager, Level level, float time = 0, bool[] newStars = null)
    {
        Debug.Log($"Ouverture du menu du niveau {level.data.number}.");
        m_levelsManager = levelsManager;
        m_level = level;

        m_titleLevel.Set(m_level.configuration.en_name, m_level.configuration.fr_name);
        m_txtMazeSize.Set($"Maze size : {level.configuration.mazeSize.x}x{level.configuration.mazeSize.y}",
            $"Taille du labyrinthe : {level.configuration.mazeSize.x}x{level.configuration.mazeSize.y}");

        // Stars
        m_txtStar1Time.SetAll(ToStringTime(level.configuration.firstStarTime));
        m_star1.enabled = m_level.data.isFinish && m_level.configuration.firstStarTime >= m_level.data.bestTime && (newStars == null || !newStars[0]);
        m_txtStar2Time.SetAll(ToStringTime(level.configuration.secondStarTime));
        m_star2.enabled = m_level.data.isFinish && m_level.configuration.secondStarTime >= m_level.data.bestTime && (newStars == null || !newStars[1]);
        m_txtStar3Time.SetAll(ToStringTime(level.configuration.thirdStarTime));
        m_star3.enabled = m_level.data.isFinish && m_level.configuration.thirdStarTime >= m_level.data.bestTime && (newStars == null || !newStars[2]);

        // Texts
        if (m_level.data.isFinish)
        {
            if (time != 0)
            {
                m_txtTime.gameObject.SetActive(true);
                m_txtTime.Set($"Time : {ToStringTime(time)}", $"Temps : {ToStringTime(time)}");
                m_txtBestTimeTransform.anchoredPosition = m_bestTimePositionRight;
            }
            else
            {
                m_txtTime.gameObject.SetActive(false);
                m_txtBestTimeTransform.anchoredPosition = m_bestTimePositionMiddle;
            }
            m_txtBestTime.Set($"Best time : {ToStringTime(m_level.data.bestTime)}", $"Meilleur temps : {ToStringTime(m_level.data.bestTime)}");
        }
        else
        {
            m_txtTime.gameObject.SetActive(false);
            m_txtTime.SetAll("");
            m_txtBestTime.SetAll("");
        }

        // Buttons
        if (time != 0) // Si le joueur vient de finir le niveau
        {
            m_buttons[0].gameObject.SetActive(true); // Bouton restart
            m_buttons[1].gameObject.SetActive(false); // Bouton continue
            m_buttons[2].gameObject.SetActive(false); // Bouton play
            m_buttons[3].gameObject.SetActive(level.data.number != 10); // Bouton next level
            m_txtLocked.SetAll("");
            m_txtEndOfTheStoryMode.gameObject.SetActive(level.data.number == 15);
        }
        else if (!level.data.isUnlocked) // Si le niveau n'est pas encore débloqué
        {
            m_buttons[0].gameObject.SetActive(false); // Bouton restart
            m_buttons[1].gameObject.SetActive(false); // Bouton continue
            m_buttons[2].gameObject.SetActive(false); // Bouton play
            m_buttons[3].gameObject.SetActive(false); // Bouton next level
            m_txtLocked.gameObject.SetActive(true);
            if (m_levelsManager.CountStars() < level.configuration.requiredStars)
            {
                m_txtLocked.Set($"You have to finish the previous level to play and get {level.configuration.requiredStars}" +
                    $"({m_levelsManager.CountStars()}/{m_level.configuration.requiredStars}) to play at this level.",
                    $"Vous devez finir le niveau précédent et obtenir {level.configuration.requiredStars} étoiles " +
                    $"({m_levelsManager.CountStars()}/{m_level.configuration.requiredStars}) pour jouer à ce niveau.");
            }
            else
            {
                m_txtLocked.Set("You have to finish the previous level to play at this one.",
                    "Vous devez finir le niveau précédent pour jouer à celui-ci.");
            }

            m_txtEndOfTheStoryMode.gameObject.SetActive(false);
        }
        else if (m_level.data.lastPlayerPosition.y > -1) // Si la partie n'est pas terminée
        {
            m_buttons[0].gameObject.SetActive(true); // Bouton restart
            m_buttons[1].gameObject.SetActive(true); // Bouton continue
            m_buttons[2].gameObject.SetActive(false); // Bouton play
            m_buttons[3].gameObject.SetActive(false); // Bouton next level
            m_txtLocked.gameObject.SetActive(false);
            m_txtEndOfTheStoryMode.gameObject.SetActive(false);
        }
        else // Si aucune partie n'est en cours dans ce niveau
        {
            m_buttons[0].gameObject.SetActive(false); // Bouton restart
            m_buttons[1].gameObject.SetActive(false); // Bouton continue
            m_buttons[2].gameObject.SetActive(true); // Bouton play
            m_buttons[3].gameObject.SetActive(false); // Bouton next level
            m_txtLocked.gameObject.SetActive(false);
            m_txtEndOfTheStoryMode.gameObject.SetActive(false);
        }

        base.Enable();

        // Stars animation
        if (newStars != null) StartCoroutine(UnlockStarsAnimation(newStars));
    }

    private IEnumerator UnlockStarsAnimation(bool[] newStars)
    {
        if (!m_level.data.isFinish) throw new System.InvalidOperationException("Je suis appelé alor que le niveau n'est pas terminé.");

        Debug.Log("Animation des étoiles débloquées ...");

        if (newStars[0])
        {
            Debug.Log("Première étoile !");
            yield return new WaitForSeconds(0.5f);
            m_star1.enabled = true;
        }

        if (newStars[1])
        {
            Debug.Log("Deuxièmre étoile !");
            yield return new WaitForSeconds(0.5f);
            m_star2.enabled = true;
        }

        if (newStars[2])
        {
            Debug.Log("Troisième étoile !");
            yield return new WaitForSeconds(0.5f);
            m_star3.enabled = true;
        }
    }

    private string ToStringTime(float time)
    {
        return string.Format("{0}min {1:D2}s", (int)time / 60, (int)time % 60);
    }

    public void Continue()
    {
        MySound.ButtonSound();
        m_levelsManager.ContinueAndGoLevel(m_level);
    }

    public void Restart()
    {
        MySound.ButtonSound();
        m_levelsManager.RestartAndGoLevel(m_level);
    }

    public void NextLevel()
    {
        MySound.ButtonSound();
        Disable();
        m_levelsManager.ShowNextLevelMenu(m_level);
    }

    public override void Disable()
    {
        if (m_level != null)
            Debug.Log($"Fermeture du menu du niveau {m_level.data.number}.");
        base.Disable();
    }
}
