using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HomeManager : MonoBehaviour
{
    public static HomeManager instance;

    // References 
    [SerializeField]
    private MySound m_soundGameObject;
    [SerializeField]
    private HomeMenu m_homeMenu;
    [SerializeField]
    private Camera m_camera;
    [SerializeField]
    private Transform m_cameraRotationPoint;
    [SerializeField]
    private List<HomeTheme> m_themes;

    // Attributes
    private static bool m_firstHome = true;
    [SerializeField]
    private IntVector2 m_mazeSize;
    private int m_themesIndex;
    public HomeTheme currentTheme { get { return m_themes[m_themesIndex]; } }
    public MapGenerator currentMapGen { get { return currentTheme.mapGen; } }
    [SerializeField]
    private float m_cameraRotateSpeed;
    public bool updateWithMusic { get; set; }
    public bool changeThemes { get; set; }

    public void Start()
    {
        instance = this;

        // Load mountain theme
        MountainTheme();

        // Play music
        if (MySound.instance == null)
        {
            DontDestroyOnLoad(Instantiate(m_soundGameObject).gameObject);
        }
        MySound.instance.PlayInfinity();

        // Show GUI elements
        m_homeMenu.Enable();

        // Privacy policy
        if ((SaveSystem.firstLaunch || SaveSystem.updateLaunch) && m_firstHome)
        {
            m_homeMenu.PrivacyPolicyMenu();
        }

        Time.timeScale = 1;
    }

    private void NextTheme()
    {
        if (m_themesIndex < 0)
            m_themesIndex = 0;
        else
            m_themesIndex++;

        if (m_themesIndex >= m_themes.Count)
            m_themesIndex = 0;
    }

    public void UpdateThemeWithMusic()
    {
        if (updateWithMusic)
        {
            UpdateTheme();
        }
    }

    public void UpdateTheme()
    {
        if (changeThemes)
            NextTheme();

        StartCoroutine(UpdateMap());
    }

    private IEnumerator UpdateMap()
    {
        yield return new WaitForEndOfFrame();

        // Generate new map
        foreach (HomeTheme theme in m_themes)
        {
            theme.mapGen.DeleteMap();
            theme.mapGen.DeleteMap();
        }

        currentMapGen.UpdateMap(new IntVector2(Random.Range(1, 8) * 2 + 1, Random.Range(3, 17)), (int)Random.Range(0, 999999999999), currentMapGen.LevelOfDetail);

        // Update colors
        m_camera.backgroundColor = currentTheme.bgColor;
        m_homeMenu.infinityButton.image.color = currentTheme.fgColor;
        if (m_homeMenu.storyButton != null)
            m_homeMenu.storyButton.image.color = currentTheme.fgColor;

        // Update the title image
        m_homeMenu.title.image.sprite = currentTheme.titleImage;
        m_homeMenu.title.PlayAnimation();
    }

    public void MountainTheme()
    {
        if (changeThemes)
            m_themesIndex = 1;
        else
            m_themesIndex = 0;

        UpdateTheme();
    }

    public void DesertTheme()
    {
        if (changeThemes)
            m_themesIndex = 0;
        else
            m_themesIndex = 1;

        UpdateTheme();
    }

    public void Update()
    {
        m_cameraRotationPoint.Rotate(0, m_cameraRotateSpeed * Time.deltaTime, 0);
    }

    public void OnDisable()
    {
        Debug.Log("La scène est désactivée.");
        instance = null;
        m_firstHome = false;
    }
}

[System.Serializable]
public class HomeTheme
{
    public string name;
    public KindOfLevel kindOfLevel;
    public MapGenerator mapGen;
    public Color bgColor;
    public Color fgColor;
    public Sprite titleImage;
}
