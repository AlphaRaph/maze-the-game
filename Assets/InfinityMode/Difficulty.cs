using UnityEngine;
using System.IO;


[System.Serializable]
public class Difficulty
{
    public DifficultyConfiguration configuration;
    public DifficultyData data;

    public Difficulty(DifficultyConfiguration configuration, DifficultyData data)
    {
        this.configuration = configuration;
        this.data = data;
    }
}

[System.Serializable]
public class DifficultyConfiguration
{
    public string en_name;
    public string fr_name;

    public IntVector2 startMazeSize;
    public IntVector2 addingMazeSize;
    public float timeCoefficient;
    public float pieceDensity;

    [System.Serializable]
    public class UnlockCondition
    {
        public int difficultyKolIndexNeeded = -1;
        public int levelNeeded = 0;
    }
    public UnlockCondition unlockCondition;

    public Color color;
}

[System.Serializable]
public class DifficultyData
{
    public int index;

    public bool isUnlocked = false;
    public int bestLevel = 0;
    public int currentLevel = 1;
    public bool continueLastGame = false;
    public int lastSeed;
    public int lastPieceAdvancement;
    public long recordStartOffset = 0;

    public DifficultyData(int _index)
    {
        index = _index;
    }
    public DifficultyData(int _index, Stream input)
    {
        index = _index;

        Read(input);
    }

    public void Write(Stream output)
    {
        using (BinaryWriter bw = new BinaryWriter(output))
        {
            Write(bw);
        }
    }
    public void Write(BinaryWriter bw)
    {
        long startingPos = bw.BaseStream.Position;
        bw.Seek(0, SeekOrigin.Current);

        bw.Write((short)index);
        bw.Write((short)bestLevel);
        ///Debug.Log($"bestLevel : {bestLevel} ({index})");
        bw.Write((short)currentLevel);
        bw.Write(continueLastGame);
        bw.Write(lastSeed);
        bw.Write(lastPieceAdvancement);
        recordStartOffset = bw.BaseStream.Position - startingPos;
    }

    public void Read(Stream input)
    {
        using (BinaryReader br = new BinaryReader(input))
        {
            long startingPos = input.Position;

            int readIndex = br.ReadInt16();
            if (readIndex != index) throw new System.Exception($"L'index sauvegardé dans le fichier ne correspond pas à l'index spécifié : {readIndex} != {index}");
            else index = readIndex;

            bestLevel = br.ReadInt16();
            currentLevel = br.ReadInt16();
            continueLastGame = br.ReadBoolean();
            lastSeed = br.ReadInt32();
            lastPieceAdvancement = br.ReadInt32();
            recordStartOffset = input.Position - startingPos;
            //if (continueLastGame)
            //{
            //    while(input.Position < input.Length)
            //    {
            //        lastPlayerPosition = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            //        lastGameTime = br.ReadSingle();
            //        lastRemainingTime = br.ReadSingle();
            //        if (br.ReadSingle() >= 0)
            //        {
            //            lastNumberOfMoves += (input.Position < input.Length - (3 / Time.fixedDeltaTime)) ? 1 : (input.Length - input.Position) * Time.fixedDeltaTime; // 3 = temps que mettent les murs à bouger
            //        }
            //    }
            //}
            //else
            //{
            //    lastPlayerPosition = new Vector3(0, -100, 0);
            //    lastGameTime = 0;
            //    lastRemainingTime = 0;
            //    lastNumberOfMoves = 0;
            //}
        }
    }
}
