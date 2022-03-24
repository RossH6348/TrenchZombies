using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This entire script will hold all the different types of zombies, and their data regarding: Health, Probability of Spawning, Appearance and so on.
public class ZombieDatabse : MonoBehaviour
{

    //Since there is going to be alot of zombies, I am going to use objedt pooling in this scenario.
    [SerializeField] private Transform zombiePool;

    //This will contain list of all type of zombies that exist by name.
    private List<string> ZombiesName = new List<string>();

    //Using the name as a key, we can get all the data related to that zombie.
    private Dictionary<string, float> ZombiesHealth = new Dictionary<string, float>(); //How much health will they spawn with?
    private Dictionary<string, float> ZombiesSpeed = new Dictionary<string, float>(); //How fast can they move?
    private Dictionary<string, Vector3> ZombiesScale = new Dictionary<string, Vector3>(); //How large or small will they physically be?
    private Dictionary<string, float> ZombiesProbability = new Dictionary<string, float>(); //What the odds of this zombie spawning when we need to spawn a zombie.

    public void addZombie(string name, float maxHealth, float speed, Vector3 scale, float probability)
    {
        //Check if this zombie type does not exist, if so we can create a new database.
        if (!ZombiesName.Contains(name))
        {
            //Add this new type of zombie to the database.
            ZombiesName.Add(name);
            ZombiesHealth.Add(name, maxHealth);
            ZombiesSpeed.Add(name, speed);
            ZombiesScale.Add(name, scale);
            ZombiesProbability.Add(name, probability);
        }
    }

    //This funciton will spawn a random zombie from the database, given a set of spawn points to randomly spawn them at.
    public GameObject spawnZombie(List<Transform> zombieSpawns, string isSpecial = "")
    {

        //Don't bother spawning a zombie if there is no type to spawn, or a spawn point to send it to.
        if (zombieSpawns.Count < 1 || ZombiesName.Count < 1)
            return null;

        //isSpecial - Determines where the wave is a special wave where only one type of zombie will spawn.
        //But before we start applying the attributes from the database to a zombie, we need to find if there is a dead one
        //available for us to re-use from the object pool.
        GameObject zombie = null;
        for (int i = zombiePool.childCount - 1; i > -1; i--)
        {
            if (!zombiePool.GetChild(i).gameObject.activeSelf)
            {
                zombie = zombiePool.GetChild(i).gameObject;
                break;
            }
        }

        if(zombie != null)
        {

            //We got a zombie we can apply attributes to, let pick a random zombie type, or elsewise use the isSpecial.
            if(isSpecial != "")
            {

            }
            else
            {

            }

            zombie.transform.position = zombieSpawns[Random.Range(0, zombieSpawns.Count - 1)].position;
        }

        return zombie;
    }
}
