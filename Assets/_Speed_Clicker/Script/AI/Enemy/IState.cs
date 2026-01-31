public interface IState
{
    void OnEnter();
    void Execute();
    void OnExit();
}