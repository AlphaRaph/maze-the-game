using UnityEngine;

public class ConfigurationInspector : MonoBehaviour
{
    [SerializeField]
    private Configuration m_configuration;
    public Configuration configuration { get => m_configuration; set => m_configuration = value; }
    private void Awake()
    {
        SaveSystem.Initialize(m_configuration);
    }

    //private void OnValidate()
    //{
    //    foreach (LevelConfiguration levelConfiguration in m_configuration.levelConfigurations)
    //    {
    //        levelConfiguration.secondStarTime = levelConfiguration.thirdStarTime * 2;
    //        levelConfiguration.firstStarTime = levelConfiguration.thirdStarTime * 3;
    //    }
    //}
}