using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class GameStateManager : NetworkBehaviour {
  // Singleton
  public static GameStateManager instance;
  public enum State { Menu, Game, Win, Lose }
  [SerializeField] private State gameState; // Actual state

  private int connectedPlayers;

  // Event that notifies changes in game state
  [Header("Events")]
  public static Action<State> onGameStateChanged;

  public void Awake() {
    // Singleton configuration
    if (instance == null)
      instance = this;
    else
      Destroy(gameObject);
  }

  public override void OnNetworkSpawn() {
    base.OnNetworkSpawn();
    // Event subscription
    NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
  }

  private void NetworkManager_OnServerStarted() {
    if (!IsServer)
      return;

    connectedPlayers++; 
    // Event subscription to new clients
    NetworkManager.Singleton.OnClientConnectedCallback +=
     Singleton_OnClientConnectedCallback;
  }

  private void Singleton_OnClientConnectedCallback(ulong obj) {
    connectedPlayers++;
    if (connectedPlayers >= 2)
      StartGame(); // We can start playing
  }

  private void StartGame() {
    StartGameClientRpc();
  }

  [ClientRpc]
  private void StartGameClientRpc() {
    gameState = State.Game;
    // Event launcherd
    onGameStateChanged?.Invoke(gameState);
  }

  void Start() {
    // Show menu before all
    gameState = State.Menu;
  }
  public override void OnDestroy() {
    base.OnDestroy();
    NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
    NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
  }
  public void SetGameState(State st) {
    gameState = st;
    onGameStateChanged?.Invoke(st);
  }
}

