using System.Collections;
using UnityEngine;

public class LevelPassage : MonoBehaviour
{
    private LevelButton m_levelButton;
    private bool m_isUnlocked;

    public void Initialize(LevelButton levelButton, bool isUnlocked)
    {
        m_levelButton = levelButton;
        m_isUnlocked = isUnlocked;
        Refresh();
    }

    public IEnumerator UnlockAnimation()
    {
        m_isUnlocked = true;
        yield return null;
    }

    private void Refresh()
    {

    }
}
