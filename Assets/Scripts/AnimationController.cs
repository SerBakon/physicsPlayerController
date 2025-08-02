using PurrNet;
using System.Globalization;
using UnityEngine;
public class AnimationController : NetworkBehaviour 
{
    [Header("Gameobjects")]
    [SerializeField] private GameObject playerPrefab;

    protected override void OnSpawned() {
        base.OnSpawned();

        enabled = isOwner;

        playerPrefab.gameObject.SetActive(!isOwner);
        Debug.Log("Removing gameobject complete");
    }
    
}
