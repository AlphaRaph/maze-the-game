using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static KindOfLevel currentKindOfLevel { get; private set; }
    public static int currentDifficultyIndex { get; private set; }

    private List<Difficulty> m_difficulties;
    public Dictionary<KindOfLevel, List<int>> m_difficultyDistribution;
    [SerializeField] private InfinityMenu menu;

    private void Start()
    {
        m_difficultyDistribution = SaveSystem.GetDifficultyDistribution();
        InitializeDifficulties();
        menu.Enable(this);
    }
    public void InitializeDifficulties()
    {
        m_difficulties = SaveSystem.GetDifficulties();
        foreach (KindOfLevel kol in m_difficultyDistribution.Keys)
        {
            foreach (int index in m_difficultyDistribution[kol])
            {
                Difficulty difficulty = m_difficulties[index];
                if (difficulty.configuration.unlockCondition.difficultyKolIndexNeeded > -1)
                {
                    if (difficulty.data.isUnlocked !=
                        GetDifficultyWithKolIndex(kol, difficulty.configuration.unlockCondition.difficultyKolIndexNeeded).data.bestLevel
                        >= difficulty.configuration.unlockCondition.levelNeeded)
                    {
                        difficulty.data.isUnlocked = !difficulty.data.isUnlocked;
                        SaveSystem.SaveDifficulty(difficulty);
                        Debug.Log("Je débloque ou bloque une difficulté !");
                    }
                }
                else
                {
                    if (difficulty.data.isUnlocked != true)
                    {
                        difficulty.data.isUnlocked = true;
                        SaveSystem.SaveDifficulty(difficulty);
                    }
                }
            }
        }
    }
    public Difficulty GetDifficultyWithKolIndex(KindOfLevel kindOfLevel, int kolIndex)
    {
        int indexCpt = 0;
        foreach (KindOfLevel kol in m_difficultyDistribution.Keys)
        {
            if (kol == kindOfLevel)
            {
                return m_difficulties[indexCpt + kolIndex];
            }
            else
            {
                indexCpt += m_difficultyDistribution[kol].Count;
            }
        }
        throw new System.Exception("m_difficultyDistribution ne contient pas tous les KindOfLevel.");
    }
    public List<Difficulty> GetDifficulties()
    {
        return m_difficulties;
    }
    public Dictionary<KindOfLevel, List<int>> GetDifficultyDistribution()
    {
        return m_difficultyDistribution;
    }

    private void GoDifficulty(Difficulty difficulty)
    {
        if (!difficulty.data.isUnlocked) return;

        currentDifficultyIndex = difficulty.data.index;

        foreach (KindOfLevel kindOfLevel in m_difficultyDistribution.Keys)
        {
            if (m_difficultyDistribution[kindOfLevel].Contains(difficulty.data.index))
            {
                currentKindOfLevel = kindOfLevel;
                Debug.Log("kindOfLevel  : " + kindOfLevel);
                if (kindOfLevel == KindOfLevel.Mountain)
                    MySceneManager.LoadScene("MountainInfinity");
                else
                    MySceneManager.LoadScene("DesertInfinity");
            }
        }
    }
    public void ContinueDifficulty(Difficulty difficulty)
    {
        GoDifficulty(difficulty);
    }

    public void RestartDifficulty(Difficulty difficulty)
    {
        if (difficulty.data.continueLastGame || difficulty.data.currentLevel > 1)
        {
            difficulty.data.currentLevel = 1;
            difficulty.data.continueLastGame = false;
            SaveSystem.SaveDifficulty(difficulty);
        }

        GoDifficulty(difficulty);
    }
}
