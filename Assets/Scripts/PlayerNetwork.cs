using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour {
  private void Update() {
    if (!IsOwner) {
      return;
    }
    Vector3 moveDirection = Vector3.zero;
    if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
    if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
    if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
    if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

    float moveSpeed = 3f;
    transform.position += moveDirection * moveSpeed * Time.deltaTime;
  }
}
