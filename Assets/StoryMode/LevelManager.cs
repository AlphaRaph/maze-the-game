using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static short currentLevel { get; private set; }

    private class StoryResult
    {
        public bool isNull = false;
        public short number;
        public Result result;
        public float time;
        public float lastBestTime;

        public StoryResult()
        {
            isNull = true;
        }
        public StoryResult(short number, Result result, float time, float lastBestTime)
        {
            this.number = number;
            this.result = result;
            this.time = time;
            this.lastBestTime = lastBestTime;
            isNull = false;
        }
    }
    private static StoryResult result = new StoryResult();
    public static void SetResult(short number, Result _result, float time, float lastBestTime) {
        result = new StoryResult(number, _result, time, lastBestTime);
    }

    private List<Level> m_levels;
    [SerializeField]
    private List<LevelButton> m_levelButtons;
    [SerializeField]
    private LevelMenu m_levelMenu;
    [SerializeField]
    private RectTransform m_map;
    [SerializeField]
    private RectTransform m_content;
    [SerializeField]
    private LanguageText m_txtCountStars;
    private const float MOVING_MAP_SECONDS = 0.5f;
    private int lastUnlockedLevel;

    public int CountStars()
    {
        int stars = 0;
        foreach (Level level in m_levels)
        {
            if (level.data.isFinish && level.data.bestTime <= level.configuration.firstStarTime) stars++;
            if (level.data.isFinish && level.data.bestTime <= level.configuration.secondStarTime) stars++;
            if (level.data.isFinish && level.data.bestTime <= level.configuration.thirdStarTime) stars++;
        }

        return stars;
    }
    public void Start()
    {
        Debug.Log("Mise en place des menus des niveaux ...");
        m_levels = SaveSystem.GetLevels();
        InitializeLevels();

        if (!result.isNull)
        {
            Level level = m_levels[result.number - 1];

            if (result.result == Result.FinishLevel)
            {
                m_levelMenu.Enable(this, level, result.time, new bool[3] { 
                    (result.lastBestTime == 0 || (result.lastBestTime > level.configuration.firstStarTime && 
                        result.time < result.lastBestTime)) && result.time < level.configuration.firstStarTime,
                    (result.lastBestTime == 0 || (result.lastBestTime > level.configuration.firstStarTime && 
                        result.time < result.lastBestTime)) && result.time < level.configuration.secondStarTime,
                    (result.lastBestTime == 0 || (result.lastBestTime > level.configuration.firstStarTime && 
                        result.time < result.lastBestTime)) && result.time < level.configuration.thirdStarTime
                });
                StartCoroutine(MoveMap(level, 0));
            }
            else
            {
                m_levelMenu.Enable(this, level);
            }
        }
        else
        {
            m_levelMenu.Disable();
        }
        m_levelButtons[lastUnlockedLevel - 1].PlayAnimation();

        currentLevel = -1;
        result = new StoryResult();

        m_txtCountStars.SetAll(CountStars().ToString());

        Debug.Log("Menus des niveux mis en place avec succés.");
    }
    public void InitializeLevels()
    {
        // On vérifie qu'il y a assez de boutons
        if (m_levelButtons.Count < m_levels.Count)
            throw new System.InvalidOperationException("Il n'y a pas assez de boutons de niveaux.");

        bool areUnlocked = true;
        for (int i = 0; i < m_levels.Count; i++)
        {
            Level level = m_levels[i];
            LevelButton levelButton = m_levelButtons[i]; if (levelButton == null) throw new System.InvalidOperationException("Le " + (i + 1) + "eme bouton est null.");
            bool unlockAnimation = false;

            // On vérifie si le niveau sont biens débloqué pour éviter la triche
            if (level.data.isUnlocked)
            {
                if (!areUnlocked)
                {
                    Debug.LogError("Le niveau devrait être vérouillé");
                    level.data.isUnlocked = false;
                }
                else
                {
                    lastUnlockedLevel = level.data.number;
                }
            }
            // On vérifie si le niveau ne vient pas d'être débloqué
            else
            {
                if (areUnlocked)
                {
                    if (i == 0 || (m_levels[i - 1].data.isFinish && CountStars() >= level.configuration.requiredStars))
                    {
                        level.data.isUnlocked = true;
                        lastUnlockedLevel = level.data.number;

                        if (currentLevel > 0 && currentLevel == i)
                            unlockAnimation = true;
                    }
                }
            }

            // On actualise le bouton en fonction
            levelButton.Initialize(this, level, level.data.isUnlocked, unlockAnimation);
            areUnlocked = areUnlocked && level.data.isUnlocked;
        }
    }

    public void ShowLevelMenu(Level level)
    {
        m_levelMenu.Enable(this, level);
        StartCoroutine(MoveMap(level, MOVING_MAP_SECONDS));
    }
    public IEnumerator MoveMap(Level level, float movingSeconds)
    {
        float xContentPosition = (Screen.width / 2) - (m_map.anchoredPosition.x + m_levelButtons[level.data.number - 1].rectTranform.anchoredPosition.x);
        Debug.Log("xMapPosition : " + xContentPosition);
        float max = 0, min = -(m_content.sizeDelta.x - Screen.width);
        if (xContentPosition > max)
        {
            xContentPosition = 0;
        }
        else if (xContentPosition < min)
        {
            xContentPosition = min;
        }

        float elapsedTime = 0f;
        Vector2 startPosition = m_content.anchoredPosition;
        Vector2 endPosition = new Vector2(xContentPosition, m_content.anchoredPosition.y);
        while (elapsedTime < movingSeconds)
        {
            m_content.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / movingSeconds);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        m_content.anchoredPosition = endPosition;
    }

    public void ContinueAndGoLevel(Level level)
    {
        if (!level.data.isUnlocked) return;
        if (CountStars() < level.configuration.requiredStars) return;

        currentLevel = level.data.number;
        MySceneManager.LoadScene("MountainStory");
    }
    public void RestartAndGoLevel(Level level)
    {
        if (!level.data.isUnlocked) return;
        if (CountStars() < level.configuration.requiredStars) return;

        level.data.ResetTrackingAttributes();
        SaveSystem.SaveLevel(level);

        currentLevel = level.data.number;
        MySceneManager.LoadScene("MountainStory");
    }
    public void ShowNextLevelMenu(Level level)
    {
        ShowLevelMenu(m_levels[level.data.number]);
    } 
}
