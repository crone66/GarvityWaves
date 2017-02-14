using XInputDotNetPure;

public struct PlayerState
{
    public PlayerIndex Index;
    public GamePadState PrevState;
    public bool Ready;
    public int ClassId;

    public PlayerState(PlayerIndex index, GamePadState state)
    {
        Index = index;
        PrevState = state;
        Ready = false;
        ClassId = 0;
    }

    public PlayerState(PlayerState playerState, GamePadState state, bool ready, int classId)
    {
        Index = playerState.Index;
        PrevState = state;
        Ready = ready;
        ClassId = classId;
    }

    public PlayerState(PlayerIndex index, GamePadState state, bool ready, int classId)
    {
        Index = index;
        PrevState = state;
        Ready = ready;
        ClassId = classId;
    }
}