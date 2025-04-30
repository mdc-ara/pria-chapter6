using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using TMPro;

public class ScoreManager : NetworkBehaviour {
  [Header(" Elements ")]
  private int hostScore;
  private int clientScore;
  [SerializeField] private TextMeshProUGUI scoreText;

  public override void OnNetworkSpawn() {
    base.OnNetworkSpawn();
    NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
  }

  private void NetworkManager_OnServerStarted() {
    if (!IsServer)
      return;
    Ball.onFellInWater += BallFellInWaterCallback;
    GameStateManager.onGameStateChanged += GameStateChangedCallback;
    Debug.Log("Event Subscribed");
  }

  public override void OnDestroy() {
    base.OnDestroy();
    NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
    Ball.onFellInWater -= BallFellInWaterCallback;
    GameStateManager.onGameStateChanged -= GameStateChangedCallback;
  }

  private void GameStateChangedCallback(GameStateManager.State gameState) {
    switch (gameState) {
      case GameStateManager.State.Game:
        ResetScores();
        break;
    }
  }

  private void ResetScores() {
    clientScore = 0;
    hostScore = 0;
    UpdateScoreClientRpc(clientScore, hostScore);
    UpdateScoreText();
    CheckForEndGame();
  }
  private void ReuseBall() {
    BallManager.instance.ReuseBall();
  }

  private void CheckForEndGame() {
    if (hostScore >= 3) {
      HostWin();
    } else if (clientScore >= 3) {
      ClientWin();
    } else {
      ReuseBall();
    }
  }
  public void HostWin() {
    HostWinClientRpc();
  }
  [ClientRpc]
  public void HostWinClientRpc() {
    if (IsServer)
      GameStateManager.instance.SetGameState(GameStateManager.State.Win);
    else
      GameStateManager.instance.SetGameState(GameStateManager.State.Lose);
  }
  public void ClientWin() {
    ClientWinClientRpc();
  }
  [ClientRpc]
  public void ClientWinClientRpc() {
    if (IsServer)
      GameStateManager.instance.SetGameState(GameStateManager.State.Lose);
    else
      GameStateManager.instance.SetGameState(GameStateManager.State.Win);
  }

  private void BallFellInWaterCallback() {
    Debug.Log("Inside BallFellInWaterCallback");
    if (PlayerSelector.instance.IsHostTurn())
      clientScore++;
    else
      hostScore++;
    //Update the displayed score
    UpdateScoreClientRpc(hostScore, clientScore);
    UpdateScoreText();
    CheckForEndGame();
  }

  [ClientRpc]
  private void UpdateScoreClientRpc(int hostScore, int clientScore) {
    this.hostScore = hostScore;
    this.clientScore = clientScore;
  }

  private void UpdateScoreText() {
    UpdateScoreTextClientRpc();
  }

  [ClientRpc]
  private void UpdateScoreTextClientRpc() {
    scoreText.text = hostScore +
     " - <color=#FF0000>" + clientScore + "</color>";
  }

  void Start() {
    UpdateScoreText();
  }
}
