using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour {
  private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  public override void OnNetworkSpawn() {
    randomNumber.OnValueChanged += (int previousValue, int newValue) => {
      // OwnerClientId is part of NetworkBehavior
      Debug.Log($"{OwnerClientId}; {randomNumber.Value}");
    };
  }

  private void Update() {
    if (!IsOwner) {
      return;
    }
    if (Input.GetKeyDown(KeyCode.T)) {
      randomNumber.Value = Random.Range(0, 100);
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
