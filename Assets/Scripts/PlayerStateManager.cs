using Unity.Netcode;
using UnityEngine;

public class PlayerStateManager : NetworkBehaviour {
  [Header(" Elements ")]
  [SerializeField] private SpriteRenderer[] renderers;
  [SerializeField] private Collider2D colider;

  public void Enable() {
    EnableClientRpc();
  }

  [ClientRpc]
  private void EnableClientRpc() {
    colider.enabled = true;
    foreach (SpriteRenderer renderer in renderers) {
      Color color = renderer.color;
      color.a = 1; // opaque
      renderer.color = color;
    }
  }
  public void Disable() {
    DisableClientRpc();
  }

  [ClientRpc]
  private void DisableClientRpc() {
    colider.enabled = false;
    foreach (SpriteRenderer renderer in renderers) {
      Color color = renderer.color;
      color.a = .2f; // seimitransparent
      renderer.color = color;
    }
  }
}