

public class MountainMaze : MyMaze
{
    public override void Initialize(MyMap map, IntVector2 mazeSize)
    {
        if (m_isInitialized)
            return;

        base.Initialize(map, mazeSize);

        VerifyAttributes();
        m_isInitialized = true;
    }

    protected override void VerifyAttributes()
    {
        base.VerifyAttributes();
    }

    public override void Generate(int seed, int numberOfPossibilities, float pieceDensity, int pieceAdvancement)
    {
        if (!m_isInitialized)
            throw new System.Exception(this + " n'est pas initialisé.");

        base.Generate(seed, numberOfPossibilities, pieceDensity, pieceAdvancement);
    }
}
