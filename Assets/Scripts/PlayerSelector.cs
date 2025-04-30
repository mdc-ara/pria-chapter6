using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class PlayerSelector : NetworkBehaviour {
  public static PlayerSelector instance;
  private bool isHostTurn;

  private void Awake() {
    if (instance == null)
      instance = this;
    else
      Destroy(gameObject);
  }

  public bool IsHostTurn() {
    return isHostTurn;
  }
  public override void OnNetworkSpawn() {
    base.OnNetworkSpawn();
    NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
  }

  private void NetworkManager_OnServerStarted() {
    if (!IsServer)
      return;
    GameStateManager.onGameStateChanged += GameStateChangedCallback;
    Ball.onHit += SwitchPlayers;
  }

  private void SwitchPlayers() {
    isHostTurn = !isHostTurn;
    InitializePlayers();
  }

  public override void OnDestroy() {
    base.OnDestroy();
    NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted; ;
    GameStateManager.onGameStateChanged -= GameStateChangedCallback;
    Ball.onHit -= SwitchPlayers;
  }

  private void GameStateChangedCallback(GameStateManager.State gameState) {
    switch (gameState) {
      case GameStateManager.State.Game:
        InitializePlayers();
        break;
    }
  }

  private void InitializePlayers() {
    // Look for every player in the game
    PlayerStateManager[] playerStateManagers = FindObjectsOfType<PlayerStateManager>();
    for (int i = 0; i < playerStateManagers.Length; i++) {
      if (playerStateManagers[i].GetComponent<NetworkObject>().IsOwnedByServer) {
        if (isHostTurn)
          playerStateManagers[i].Enable();  // Is the host and its turn
        else
          playerStateManagers[i].Disable(); // Is the host but not its turn
      } else {
        if (isHostTurn)
          playerStateManagers[i].Disable(); // Is the client but not its turn
        else
          playerStateManagers[i].Enable(); // Is the client and its turn
      }
    }
  }
}
