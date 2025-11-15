using Unity.VisualScripting;

public interface IPlayerState {
  void Enter(Player player, StateMachine stateMachine);
  void Exit();
  void Tick();
  void FixedTick();
}