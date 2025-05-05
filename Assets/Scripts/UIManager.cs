using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode.Transports.UTP;


public class UIManager : MonoBehaviour {
  [Header(" Panels ")]
  [SerializeField] private GameObject connPannel;
  [SerializeField] private GameObject waitPannel;
  [SerializeField] private GameObject gamePannel;
  [SerializeField] private GameObject winPannel;
  [SerializeField] private GameObject losePannel;

  private UnityTransport utp;

  void Start() {
    ShowConnPannel();
    utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
    GameStateManager.onGameStateChanged += GameStateChangedCallback;
  }

  private void GameStateChangedCallback(GameStateManager.State s) {
    switch (s) {
      case GameStateManager.State.Game:
        ShowGamePannel();
        break;
      case GameStateManager.State.Win:
        ShowWinPannel();
        break;
      case GameStateManager.State.Lose:
        ShowLosePannel();
        break;
    }
  }
  public void OnDestroy() {
    GameStateManager.onGameStateChanged -= GameStateChangedCallback;
  }

  private void showPannels(bool c, bool wa, bool g, bool wi, bool l) {
    connPannel.SetActive(c);
    waitPannel.SetActive(wa);
    gamePannel.SetActive(g);
    winPannel.SetActive(wi);
    losePannel.SetActive(l);
  }
  private void ShowConnPannel() {
    showPannels(true, false, false, false, false);
  }
  private void ShowWaitPannel() {
    showPannels(false, true, false, false, false);
  }
  private void ShowGamePannel() {
    showPannels(false, false, true, false, false);
  }
  private void ShowWinPannel() {
    winPannel.SetActive(true);
  }

  private void ShowLosePannel() {
    losePannel.SetActive(true);
  }

  private bool IsValidIPAddress(string ipAddress) {
    if (IPAddress.TryParse(ipAddress, out IPAddress address)) {
      return true;
    }
    return false;
  }
  public void HostButtonCallback() {
    // Validar IP antes de intentar conectar
    string ipAddress = IPManager.instance.GetInputIP();
    if (!IsValidIPAddress(ipAddress)) {
      IPManager.instance.SetInputIP("Invalid IP");
      return;
    }
    utp.SetConnectionData(ipAddress, 777);
    NetworkManager.Singleton.StartHost();
    ShowWaitPannel();
  }
  public void ClientButtonCallback() {
    string ipAddress = IPManager.instance.GetInputIP();
    if (!IsValidIPAddress(ipAddress)) {
      IPManager.instance.SetInputIP("Invalid IP");
      return;
    }
    utp.SetConnectionData(ipAddress, 777);
    NetworkManager.Singleton.StartClient();
    ShowWaitPannel();
  }
  public void NextButtonCallback() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    NetworkManager.Singleton.Shutdown();
  }

}
