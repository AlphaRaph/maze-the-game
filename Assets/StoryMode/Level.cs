using System.IO;
using UnityEngine;

[System.Serializable]
public class Level
{
    public LevelConfiguration configuration;
    public LevelData data;

    public Level(LevelConfiguration _confiuration, LevelData _data)
    {
        configuration = _confiuration;
        data = _data;
    }
}

[System.Serializable]
public class LevelConfiguration
{
    public string en_name;
    public string fr_name;
    public int number;
    public KindOfLevel kindOfLevel;

    public int seed;
    public IntVector2 mazeSize;
    public int pieceAdvancement;
    public int maxTime;
    public int firstStarTime;
    public int secondStarTime;
    public int thirdStarTime;
    public int requiredStars;
}

[System.Serializable]
public class LevelData
{
    public short number;
    public bool isUnlocked = false;
    public bool isFinish = false;
    public int attemps = 0;
    public int finishedAttempts = 0;
    public float bestTime = 0;

    public bool continueLastGame = false;
    public Vector3 lastPlayerPosition = new Vector3(0, -100, 0);
    public Quaternion lastPlayerRotation = Quaternion.identity;
    public float lastGameTime = 0;

    public LevelData(short _number)
    {
        number = _number;
    }
    public LevelData(short _number, Stream input)
    {
        number = _number;

        Read(input);
    }

    public void Write(Stream output)
    {
        using (BinaryWriter writer = new BinaryWriter(output))
        {
            Write(writer);
        }
    }
    public void Write(BinaryWriter writer)
    {
        writer.Seek(0, SeekOrigin.Current);
        // On écrit le numéro du niveau qu'on sauvegarde
        writer.Write(number);

        // On écrit les statistiques du niveau
        writer.Write(isUnlocked);
        writer.Write(isFinish);
        writer.Write(attemps);
        writer.Write(finishedAttempts);
        writer.Write(bestTime);

        // On écrit les données concernant la partie actuelle
        writer.Write(continueLastGame);
        writer.Write(lastPlayerPosition.x);
        writer.Write(lastPlayerPosition.y);
        writer.Write(lastPlayerPosition.z);
        writer.Write(lastPlayerRotation.eulerAngles.y);
        writer.Write(lastGameTime);
    }

    public void Read(Stream input)
    {
        using (BinaryReader reader = new BinaryReader(input))
        {
            Read(reader);
        }
    }
    public void Read(BinaryReader reader)
    {
        // On vérifie si on lie les données du bon level
        if (reader.ReadInt16() != number)
            throw new System.Exception($"Le fichier lu ne contient pas les données de ce niveau {number}.");

        // On récupère les statistiques du niveau sauvegardées
        isUnlocked = reader.ReadBoolean();
        isFinish = reader.ReadBoolean();
        attemps = reader.ReadInt32();
        finishedAttempts = reader.ReadInt32();
        bestTime = reader.ReadSingle();

        // On récupère les données concernant la dernière partie jouée
        continueLastGame = reader.ReadBoolean();
        lastPlayerPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        lastPlayerRotation = Quaternion.Euler(0, reader.ReadSingle(), 0);
        lastGameTime = reader.ReadSingle();
    }

    public void ResetTrackingAttributes()
    {
        continueLastGame = false;
        lastPlayerPosition = new Vector3(0, -100, 0);
        lastGameTime = 0;
    }
}
