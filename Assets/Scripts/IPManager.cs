using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class IPManager : MonoBehaviour {
  public static IPManager instance;

  [Header(" Elements ")]
  [SerializeField] private TMP_InputField ipInputField;
  [SerializeField] private TextMeshProUGUI ipText;

  private void Awake() {
    if (instance == null)
      instance = this;
    else
      Destroy(gameObject);
  }
  void Start() {
    ipText.text = GetLocalIPs();
  }
  public string GetInputIP() {
    return ipInputField.text;
  }
  public void SetInputIP(string s) {
    ipInputField.text = s;
  }
  public string GetLocalIPs() {
    List<string> ipList = new List<string>(); // 0 or more IPs in the host
    // All ips associated with the host
    IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

    foreach (IPAddress address in addressList) {
      // If IPv4
      if (address.AddressFamily == AddressFamily.InterNetwork) {
        ipList.Add(address.ToString());
      }
    }

    if (ipList.Count > 0) {
      return (string.Join(",", ipList));
    } else {
      return ("No IP found");
    }
  }
}