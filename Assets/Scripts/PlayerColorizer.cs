using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerColorizer : NetworkBehaviour {
  [Header(" Elements ")]
  [SerializeField] private SpriteRenderer[] renderers;

  public override void OnNetworkSpawn() {
    base.OnNetworkSpawn(); /* invocas la implementación del método
                           * OnNetworkSpawn en la clase base */
    if(!IsServer&& IsOwner) {
      ColorizeServerRpc(Color.red);
    }
  }

  [ServerRpc]
  private void ColorizeServerRpc(Color color) {
    ColorizeClientRpc(color);
  }

  [ClientRpc]
  private void ColorizeClientRpc(Color color) {
    foreach(SpriteRenderer renderer in renderers) {
      renderer.color = color;
    }
  }
}