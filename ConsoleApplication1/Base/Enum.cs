namespace DataAnalyst.Base
{
    public enum Period
    {
        Day = 0,
        Week = 1,
        Month = 2
    }

    public enum CrossDirection
    {
        Up = 0,
        Down = 1,
        Same = 2
    }

    public enum Direction
    {
        Up = 0,
        Down = 1,
        Unknown = 2
    }

    public enum Exchange
    {
        SZ = 0,
        SH = 1
    }

    public enum DateNotFound
    {
        None,
        MovingForward,
        MovingBackward
    }

    public enum EventType
    {
        UpCross,
        DownCross,
        TopPole,
        BottomPole
    }
}
