using System.Collections.Generic;
using UnityEngine;

public class InfinityPanel : MonoBehaviour
{
    // Difficulties
    private DifficultyManager m_difficultyManager;
    private InfinityMenu m_menu;
    private List<Difficulty> m_difficulties;
    [SerializeField]
    private KindOfLevel m_kindOfLevel;
    [SerializeField]
    private List<DifficultyButton> m_difficultyButtons;
    [SerializeField]
    private Color m_mainColor;
    public Color mainColor { get => m_mainColor; }


    public void Initialize(DifficultyManager difficultyManager, InfinityMenu menu)
    {
        m_difficultyManager = difficultyManager;
        m_menu = menu;
        m_difficulties = difficultyManager.GetDifficulties();

        if (m_difficulties == null || m_difficulties.Count == 0)
        {
            throw new System.InvalidOperationException("m_difficulties est null");
        }
        if (m_difficulties.Count < m_difficultyButtons.Count)
        {
            throw new System.InvalidOperationException("Il n'y a pas assez de boutons pour les difficultés");
        }

        int i = 0;
        foreach(int index in difficultyManager.GetDifficultyDistribution()[m_kindOfLevel])
        {
            m_difficultyButtons[i].Initialize(m_difficultyManager, menu, m_kindOfLevel, m_difficulties[index]);
            i++;
        }
    }
}
