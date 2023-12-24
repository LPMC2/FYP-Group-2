using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DilmerGames.Core.Singletons;
using Unity.Netcode.Components;

public class NetworkObjectManager : NetworkBehaviour
{
    public static NetworkObjectManager Singleton;
    [SerializeField]
    private int maxObjectInstanceCount = 3;
    private NetworkObject obj;
    public GameObject spawnedObj;
    private Vector3 pos;
    private Quaternion quaternion;
    private NetworkObject parentObj;
        public GameObject SpawnObject(GameObject targetObject, Vector3 position, Quaternion quaternion)
        {
        obj = targetObject.GetComponent<NetworkObject>();
        pos = position;
        this.quaternion = quaternion;
            SpawnObjServerRpc();


          return spawnedObj;
        }
    private void Awake()
    {
        Singleton = this;
    }
    private NetworkObject childObj;
    public void SetParent(NetworkObject childToParent, NetworkObject parentObj)
    {
        childObj= childToParent;
        this.parentObj = parentObj;
        SetParentServerRpc();
    }
    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        childObj.TrySetParent(parentNetworkObject);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnObjServerRpc()
    {
        NetworkObject networkObject = Instantiate(obj, pos, quaternion);
       
        spawnedObj = networkObject.gameObject;
        networkObject.SpawnAsPlayerObject(OwnerClientId);
    }
    #region Server Controls
    //[ServerRpc(RequireOwnership = false)]
    //public void SpawnObjectServerRpc(ForceNetworkSerializeByMemcpy<NetworkObject> target, ForceNetworkSerializeByMemcpy<Vector3> pos, ForceNetworkSerializeByMemcpy<Quaternion> quaternion)
    //{
    //    NetworkObject networkObject = (NetworkObject)Instantiate(target, pos.Value, Quaternion.identity);
    //    networkObject.SpawnAsPlayerObject(OwnerClientId);
        
    //}
    ////[ServerRpc(RequireOwnership = false)]
    ////public void SetParentServerRpc(NetworkObject target, ForceNetworkSerializeByMemcpy<Transform> parent = default)
    ////{
    ////    target.TrySetParent(parent);
    ////}
    //[ServerRpc(RequireOwnership = false)]
    //public void SetPositionServerRpc(NetworkObject target, ForceNetworkSerializeByMemcpy<Vector3> pos)
    //{
    //    target.transform.position = pos;
    //}
    //[ServerRpc(RequireOwnership = false)]
    //public void SetRotationServerRpc(NetworkObject target, ForceNetworkSerializeByMemcpy<Quaternion> rotation)
    //{
    //    target.transform.rotation = rotation;
    //}
    #endregion
    [ServerRpc(RequireOwnership = false)]
    public void SetParentServerRpc()
    {
        childObj.TrySetParent(parentObj);
    }
    NetworkObject targetToDestroy;
    public void DestroyObject(NetworkObject networkObject)
    {
        targetToDestroy = networkObject;
        DestroyObjectServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void DestroyObjectServerRpc()
    {
        targetToDestroy.Despawn();
    }
    NetworkVariable<Vector3> targetPosition = new NetworkVariable<Vector3>();
    NetworkVariable<Vector3> targetRotation = new NetworkVariable<Vector3>();
    Transform targetTransform;
    Quaternion rotation;
    public void ChangePosition(Vector3 finalPos, Transform transform)
    {
        targetPosition.Value = finalPos;
        targetTransform = transform;
        ChangePosServerRpc();
    }
    public void ChangeRotation(Vector3 finalRotation, Transform transform)
    {
        targetRotation.Value = finalRotation;
        targetTransform = transform;
        ChangeRotServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangePosServerRpc()
    {
        targetTransform.position = targetPosition.Value;
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangeRotServerRpc()
    {
        targetTransform.Rotate(targetRotation.Value, Space.World);
    }
    public void SetRotation(NetworkObject targetTran, Quaternion rotation)
    {
        targetTransform = targetTran.transform;
        this.rotation = rotation;
        SetRotationServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetRotationServerRpc()
    {
        targetTransform.rotation = rotation;
    }
    GameManager targetGameManager;
    public void UpdateManager(GameManager gameManager)
    {
        targetGameManager = gameManager;
    }
    [ServerRpc]
    private void UpdateManagerServerRpc()
    {
        GameManager.Singleton = targetGameManager;
    }
}

