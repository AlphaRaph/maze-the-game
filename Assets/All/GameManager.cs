using UnityEngine;

public abstract class GameManager : MonoBehaviour
{
    public static GameManager instance { get; set; }

    // References
    [Header("References")]
    protected Player m_player;

    // Attributes
    protected MyMap m_map;
    public MyMap map { get { return m_map; } }
    public float gameTime { get; protected set; }
    protected Mode m_mode;
    public Mode mode { get { return m_mode; } }
    [SerializeField]
    protected KindOfLevel m_kindOfLevel;
    public KindOfLevel kindOfLevel { get { return m_kindOfLevel; } }
    protected bool m_isDying;
    protected bool m_hasEnteredTheMaze;

    protected abstract void InitializeGame();
    public abstract void StartGame();
    public abstract void EnterTheMaze();
    public abstract void PauseTime();
    public abstract void RestartTime();
    protected abstract void StopGame();
    public abstract void FinishGame(Result result);
    public abstract void SwitchToEndMenu(Result result);
    //protected abstract void PlayerDie();
    //protected abstract void PlayerDieEndMenu();
    //public abstract void FinishLevel();
    //protected abstract void GetPerfs();
    //protected abstract void ResetPerfs();
    //protected abstract void SavePerfs();
    protected abstract void SaveResult(Result result);
    public virtual void AddCoinsToPlayer(int quantity = 1)
    {
        if (m_player == null)
            throw new System.Exception("Le joueur est null");

        m_player.AddCoins(quantity);
    }

    public virtual void StopPlayer()
    {
        if (m_player == null)
            throw new System.Exception("Le joueur est null");

        m_player.GetComponent<PlayerController>().CanMove = false;
    }

    public virtual void RestartPlayer()
    {
        if (m_player == null)
            throw new System.Exception("Le joueur est null");

        m_player.controller.CanMove = true;
        m_player.controller.Refresh();
    }
}

public enum Mode
{
    Generator,
    Infinity,
    Story
}

public enum KindOfLevel
{
    None,
    Mountain,
    Desert,
    Volcano
}

public static class KindOfLevels
{
    public static KindOfLevel Get(string str)
    {
        if (str == "Mountain")
        {
            return KindOfLevel.Mountain;
        }
        else if (str == "Desert")
        {
            return KindOfLevel.Desert;
        }
        else if (str == "Volcano")
        {
            return KindOfLevel.Volcano;
        }
        else
        {
            return KindOfLevel.None;
        }
    }

    public static string String(this KindOfLevel kindOfLevel)
    {
        if (kindOfLevel == KindOfLevel.Mountain)
            return "Mountain";
        else if (kindOfLevel == KindOfLevel.Desert)
            return "Desert";
        else if (kindOfLevel == KindOfLevel.Volcano)
            return "Volcano";
        else
            return "";
    }
}

public enum Result
{
    TimeOut,
    FinishLevel,
    Abandon,
    ContinueLater
}
