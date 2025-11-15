using System;
using UnityEngine;

public class PlayerMoveState : IPlayerState {
  private const float MOVEMENT_THRESHOLD = .01f;

  private Player player;
  private StateMachine stateMachine;

  public void Enter(Player player, StateMachine stateMachine) {
    this.player = player;
    this.stateMachine = stateMachine;
  }

  public void Exit() {}

  // Probably need some stop moving animation
  // moving left to right results in a brief idle state
  public void Tick() {
    Debug.Log("Player Move State");
    if(Math.Abs(player.moveInput) < MOVEMENT_THRESHOLD) {
      stateMachine.ChangeState(player.IdleState, player);
    }
  }

  public void FixedTick() {
    if (!player.isDashing) {
      // Determine if this is right (infinity?)
      float clampedY = Mathf.Clamp(player.rigidBody.linearVelocity.y, player.maxFallSpeed, Mathf.Infinity);
      player.rigidBody.linearVelocity = new Vector2(player.moveInput * player.moveSpeed, clampedY);
    }
  }
}