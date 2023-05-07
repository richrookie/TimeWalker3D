public class Define
{
    public enum eGameState : byte
    {
        Ready,
        Play,
        End,
    }

    public enum eJoyStickMethod : byte
    {
        DoNotUse,
        Fixed,
        Follow,
    }

    public enum eAnimState : byte
    {
        Idle,
        Run,
        Timeback,
        Death
    }

    public enum eSound : byte
    {
        Bgm,
        TimeBack,
        TickTock,
        MaxCount
    }
}
