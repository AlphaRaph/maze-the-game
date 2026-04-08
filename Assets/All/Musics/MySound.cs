using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MySound : MonoBehaviour
{
    public static MySound instance;

    // References
    [SerializeField]
    private List<AudioClip> m_instruInfinity;
    [SerializeField]
    private List<AudioClip> m_instruStory;
    [SerializeField]
    private AudioSource m_instruSource;
    private bool m_playingInfinity, m_playingStory;
    [SerializeField]
    private AudioSource m_source;
    [SerializeField]
    private AudioClip m_clickClip;
    [SerializeField]
    private AudioClip m_pieceClip;

    public void Awake()
    {
        if (instance != null) throw new System.Exception("Il y a plusieur MySound dans la scène.");

        m_playingInfinity = false;
        m_playingStory = false;
        instance = this;
    }
    public void Update()
    {
        if (!m_instruSource.isPlaying && !AudioListener.pause)
        {
            if (m_playingInfinity)
            {
                NewInfinityClip();
            }
            else if (m_playingStory)
            {
                NewStoryClip();
            }

            // Home
            if (HomeManager.instance != null)
                HomeManager.instance.UpdateThemeWithMusic();
        }
    }

    public void PlayInfinity()
    {
        m_playingInfinity = true;
        m_playingStory = false;
        NewInfinityClip();
    }
    public void PlayStory()
    {
        m_playingInfinity = false;
        m_playingStory = true;
        NewStoryClip();
    }
    
    public void NewInfinityClip()
    {
        m_instruSource.clip = m_instruInfinity[Random.Range(0, m_instruInfinity.Count - 1)];
        m_instruSource.Play();
    }
    public void NewStoryClip()
    {
        m_instruSource.clip = m_instruStory[Random.Range(0, m_instruInfinity.Count - 1)];
        m_instruSource.Play();
    }

    // Sounds
    public static void ButtonSound()
    {
        if (SaveSystem.GetSettings().buttonSound)
        {
            instance.m_source.clip = instance.m_clickClip;
            instance.m_source.Play();
        }
    }
    public static void PieceSound()
    {
        instance.m_source.clip = instance.m_pieceClip;
        instance.m_source.Play();
    }
}
