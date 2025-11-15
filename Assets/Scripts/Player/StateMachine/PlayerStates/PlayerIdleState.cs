using System;
using UnityEngine;

public class PlayerIdleState : IPlayerState {
  private const float MOVEMENT_THRESHOLD = .01f;

  private Player player;
  private StateMachine stateMachine;

  public void Enter(Player player, StateMachine stateMachine) {
    this.player = player;
    this.stateMachine = stateMachine;
  }

  public void Exit() {}

  public void Tick() {
    Debug.Log("Player Idle State");
    if(Math.Abs(player.moveInput) > MOVEMENT_THRESHOLD) {
      stateMachine.ChangeState(player.MoveState, player);
    }
  }

  public void FixedTick() {}
}