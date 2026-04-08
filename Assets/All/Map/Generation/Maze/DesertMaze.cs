

public class DesertMaze : MyMaze
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
        base.Generate(seed, numberOfPossibilities, pieceDensity, pieceAdvancement);
    }
}
