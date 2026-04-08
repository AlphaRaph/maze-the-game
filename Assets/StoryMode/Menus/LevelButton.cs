using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButton : MonoBehaviour
{
    // Attributes
    private LevelManager m_levelsManager;
    private Level m_level;
    private bool m_isUnlocked;

    // Components
    [SerializeField] private Text m_txtLevel;
    [SerializeField] private Image m_lockedImage;
    [SerializeField] private LevelPassage m_levelPassage;
    [SerializeField] private Animation m_animation;
    public RectTransform rectTranform { get => GetComponent<RectTransform>(); }

    public void Initialize(LevelManager levelManager, Level level, bool isUnlocked, bool unlockAnimation)
    {
        m_levelsManager = levelManager;
        m_level = level;
        m_isUnlocked = isUnlocked && !unlockAnimation;

        m_levelPassage.Initialize(this, m_isUnlocked && !unlockAnimation);

        Refresh();
        if (unlockAnimation) StartCoroutine(UnlockAnimation());
    }

    public void Click()
    {
        MySound.ButtonSound();
        m_levelsManager.ShowLevelMenu(m_level);
    }

    private void Refresh()
    {
        m_txtLevel.text = "" + m_level.data.number;
        m_lockedImage.enabled = !m_isUnlocked;
    }
    private IEnumerator UnlockAnimation()
    {
        yield return StartCoroutine(m_levelPassage.UnlockAnimation());

        m_isUnlocked = true;
        m_lockedImage.enabled = false;
    }

    public void PlayAnimation()
    {
        m_animation.Play();
    }
}
