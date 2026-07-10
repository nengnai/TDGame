
public interface IState
{
    void Enter(AI owner, IState LastState);
    void Update(AI owner);
    void Exit(AI owner);
}

