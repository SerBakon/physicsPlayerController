using PurrNet;
using System.Globalization;
using UnityEngine;
public class AnimationController : NetworkBehaviour 
{
    [Header("Gameobjects")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Scripts")]
    [SerializeField] private PlayerMovement PlayerMovement;

    protected override void OnSpawned() {
        base.OnSpawned();

        enabled = isOwner;

        //playerPrefab.gameObject.SetActive(!isOwner);
        //Debug.Log("Removing gameobject complete");
    }

    private void Update() {
        switch (PlayerMovement.state) {
            case PlayerMovement.movementState.Air:

                break;
        }
    }

}
