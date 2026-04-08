using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static bool isInitialized = false;
    private static Configuration configuration;
    public static bool firstLaunch = false;
    public static bool updateLaunch = false;

    #region Utils
    private static string ToPersistentPath(string fileName, bool addExtension = false)
    {
        if (addExtension)
            return Path.Combine(Application.persistentDataPath, fileName + configuration.dataExtension);
        else
            return Path.Combine(Application.persistentDataPath, fileName);
    }
    private static string ToDifficultyPath(int index)
    {
        return Path.Combine(Application.persistentDataPath, configuration.difficultyDirectoryName, configuration.difficultyFileName + index + configuration.dataExtension);
    }
    private static int Sum(int[] array, int limit = -1)
    {
        int cpt = 0;
        for (int i = 0; i < array.Length && (limit == -1 || i < limit); i++)
        {
            cpt += array[i];
        }
        return cpt;
    }
    private static string ToLevelPath(int number)
    {
        return Path.Combine(Application.persistentDataPath, configuration.levelDirectoryName, configuration.levelFileName + number + configuration.dataExtension);
    }
    #endregion

    public static void Initialize(Configuration _configuration)
    {
        if (isInitialized) return;

        Debug.Log("Chargement du SaveSystem ...");

        configuration = _configuration;

        if (!File.Exists(ToPersistentPath(configuration.versionFileName)))
        {
            Debug.Log("Création des fichiers de sauvegarde.");
            firstLaunch = true;
            CreateDataFiles();
        }
        else if (GetVersion() != Application.version)
        {
            Debug.Log("Mise à jour des fichiers de sauvegarde.");
            updateLaunch = true;
            UpdateDataFiles();
        }

        VerifySettings();
        VerifyDifficulties();
        VerifyLevels();

        isInitialized = true;

        Debug.Log("Chargement du SaveSystem avec succés.");
    }
    private static void CreateDataFiles()
    {
        using (BinaryWriter bw = new BinaryWriter(File.Open(ToPersistentPath(configuration.versionFileName), FileMode.Create)))
        {
            bw.Write(Application.version);
        }

        // Settings
        using (FileStream settingStream = new FileStream(ToPersistentPath(configuration.settingsFileName), FileMode.Create))
        {
            configuration.settings.Write(settingStream);
        }

        // Difficulties
        if (!Directory.Exists(ToPersistentPath(configuration.difficultyDirectoryName)))
            Directory.CreateDirectory(ToPersistentPath(configuration.difficultyDirectoryName));

        foreach (KindOfLevel kindOfLevel in configuration.difficultyDistribution.Keys)
        {
            foreach(int difficultyIndex in configuration.difficultyDistribution[kindOfLevel])
            {
                CreateDifficulty(new DifficultyData(difficultyIndex));
            }
        }

        // Levels
        if (!Directory.Exists(ToPersistentPath(configuration.levelDirectoryName)))
            Directory.CreateDirectory(ToPersistentPath(configuration.levelDirectoryName));

        for (short number = 1; number <= configuration.levelConfigurations.Count; number++)
        {
            CreateLevel(new LevelData(number));
        }
    }
    private static void UpdateDataFiles()
    {
        Debug.Log("Je mets à jour les fichiers de sauvegarde.");
        CreateDataFiles();
    }

    #region Version
    // Version
    private static string GetVersion()
    {
        try
        {
            using (BinaryReader br = new BinaryReader(File.Open(ToPersistentPath(configuration.versionFileName), FileMode.Open)))
            {
                return br.ReadString();
            }
        }
        catch (System.Exception)
        {
            return "Error";
        }
    }
    #endregion

    #region Settings
    private static Settings settings = null;
    public static void VerifySettings()
    {
        Debug.Log("Vérification des données des paramètres ...");
        try
        {
            using (FileStream settingsStream = new FileStream(ToPersistentPath(configuration.settingsFileName), FileMode.Open))
            {
                Settings settings = new Settings(settingsStream);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Les données des paramètres sont altérées. \n" + e);
            CreateSettings(configuration.settings);
            Debug.Log($"Un nouveau fichier a été créé avec les données d'usine des paramètres.");
        }
    }
    public static void CreateSettings(Settings settings)
    {
        using (FileStream stream = new FileStream(ToPersistentPath(configuration.settingsFileName), FileMode.Create))
        {
            settings.Write(stream);
        }
    }
    public static Settings GetSettings()
    {
        if (!isInitialized) throw new System.Exception("SaveSystem n'est pas initialisé.");

        if (settings == null)
        {
            try
            {
                using (FileStream stream = new FileStream(ToPersistentPath(configuration.settingsFileName), FileMode.Open))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    settings = new Settings(stream);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Un erreur s'est produite lors de la lecture des données des paramètres. \n{e}");
                VerifySettings();
                try
                {
                    using (FileStream stream = new FileStream(ToPersistentPath(configuration.settingsFileName), FileMode.Open))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        settings = new Settings(stream);
                    }
                }
                catch (System.Exception e2)
                {
                    throw new System.Exception($"Impossible de corriger l'erreur produite lors de la lecture des données des paramètres.\n{e2}");
                }
            }
        }

        return settings;
    }
    public static void SaveSettings(Settings settings)
    {
        try
        {
            using (FileStream stream = new FileStream(ToPersistentPath(configuration.settingsFileName), FileMode.Create))
            {
                stream.Seek(0, SeekOrigin.Begin);
                settings.Write(stream);
            }
        }
        catch (System.Exception e)
        {
            // Si le fichier de sauvegarde est altéré, on le remplace avec un nouveau fichier contenant les sauvegardes actuelles
            Debug.LogError($"Un erreur s'est produite lors de l'écriture des données des paramètres. \n" + e);
            CreateSettings(settings);
            Debug.Log($"Un nouveau fichier a été créé avec les données d'usine des paramètres.");
        }

        SaveSystem.settings = settings;
    }
    #endregion

    #region Difficulty
    private static void VerifyDifficulties()
    {
        Debug.Log("Vérification des données des difficultés ...");
        foreach (KindOfLevel kindOfLevel in configuration.difficultyDistribution.Keys)
        {
            foreach (int index in configuration.difficultyDistribution[kindOfLevel])
            {
                try
                {
                    using (FileStream difficultyStream = new FileStream(ToDifficultyPath(index), FileMode.Open))
                    {
                        DifficultyData difficultyData = new DifficultyData(index, difficultyStream);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Les données de la difficulté {index} sont altérées. \n" + e);
                    CreateDifficulty(new DifficultyData(index));
                    Debug.Log($"Un nouveau fichier a été créé avec les données d'usine de la difficulté {index}.");
                }
            }
        }
    }
    private static void CreateDifficulty(DifficultyData data)
    {
        using (FileStream stream = new FileStream(ToDifficultyPath(data.index), FileMode.Create))
        {
            data.Write(stream);
        }
    }

    public static Difficulty GetDifficulty(int difficultyIndex)
    {
        if (!isInitialized) throw new System.Exception("SaveSystem n'est pas initialisé.");

        if (DifficultyTracking.isActive && difficultyIndex == DifficultyTracking.difficulty.data.index)
        {
            return DifficultyTracking.difficulty;
        }
        else
        {
            try
            {
                using (FileStream difficultyStream = new FileStream(ToDifficultyPath(difficultyIndex), FileMode.Open))
                {
                    difficultyStream.Seek(0, SeekOrigin.Begin);
                    return new Difficulty(configuration.GetDifficultyConfiguration(difficultyIndex), new DifficultyData(difficultyIndex, difficultyStream));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Un erreur s'est produite lors de la lecture des données de la difficulté {difficultyIndex} du mode infini. \n{e}");
                VerifyDifficulties();
                try
                {
                    using (FileStream difficultyStream = new FileStream(ToDifficultyPath(difficultyIndex), FileMode.Open))
                    {
                        difficultyStream.Seek(0, SeekOrigin.Begin);
                        return new Difficulty(configuration.GetDifficultyConfiguration(difficultyIndex), new DifficultyData(difficultyIndex, difficultyStream));
                    }
                }
                catch (System.Exception e2)
                {
                    throw new System.Exception($"Impossible de corriger l'erreur produite lors de la lecture des données de la difficulté {difficultyIndex}.\n{e2}");
                }
            }
        }
    }
    public static List<Difficulty> GetDifficulties()
    {
        if (!isInitialized) throw new System.Exception("SaveSystem n'est pas initialisé.");

        List<Difficulty> difficulties = new List<Difficulty>();
        foreach (KindOfLevel kol in configuration.difficultyDistribution.Keys)
        {
            foreach (int difficultyIndex in configuration.difficultyDistribution[kol])
            {
                difficulties.Add(GetDifficulty(difficultyIndex));
            }
        }
        return difficulties;
    }
    public static Dictionary<KindOfLevel, List<int>> GetDifficultyDistribution()
    {
        if (!isInitialized) throw new System.Exception("SaveSystem n'est pas initialisé.");

        return configuration.difficultyDistribution;
    }
    public static void SaveDifficulty(Difficulty difficulty)
    {
        if (!isInitialized) throw new System.Exception("SaveSystem n'est pas initialisé.");

        if (DifficultyTracking.isActive && difficulty.data.index == DifficultyTracking.difficulty.data.index)
        {
            DifficultyTracking.SaveDifficulty(difficulty);
        }
        else
        {
            try
            {
                using (FileStream stream = new FileStream(ToDifficultyPath(difficulty.data.index), FileMode.Open))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    difficulty.data.Write(stream);
                }
            }
            catch (System.Exception e)
            {
                // Si le fichier de sauvegarde est altéré, on le remplace avec un nouveau fichier contenant les sauvegardes actuelles
                Debug.LogError($"Un erreur s'est produite lors de l'écriture des données de la difficulté {difficulty.data.index}. \n" + e);
                CreateDifficulty(difficulty.data);
                Debug.Log($"Un nouveau fichier a été créé avec les données d'usine de la difficulté {difficulty.data.index}.");
            }
        }
    }


    public static class DifficultyTracking
    {
        public static bool isActive = false;
        public static bool resetTracking = false;
        private static FileStream stream = null;

        public static Difficulty difficulty { get; private set; }
        private static bool isWriting;
        private static bool isReading;
        private static BinaryWriter bw = null;
        private static BinaryReader br = null;

        public static void Enable(Difficulty _difficulty)
        {
            if (!isInitialized) throw new System.Exception("SaveSystem n'est pas initialisé.");

            difficulty = _difficulty;

            stream = new FileStream(ToDifficultyPath(difficulty.data.index), FileMode.Open);
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);

            resetTracking = false;
            isActive = true;

            if (!difficulty.data.continueLastGame)
                ResetTracking(); // Suppression de l'enregistrement si on ne continue pas une partie

            difficulty.data.continueLastGame = true; // Si jamais le joueur quitte le jeu en pleine partie
            SaveDifficulty(difficulty); // Au cas où il y a eu des changements
        }
        //private static void VerifyLevel()
        //{
        //    stream.Seek(difficulty.data.recordStartOffset, SeekOrigin.Begin);
            
        //    if (difficulty.data.trackedLevel == difficulty.data.currentLevel)
        //    {
        //        Debug.Log("Je continue un ancien enregistrement : level " + difficulty.data.trackedLevel);
        //    }
        //    else
        //    {
        //        Debug.Log($"La difficulté de l'enregistrement ne correspond pas : {difficulty.data.currentLevel} != {difficulty.data.trackedLevel}");

        //        stream.SetLength(difficulty.data.recordStartOffset); // Supression de l'ancien enregistrement de partie

        //        difficulty.data.trackedLevel = difficulty.data.currentLevel; // Définition du level qui DifficultyTracking traite
        //        SaveDifficulty(difficulty); // Et on re-re-resauvegarde la difficulté !
        //    }
        //}
        private static void ResetTracking()
        {
            stream.SetLength(difficulty.data.recordStartOffset); // Supression de l'ancien enregistrement de partie
        }

        private static void InitializeWriting()
        {
            isReading = false;
            isWriting = true;

            bw.Seek(0, SeekOrigin.End);
        }
        public static void Write(float[] datas)
        {
            if (!isActive) throw new System.Exception("DifficultyTracking n'est pas actif.");
            if (!isWriting) InitializeWriting();

            //Debug.Log($"J'écris (file's length = {stream.Length}, position = {stream.Position}, {difficulty.data.currentLevel}");
            foreach (float data in datas)
            {
                bw.Write(data);
            }
            //Debug.Log($"J'ai fini d'écrire (file's length = {stream.Length}, position = {stream.Position}, {difficulty.data.currentLevel}");
        }

        public static void ResetReading()
        {
            if (!isActive) throw new System.Exception("DifficultyTracking n'est pas actif.");

            isWriting = false;
            isReading = true;

            stream.Seek(difficulty.data.recordStartOffset, SeekOrigin.Begin);
        }
        public static float[] Read()
        {
            if (!isActive) throw new System.Exception("DifficultyTracking n'est pas actif.");
            if (!isReading) ResetReading();

            //Debug.Log("Using DifficultyTracking.Read()");
            //Debug.Log($"Je lis (file's length = {stream.Length}, position = {stream.Position}");
            return new float[7] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle() };
        }

        public static void SaveDifficulty(Difficulty _difficulty)
        {
            if (!isActive) throw new System.Exception("DifficultyTracking n'est pas actif.");

            isWriting = false;
            isReading = false;

            difficulty = _difficulty;

            stream.Seek(0, SeekOrigin.Begin);
            difficulty.data.Write(bw);
        }

        public static void Disable()
        {
            if (!isActive) throw new System.Exception("DifficultyTracking n'est pas actif.");

            isActive = false;

            if (resetTracking)
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(difficulty.data.recordStartOffset);
            }

            br.Dispose();
            bw.Dispose();
            stream.Dispose();
            br = null;
            bw = null;
            stream = null;
        }
    }
    #endregion

    #region Level
    private static void VerifyLevels()
    {
        Debug.Log("Vérification des données des niveaux ...");
        for (short number = 1; number <= configuration.levelConfigurations.Count; number++)
        {
            try
            {
                using (FileStream stream = new FileStream(ToLevelPath(number), FileMode.Open))
                {
                    LevelData levelData = new LevelData(number, stream);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Les données du niveau {number} sont altérées. \n" + e);
                CreateLevel(new LevelData(number));
                Debug.Log($"Un nouveau fichier a été créé avec les données d'usine du niveau {number}.");
            }
        }
    }
    private static void CreateLevel(LevelData data)
    {
        if (!Directory.Exists(ToPersistentPath(configuration.levelFileName)))
            Directory.CreateDirectory(ToPersistentPath(configuration.levelDirectoryName));

        using (FileStream stream = new FileStream(ToLevelPath(data.number), FileMode.Create))
        {
            data.Write(stream);
        }
    }

    public static Level GetLevel(short number)
    {
        if (!isInitialized) throw new System.Exception("SaveSystem n'est pas initialisé.");

        if (LevelTracking.isActive && number == LevelTracking.level.data.number)
        {
            return LevelTracking.level;
        }
        else
        {
            try
            {
                using (FileStream stream = new FileStream(ToLevelPath(number), FileMode.Open))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return new Level(configuration.levelConfigurations[number - 1], new LevelData(number, stream));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Un erreur s'est produite lors de la lecture des données du niveau {number}. \n{e}");
                VerifyLevels();
                try
                {
                    using (FileStream stream = new FileStream(ToLevelPath(number), FileMode.Open))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        return new Level(configuration.levelConfigurations[number - 1], new LevelData(number, stream));
                    }
                }
                catch (System.Exception e2)
                {
                    throw new System.Exception($"Impossible de corriger l'erreur produite lors de la lecture des données du niveau {number}.\n{e2}");
                }
            }
        }
    }
    public static List<Level> GetLevels()
    {
        if (!isInitialized) throw new System.Exception("SaveSystem n'est pas initialisé.");

        List<Level> levels = new List<Level>();
        for (short number = 1; number <= configuration.levelConfigurations.Count; number++)
        {
            levels.Add(GetLevel(number));
            //Debug.Log(levels[number - 1].data.number);
            //Debug.Log(levels[number - 1].data.isUnlocked);
            //Debug.Log(levels[number - 1].data.bestTime);
        }
        return levels;
    }
    public static void SaveLevel(Level level)
    {
        if (!isInitialized) throw new System.Exception("SaveSystem n'est pas initialisé.");

        if (LevelTracking.isActive && level.data.number == LevelTracking.level.data.number)
        {
            LevelTracking.SaveLevel(level);
        }
        else
        {
            try
            {
                using (FileStream stream = new FileStream(ToLevelPath(level.data.number), FileMode.Open))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    level.data.Write(stream);
                }
            }
            catch (System.Exception e)
            {
                // Si le fichier de sauvegarde est altéré, on le remplace avec un nouveau fichier contenant les sauvegardes actuelles
                Debug.LogError($"Un erreur s'est produite lors de la lecture des données du niveau {level.data.number}. \n" + e);
                CreateLevel(level.data);
                Debug.Log($"Un nouveau fichier a été créé avec les données actuelles du niveau {level.data.number}.");
            }
        }
    }

    public static class LevelTracking
    {
        public static bool isActive { get; private set; }
        public static Level level { get; private set; }

        private static FileStream stream;
        private static BinaryWriter writer;

        public static void Enable(Level _level)
        {
            if (!isInitialized) throw new System.Exception("SaveSystem n'est pas initialisé.");
            if (isActive) Disable();

            level = _level;

            stream = new FileStream(ToLevelPath(level.data.number), FileMode.Open);
            writer = new BinaryWriter(stream);

            isActive = true;

            SaveLevel(level); // Au cas où il y a eu des changements
        }

        public static void UpdateLevel(Vector3 lastPlayerPosition, Quaternion lastPlayerRotation, float lastGameTime)
        {
            if (!isActive) throw new System.Exception("DifficultyTracking n'est pas actif.");

            writer.Seek(-20, SeekOrigin.End); // 20 = 3*4(lastPlayerPosition) + 4(lastPlayerRotation) + 4(lastGameTime)
            writer.Write(lastPlayerPosition.x);
            writer.Write(lastPlayerPosition.y);
            writer.Write(lastPlayerPosition.z);
            writer.Write(lastPlayerRotation.eulerAngles.y);
            writer.Write(lastGameTime);
        }

        public static void SaveLevel(Level _level)
        {
            if (!isActive) throw new System.Exception("DifficultyTracking n'est pas actif.");

            level = _level;
            writer.Seek(0, SeekOrigin.Begin);
            level.data.Write(writer);

            //LevelData test = new LevelData(level.data.number, stream);

            //Debug.Log("number : " + test.number);
            //Debug.Log("isUnlocked : " + test.isUnlocked);
            //Debug.Log("isFinished : " + test.isFinish);
            //Debug.Log("attempts : " + test.attemps);
            //Debug.Log("finishedAttempts : " + test.finishedAttempts);
            //Debug.Log("bestTime : " + test.bestTime);

            //using (BinaryReader reader = new BinaryReader(File.Open(ToLevelPath(level.data.number), FileMode.Open)))
            //{
            //    Debug.Log(reader.ReadSingle());
            //    Debug.Log(reader.ReadSingle());
            //    Debug.Log(reader.ReadSingle());
            //}
        }

        public static void Disable()
        {
            if (!isActive) throw new System.Exception("DifficultyTracking n'est pas actif.");

            Debug.Log("Désactivation LevelTracking ...");
            isActive = false;

            writer.Dispose();
            stream.Dispose();
            writer = null;
            stream = null;

            Debug.Log("LevelTracking desactivé avec succés.");
        }
    }
    #endregion

    public static void ChangeScene()
    {
        if (DifficultyTracking.isActive)
            DifficultyTracking.Disable();

        if (LevelTracking.isActive)
            LevelTracking.Disable();
    }
}
