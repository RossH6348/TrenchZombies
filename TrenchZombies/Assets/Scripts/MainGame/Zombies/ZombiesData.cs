using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Name: Ross Hutchins
//ID: HUT18001284

public class ZombiesData : MonoBehaviour
{
    //Since there is going to be alot of zombies, I am going to use object pooling in this scenario.
    [SerializeField] private Transform zombiePool;
    [SerializeField] private Transform zombieSpawns;

    //This will contain list of all type of zombies that exist by name.
    public List<string> ZombiesName = new List<string>();

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

    //This function returns where all zombies are dead, in other words the entire object pool is not active.
    public bool isAllDead()
    {
        for (int i = zombiePool.childCount - 1; i > -1; i--)
            if (zombiePool.GetChild(i).gameObject.activeSelf)
                return false; //One is still active, so no.

        //If it reached here, then it means all of them are dead.
        return true;
    }

    //This funciton will spawn a random zombie from the database, given a set of spawn points to randomly spawn them at.
    public GameObject spawnZombie(float baseModifier = 1.0f, float incModifier = 0.0f, string isSpecial = "")
    {

        //Don't bother spawning a zombie if there is no type to spawn, or a spawn point to send it to.
        if (zombieSpawns.childCount < 1 || ZombiesName.Count < 1)
            return null;

        //isSpecial - Determines where the wave is a special wave where only one type of zombie will spawn.
        //But before we start applying the attributes from the database to a zombie, we need to find if there is a dead one
        //available for us to re-use from the object pool.
        GameObject zombie = null;
        for (int i = zombiePool.childCount - 1; i > -1; i--)
        {
            GameObject potential = zombiePool.GetChild(i).gameObject;
            if (!potential.activeSelf)
            {
                zombie = potential;
                break;
            }
        }

        if (zombie != null)
        {

            BaseZombie baseAI = zombie.GetComponent<BaseZombie>();
            NavMeshAgent agent = zombie.GetComponent<NavMeshAgent>();
            //We got a zombie we can apply attributes to, let pick a random zombie type, or elsewise use the isSpecial.
            if (isSpecial == "")
            {
                //Pick a random zombie based on probability, which is a little complicated to handle...
                //As it involves adding up all the probabilities, picking a random number out of that range and then comparing it down to zero.
                int index = 0;
                float rng = 0.0f;
                for(int i = 0; i < ZombiesName.Count; i++)
                    rng += ZombiesProbability[ZombiesName[i]];

                rng = Random.Range(0.0f, rng);
                while(index < ZombiesName.Count)
                {
                    string name = ZombiesName[index];
                    if(ZombiesProbability[name] < rng)
                    {
                        rng -= ZombiesProbability[name];
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                //We got a random zombie to pick from, let use its database.
                isSpecial = ZombiesName[index];
            }

            //Set the name of this zombie.
            zombie.name = isSpecial;

            //Calculate their max health based on the basemod and incmod provided by the wave system.
            baseAI.Health = ZombiesHealth[isSpecial] * baseModifier;

            //Add any extra health increase per wave.
            baseAI.Health *= incModifier;

            //Change scale and speed of this zombie.
            zombie.transform.localScale = ZombiesScale[isSpecial];
            agent.speed = ZombiesSpeed[isSpecial];

            //Go ahead and spawn this new zombie.
            zombie.transform.position = zombieSpawns.GetChild(Random.Range(0, zombieSpawns.childCount - 1)).position;
            zombie.SetActive(true);

        }

        return zombie;
    }
}
