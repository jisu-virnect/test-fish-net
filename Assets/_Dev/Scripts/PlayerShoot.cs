using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    public int damage;

    public float timeBetweenFire = 0.5f;
    float fireTimer;

    public Transform gun;
    public GameObject bullet;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
            GetComponent<PlayerShoot>().enabled = false;
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (fireTimer < Time.time)
            {
                fireTimer = Time.time+ timeBetweenFire;
                //ShootServer(damage, Camera.main.transform.position, Camera.main.transform.forward);
            }
            //GameObject spawned = Instantiate(bullet, gun.position, Quaternion.Euler(gun.transform.eulerAngles));
            //ServerManager.Spawn(spawned);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServer(int damageToGive, Vector3 position, Vector3 direction)
    {
        if(Physics.Raycast(position,direction, out RaycastHit hit) && hit.transform.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.hp -= damageToGive;
        }
    }
}
