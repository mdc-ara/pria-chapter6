using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour {
  [Header(" Physics Settings ")]
  [SerializeField] private float bounceVelocity;
  private Rigidbody2D rig;
  private bool isAlive;
  private float gravityScale;

  [Header(" Events ")]
  public static Action onHit;
  public static Action onFellInWater;
  void Start() {
    rig = GetComponent<Rigidbody2D>();
    isAlive = true;
    gravityScale = rig.gravityScale;
    rig.gravityScale = 0;
    // Wait 2 seconds then fall
    StartCoroutine("WaitAndFall");
  }

  IEnumerator WaitAndFall() {
    yield return new WaitForSeconds(2);
    rig.gravityScale = gravityScale;
  }

  public void Reuse() {
    transform.position = Vector2.up * 5;
    rig.velocity = Vector2.zero;
    rig.angularVelocity = 0;
    rig.gravityScale = 0;
    isAlive = true;
    StartCoroutine("WaitAndFall");
  }

  private void Bounce(Vector2 normal) {
    rig.velocity = normal * bounceVelocity;
  }
  private void OnCollisionEnter2D(Collision2D collision) {
    if (!isAlive)
      return;
    if (collision.collider.TryGetComponent(out PlayerControler pc)) {
      Bounce(collision.GetContact(0).normal);
      onHit?.Invoke();
    }
  }
  private void OnTriggerEnter2D(Collider2D collider) {
    Debug.Log("Inside Ball's OnTriggerEnter2D");
    if (!isAlive)
      return;
    if (collider.CompareTag("Water")) {
      isAlive = false;
      onFellInWater?.Invoke();
    }
  }
}
