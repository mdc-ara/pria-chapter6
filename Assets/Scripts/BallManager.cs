using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class BallManager : NetworkBehaviour {
  public static BallManager instance;

  [Header(" Elements ")]
  [SerializeField] private Ball ballPrefab;

  private void Awake() {
    if (instance == null)
      instance = this;
    else
      Destroy(gameObject);
  }
  public void ReuseBall() {
    if (!IsServer)
      return;
    if (transform.childCount <= 0)
      return;
    transform.GetChild(0).GetComponent<Ball>().Reuse();
  }

  void Start() {
    GameStateManager.onGameStateChanged += GameStateChangedCallback;
  }

  public override void OnDestroy() {
    base.OnDestroy();
    GameStateManager.onGameStateChanged -= GameStateChangedCallback;
  }

  private void GameStateChangedCallback(GameStateManager.State gameState) {
    switch (gameState) {
      case GameStateManager.State.Game:
        SpawnBall();
        break;
    }
  }

  private void SpawnBall() {
    if (!IsServer)
      return;
    if (transform.childCount > 0)
      return;
    Ball b = Instantiate(ballPrefab, Vector2.up * 5, Quaternion.identity);
    b.GetComponent<NetworkObject>().Spawn();
    b.transform.SetParent(transform); // Set parent to the Manager
  }
}
