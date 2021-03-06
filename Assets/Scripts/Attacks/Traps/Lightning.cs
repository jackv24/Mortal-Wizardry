﻿/*
**  Lightning.cs: Controls lightning after it has been spawned
*/

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Lightning : NetworkBehaviour
{
    //How much hp is drained per second
    public int damage = 20;

    //How long after spawning until it is destroyed
    public float lifeTime = 1f;
    //Time until new bolt in chain is spawn
    public float spawnTime = 0.25f;

    public int chainLength = 5;

    public float minDistance = 1f;
    public float maxDistance = 3f;

    //The owner of this trap
    [SyncVar]
    public GameObject owner;

    void Start()
    {
        //Destroy gameobject after time
        Destroy(gameObject, lifeTime);

        if (isServer)
            StartCoroutine("SpawnCounter");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Handle colisions only on the server
        if (!isServer)
            return;

        //Add all player that enter, except for the owner
        if (other.transform.parent.tag == "Player" && other.transform.parent.gameObject != owner)
        {
            PlayerStats playerStats = other.transform.parent.GetComponent<PlayerStats>();

            playerStats.CmdApplyDamage(damage, owner.GetComponent<PlayerInfo>().username, "Lightning");
        }
    }

    [Command]
    void CmdSpawn(Vector3 origin)
    {
        Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        direction.Normalize();

        Vector3 newPos = origin + direction * Random.Range(minDistance, maxDistance);

        GameObject obj = (GameObject)Instantiate(gameObject, newPos, Quaternion.identity);
        obj.name = name;
        obj.SetActive(true);

        Lightning lightning = obj.GetComponent<Lightning>();
        lightning.SetOwner(owner);
        lightning.chainLength--;

        NetworkServer.Spawn(obj);
    }

    IEnumerator SpawnCounter()
    {
        yield return new WaitForSeconds(spawnTime);

        if (chainLength > 0)
        {
            CmdSpawn(owner.transform.position);
        }
    }

    //Called via sendmessage from playerattack
    void SetOwner(GameObject obj)
    {
        owner = obj;
    }
}
