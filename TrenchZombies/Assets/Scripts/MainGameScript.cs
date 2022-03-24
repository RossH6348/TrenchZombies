using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Name: Ross Hutchins
//ID: HUT18001284

public class MainGameScript : MonoBehaviour
{

    //To access the ZombiesData script, adding types of zombies and even spawning them.
    [SerializeField] private ZombiesData zombieData;

    private int Wave = 1; //What wave is it currently?
    private int ZombieCount = 0; //How many zombies will there be in this wave?
    private string SpecialWave = ""; //Is it a special wave? As in only one type of zombie can exist.


    [SerializeField] private int MaxZombies = 20; //How many zombies can exist at a time? This will setup the object pooling upon start.
    [SerializeField] private GameObject zombieTemplate;
    [SerializeField] private Transform zombiePool;


    public int score = 0; //How much score does the player currently have?

    //All HUD elements of the player references.
    [SerializeField] private Text center;


    // Start is called before the first frame update
    void Start()
    {
        //Begin adding three types of zombies to the database.
        zombieData.addZombie("Zombie", 100.0f, 5.0f, new Vector3(1.0f, 1.0f, 1.0f), 1.0f);
        zombieData.addZombie("Fast Zombie", 50.0f, 7.5f, new Vector3(1.0f, 1.0f, 1.0f), 0.5f);
        zombieData.addZombie("Fat Zombie", 200.0f, 3.0f, new Vector3(1.4f, 1.4f, 0.8f), 0.25f);

        //Create our zombie object pool.
        for (int i = 0; i < MaxZombies; i++)
        {
            GameObject newZombie = Instantiate(zombieTemplate, zombiePool);
            newZombie.transform.localPosition = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //These two function/IEnumerator will allow me to display text on the player's screen.
    //For displaying store items and prices.
    public void centerPrint(Color color, string text, float time)
    {
        center.color = color;
        center.text = text;

        Debug.Log("LOL!");

        //Stop any coroutine that is clearing the print of the previous text.
        StopCoroutine("clearPrint");
        //Start a new one.
        StartCoroutine(clearPrint(time));
    }

    IEnumerator clearPrint(float time)
    {
        yield return new WaitForSeconds(time);
        center.text = "";
    }

}
