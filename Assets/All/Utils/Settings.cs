using System.IO;

[System.Serializable]
public class Settings
{
    public float sensitivityX = 25, sensitivityY = 25;
    public bool showFPS = false;
    public float volume = 0.5f;
    public bool isMuted = false;
    public bool vibration = true;
    public bool buttonSound = true;
    public Language language = Language.French;

    public Settings(Stream input)
    {
        Read(input);
    }

    public void Write(Stream output)
    {
        using (BinaryWriter bw = new BinaryWriter(output))
        {
            bw.Write(sensitivityX);
            bw.Write(sensitivityY);
            bw.Write(showFPS);
            bw.Write(volume);
            bw.Write(isMuted);
            bw.Write(vibration);
            bw.Write(buttonSound);
            bw.Write((byte)language);
        }
    }

    private void Read(Stream input)
    {
        using (BinaryReader br = new BinaryReader(input))
        {
            sensitivityX = br.ReadSingle();
            sensitivityY = br.ReadSingle();
            showFPS = br.ReadBoolean();
            volume = br.ReadSingle();
            isMuted = br.ReadBoolean();
            vibration = br.ReadBoolean();
            buttonSound = br.ReadBoolean();
            language = (Language)br.ReadByte();
        }
    }

    public static bool operator==(Settings a, Settings b)
    {
        return ReferenceEquals(a, b) || 
            (!ReferenceEquals(a, null) && 
            !ReferenceEquals(b, null) &&
            a.sensitivityX == b.sensitivityX &&
            a.sensitivityY == b.sensitivityY &&
            a.showFPS == b.showFPS &&
            a.vibration == b.vibration &&
            a.buttonSound == b.buttonSound &&
            a.volume == b.volume);          
    }
    public static bool operator !=(Settings a, Settings b)
    {
        return !(a == b);
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString()
    {
        return base.ToString();
    }
}
