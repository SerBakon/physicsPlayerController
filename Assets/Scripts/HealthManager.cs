using PurrNet;
using UnityEngine;

public class HealthManager : NetworkBehaviour
{
    protected override void OnSpawned() {
        base.OnSpawned();


        //if (isOwner)
            //InstanceHandler.GetInstance<MainGameView>.updateHealth();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
