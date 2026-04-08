using System.Collections.Generic;

[System.Serializable]
public class Configuration
{
    public string dataExtension = ".bin";
    public string versionFileName = "version.bin";

    public string settingsFileName = "settings.bin";
    public Settings settings;

    public string difficultyDirectoryName = "difficulties";
    public string difficultyFileName = "difficulty_";
    public Dictionary<KindOfLevel, List<int>> difficultyDistribution = new Dictionary<KindOfLevel, List<int>>() {
        { KindOfLevel.Mountain, new List<int>() {0, 1, 2} }, 
        { KindOfLevel.Desert, new List<int>() {3, 4, 5 }} 
    };
    public List<DifficultyConfiguration> difficultyConfigurations;
    public DifficultyConfiguration GetDifficultyConfiguration(int difficultyIndex)
    {
        foreach (KindOfLevel kol in difficultyDistribution.Keys)
        {
            int i = 0;
            foreach(int index in difficultyDistribution[kol])
            {
                if (difficultyIndex == index)
                    return difficultyConfigurations[i];
                i++;
            }
        }
        throw new System.IndexOutOfRangeException("difficultyIndex est invalide.");
    }

    public string levelDirectoryName = "levels";
    public string levelFileName = "level_";
    public List<LevelConfiguration> levelConfigurations;
}
