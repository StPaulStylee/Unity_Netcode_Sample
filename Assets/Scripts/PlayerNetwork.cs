using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour {

  [SerializeField] private Transform spawnedObjectPrefab;
  private Transform spawnedObject;

  private NetworkVariable<MyCustomData> myData = new NetworkVariable<MyCustomData>(
    new MyCustomData {
      Int = 23,
      Bool = true
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  public override void OnNetworkSpawn() {
    myData.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
      // OwnerClientId is part of NetworkBehavior
      Debug.Log($"{OwnerClientId}; Int: {newValue.Int}, Bool: {newValue.Bool}, Message: {newValue.Message}");
    };
  }

  public struct MyCustomData : INetworkSerializable {
    public int Int;
    public bool Bool;
    public FixedString128Bytes Message;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
      serializer.SerializeValue(ref Int);
      serializer.SerializeValue(ref Bool);
      serializer.SerializeValue(ref Message);
    }
  }

  private void Update() {
    if (!IsOwner) {
      return;
    }
    if (Input.GetKeyDown(KeyCode.T)) {
      spawnedObject = Instantiate(spawnedObjectPrefab);
      spawnedObject.GetComponent<NetworkObject>().Spawn(true);
      // TestClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } } });
      // TestServerRpc(new ServerRpcParams());
      // myData.Value = new MyCustomData {
      //   Int = Random.Range(0, 100),
      //   Bool = !myData.Value.Bool,
      //   Message = $"Hello there, from OwnerClientId: {OwnerClientId}"
      // };
    }

    if (Input.GetKeyDown(KeyCode.Y)) {
      Destroy(spawnedObject.gameObject);
    }

    Vector3 moveDirection = Vector3.zero;
    if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
    if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
    if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
    if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

    float moveSpeed = 3f;
    transform.position += moveDirection * moveSpeed * Time.deltaTime;
  }

  [ServerRpc]
  private void TestServerRpc(ServerRpcParams serverRpcParams) {
    Debug.Log($"TestServerRpc(): {OwnerClientId}; SenderClientId: {serverRpcParams.Receive.SenderClientId}");
  }

  [ClientRpc]
  private void TestClientRpc(ClientRpcParams clientRpcParams) {
    Debug.Log($"ClientServerRpc(): {OwnerClientId}");
  }
}
