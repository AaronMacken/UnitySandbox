public class StateMachine {
  public IPlayerState CurrentState { get; private set; }

  public void ChangeState(IPlayerState nextPlayerState, Player player) {
    if (nextPlayerState == CurrentState) {
      return;
    }

    CurrentState?.Exit();

    CurrentState = nextPlayerState;
    CurrentState.Enter(player, this);
  }

  public void Tick() {
    CurrentState?.Tick();
  }

  public void FixedTick() {
    CurrentState?.FixedTick();
  }
}