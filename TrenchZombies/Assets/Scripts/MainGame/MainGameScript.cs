using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

//Name: Ross Hutchins
//ID: HUT18001284

public class MainGameScript : MonoBehaviour
{

    //Since the mainGame is deactivated at the start, beginGame will initialize at least once.
    private bool hasInitialized = false;
    [SerializeField] private MainMenuScript mainMenu;

    //To access the ZombiesData script, adding types of zombies and even spawning them.
    [SerializeField] private ZombiesData zombieData;

    private int Wave = 1; //What wave is it currently?
    private int ZombieCount = 0; //How many zombies will there be in this wave?
    private string SpecialWave = ""; //Is it a special wave? As in only one type of zombie can exist.


    [SerializeField] private int MaxZombies = 20; //How many zombies can exist at a time? This will setup the object pooling upon start.
    [SerializeField] private GameObject zombieTemplate;
    [SerializeField] private Transform zombiePool;

    [SerializeField] private GameObject player;
    public PlayerMovement playerMovement;

    [SerializeField] private GameObject deathCam;

    //How much health does the fortress have?
    private float maxFortressHealth = 500.0f;
    private float FortressHealth = 500.0f;

    private int Lives = 3; //How many lives does the player have?
    public int score = 0; //How much score does the player currently have?

    //All HUD elements of the player references.
    [SerializeField] private Transform playerHUD;

    [SerializeField] private Text center;
    [SerializeField] private Transform fortressBar;
    [SerializeField] private Text HealthTxt;
    [SerializeField] private Text ScoreTxt;
    [SerializeField] private Text WaveTxt;

    //Inventory related elements.
    [SerializeField] private Image inventoryHUD;
    [SerializeField] private GameObject iconTemplate;

    //Powerups.
    [SerializeField] private GameObject gunshipPowerup;
    [SerializeField] private GameObject nukePowerup;

    //All related variables for tweaking difficulty of the game.
    [SerializeField] private int StartingCount = 6; //How many zombies at wave 1 should start with.
    [SerializeField] private int WaveInc = 2; //How many zombies to increase the StartingCount by per wave beyond the first wave.
    [SerializeField] private float SpecialWaveChance = 0.1f; //What are the chance of a special wave occurring where it is just one type? 0 - 1 = 0% - 100%

    [SerializeField] private float StartingHealth = 1.0f; //How much of the base health should Wave 1 zombies have? 0 - infinity = 0% - infinity% of base health.
    [SerializeField] private float HealthInc = 0.02f; //How much health increase should the zombies get per wave? 0 - infinity = 0% - infinity%

    [SerializeField] private int KillScore = 50; //How much points is rewarded for killing a zombie?
    [SerializeField] private int SurviveScore = 200; //How much points is rewarded for surviving a wave?


    //Pause menu of some sort.
    [SerializeField] private GameObject Pause;

    [SerializeField] private Text VolumeTxt;
    [SerializeField] private Text SensitivityTxt;

    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private Slider SensitivitySlider;

    [SerializeField] private Button ReturnButton;


    //List and class object for announcement messages.
    class AnnouncementText
    {
        public GameObject announcement; //The text component.
        private float lifeTime = 5.0f; //How much time has it got left being displayed?
        public AnnouncementText(GameObject announcementObj, Color color, string txt)
        {
            announcement = announcementObj;

            Text textObj = announcement.GetComponent<Text>();

            textObj.color = color;
            textObj.text = txt;
        }

        //If a color is not provided, it will overload and just default to white.
        public AnnouncementText(GameObject announcementObj, string txt)
        {
            announcement = announcementObj;

            Text textObj = announcement.GetComponent<Text>();

            textObj.color = Color.white;
            textObj.text = txt;
        }

        public void tick(float time, int index)
        {
            lifeTime -= time;
            if (lifeTime <= 0.0f)
                Destroy(announcement);
            else
                announcement.transform.localPosition = Vector3.MoveTowards(announcement.transform.localPosition, new Vector3(Screen.currentResolution.width / -2, Screen.currentResolution.height / 2 - (index * 80.0f), 0.0f), 128.0f * time);
        }
    }
    private List<AnnouncementText> announcements = new List<AnnouncementText>();
    [SerializeField] private GameObject announcementTemplate;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //This HUUUGE bit is just to handle the inventory display... When the player is equipping, scrolling and unequipping.

        //Get the itemdatas from the player's inventory.
        List<ItemData> datas = playerMovement.entity.getInventory();

        //Adjust the background's size based on how many items they have.
        inventoryHUD.rectTransform.sizeDelta = new Vector2(128.0f, 128.0f * datas.Count);

        //This is figuring out where to move the inventory background based on whether they are equipped or not.
        Vector3 inventoryPos = new Vector3(Screen.currentResolution.width / 2 - 128.0f, Screen.currentResolution.height / 2, 0.0f);
        float inventoryHeight = inventoryHUD.rectTransform.sizeDelta.y;

        if (!playerMovement.entity.equipItem)
            inventoryPos.y += inventoryHeight;

        inventoryHUD.transform.localPosition = Vector3.MoveTowards(inventoryHUD.transform.localPosition, inventoryPos, inventoryHeight * 10.0f * Time.deltaTime);

        //See if we need to update the inventory listing.
        int inventoryCount = inventoryHUD.transform.childCount;
        if (inventoryHUD.transform.childCount != datas.Count)
        {
            //Clear the list out now.
            for (int i = inventoryCount - 1; i > -1; i--)
                Destroy(inventoryHUD.transform.GetChild(i).gameObject);

            //Fill it out with new image icons.
            for (int i = 0; i < datas.Count; i++)
            {
                GameObject newIcon = Instantiate(iconTemplate, inventoryHUD.transform);
                newIcon.name = datas[i].name;

                Image img = newIcon.GetComponent<Image>();
                img.sprite = datas[i].itemIcon;

                newIcon.transform.localPosition = new Vector3(
                    64.0f,
                    ((float)i * -128.0f) - 64.0f,
                    0.0f
                );
                newIcon.SetActive(true);
            }
        }
        else
        {
            //Scale all images based on which one is being selected.
            //Smaller for those that are not selected.
            for(int i = inventoryCount - 1; i > -1; i--)
            {
                Transform icon = inventoryHUD.transform.GetChild(i);
                icon.localScale = (playerMovement.entity.currentItem - 1 == i ? new Vector3(1.0f, 1.0f, 1.0f) : new Vector3(0.667f, 0.667f, 0.667f));
            }
        }

        //This bit will handle moving and deleting of announcement messages.
        for(int i = announcements.Count - 1; i > -1; i--)
        {
            AnnouncementText announcement = announcements[i];
            if (announcement.announcement == null)
                announcements.RemoveAt(i); //This announcement no longer exists.
            else
                announcement.tick(Time.deltaTime, i);
        }

        //Handle the HUD elements of the player.
        HealthTxt.text = "Health: " + playerMovement.entity.Health.ToString();
        ScoreTxt.text = "Points: " + score.ToString();
        WaveTxt.text = "Wave: " + Wave.ToString();

        fortressBar.localScale = new Vector3(FortressHealth / maxFortressHealth, 1.0f);

    }

    //This function will reset everything, the wave, the player, the zombies and so on.
    public void beginGame()
    {

        //Check if this is the first time the game has begun, if so we need to initialize everything.
        if (!hasInitialized)
        {

            //Begin adding three types of zombies to the database.
            zombieData.addZombie("Zombie", 100.0f, 5.0f, new Vector3(1.0f, 1.0f, 1.0f), 1.0f);
            zombieData.addZombie("Fast Zombie", 50.0f, 7.0f, new Vector3(1.0f, 1.0f, 1.0f), 0.5f);
            zombieData.addZombie("Fat Zombie", 200.0f, 3.75f, new Vector3(1.4f, 0.9f, 1.4f), 0.334f);

            //Create our zombie object pool.
            for (int i = 0; i < MaxZombies; i++)
            {
                GameObject newZombie = Instantiate(zombieTemplate, zombiePool);
                newZombie.transform.localPosition = Vector3.zero;
            }
            hasInitialized = true;
        }

        //Reset the wave system, score, and lives.
        Wave = 1;
        ZombieCount = 0;
        SpecialWave = "";

        score = 0;
        Lives = 3;

        //Reset all zombies that are still active.
        foreach (Transform zombieTrans in zombiePool)
           zombieTrans.gameObject.SetActive(false);

        //Reset the player.
        deathCam.SetActive(false);
        player.SetActive(true);
        playerMovement.Respawn();
        playerMovement.entity.clearItems();

        //Reset all the powerups.
        gunshipPowerup.SetActive(false);
        nukePowerup.SetActive(false);
        nukePowerup.transform.position = new Vector3(0.0f, 30.0f, 0.0f);

        //Clear their announcement messages as well, and centerprint.
        StopCoroutine("clearPrint");
        StartCoroutine(clearPrint(0.0f));

        for (int i = announcements.Count - 1; i > -1; i--)
        {
            AnnouncementText announcement = announcements[i];
            if (announcement.announcement != null)
                Destroy(announcement.announcement);
            announcements.RemoveAt(i);
        }

        //Close the pause menu.
        Pause.SetActive(false);

        //Time to begin the game loop.
        StopCoroutine("WaveSystem");
        StartCoroutine("WaveSystem");

        //Prevent any future respawnplayers.
        StopCoroutine("RespawnPlayer");

    }

    //Self-explanatory.
    public void endGame()
    {
        mainMenu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    //These two function/IEnumerator will allow me to display text on the player's screen.
    //For displaying store items and prices, or other announcements.
    public void centerPrint(Color color, string text, float time)
    {
        center.color = color;
        center.text = text;

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

    //This will announce large text in the top left corner of the player's screen.
    //This will be useful for such as the game announcing when the wave begins and so on.
    public void announce(Color color, string text)
    {

        GameObject newAnnouncement = Instantiate(announcementTemplate, playerHUD);
        announcements.Add(new AnnouncementText(newAnnouncement, color, text));

        //Auto-snap it to position.
        newAnnouncement.transform.localPosition = new Vector3(Screen.currentResolution.width / -2, Screen.currentResolution.height / 2 - ((announcements.Count - 1) * 80.0f), 0.0f);

        newAnnouncement.SetActive(true);

    }

    private void GameOver()
    {
        //Game over...
        mainMenu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }


    //Reward player points for killing the zombie.
    public void onKill(GameObject zombie)
    {
        score += KillScore;
    }

    //The player have been killed, carry out the death cutscene.
    public void onKilled()
    {
        player.SetActive(false);
        deathCam.SetActive(true);

        Lives--;
        if (Lives > 0)
        {
            announce(Color.red, "You have died! You will respawn in 10 seconds, " + Lives.ToString() + " " + (Lives > 1 ? "lives" : "life") + " left...");
            StartCoroutine("RespawnPlayer");
        }
        else
        {
            //Game over...
            GameOver();
        }
    }

    //This function allow the zombie to slowly start draining the fortress' health.
    public void DamageFortress()
    {
        if(FortressHealth > 0.0f)
        {
            FortressHealth -= Time.deltaTime * 2.5f;
            if(FortressHealth <= 0.0f)
            {
                //Game over!!!
                GameOver();
            }
        }
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(10.0f);
        player.SetActive(true);
        playerMovement.Respawn();
        deathCam.SetActive(false);
    }


    //This will be the main loop for handling the games' wave system.
    //It will be running in a coroutine, as it only need to periodically check every now and then to spawn zombies.
    //Or whether to advance to the next wave.

    private int stage = 0; //This will represent what point in the game is the player currently is in.

    //0 - Wave countdown before it begins.
    //1 - Mid-wave/Wave in progress.
    //2 - Advancing wave and rewarding points.

    private int Countdown = 60; //There will be a minute given between waves to allow players to buy powerups and new weapons.

    IEnumerator WaveSystem()
    {

        //Reset both stage and countdown values.
        stage = 0;
        Countdown = 60;

        while (true)
        {

            float delay = 0.0f;

            if(stage == 0)
            {
                //Wave countdown/preparation, tickrate will be one every second.
                delay = 1.0f;

                if (Countdown > 0) {
                    if ((Countdown > 15 && Countdown % 30 == 0) || (Countdown <= 15 && Countdown % 5 == 0))
                        announce(Color.yellow, (SpecialWave != "" ? "Special wave " : "Wave ") + Wave.ToString() + " will begin in " + Countdown.ToString() + " second(s)!");
                    Countdown--;
                }
                else
                {
                    //The wave have begun! Calculate how many zombies there will be for this wave, and advance to stage 1!
                    announce(Color.yellow, (SpecialWave != "" ? "Special wave " : "Wave ") + Wave.ToString() + " has begun!");
                    ZombieCount = StartingCount + (WaveInc * (Wave - 1));
                    stage = 1;
                }

            }else if(stage == 1)
            {
                //Mid-wave, increase tickrate to 10 a seconds for spawning and checking when all zombies are dead.
                delay = 0.1f;

                //See if we need to spawn in more zombies, if so attempt to spawn one.
                //The spawnZombie function will return null if it fails to do so, otherwise a valid gameObject.
                //Therefore in short, the spawnZombie function will handle the object pooling for us.
                if (ZombieCount > 0 && zombieData.spawnZombie(StartingHealth, Mathf.Pow((1.0f + HealthInc), (float)(Wave - 1)), SpecialWave) != null)
                    ZombieCount--;
                else if (ZombieCount < 1 && zombieData.isAllDead())
                    stage = 2; //All of them have been killed now, proceed to stage 2.
            }
            else
            {
                //Announce that the wave is over, and reward the player points for surviving.
                announce(Color.yellow, "Wave " + Wave.ToString() + " survived! " + SurviveScore.ToString() + " points rewarded.");
                score += SurviveScore;

                //Time to calculate whether it should be special wave the next time or not.
                //If it is a special wave, pick a random name from the zombies database to pass to the spawnZombie.
                //Effectively telling it to only spawn this type of zombie this wave, and not random ones.
                SpecialWave = (Random.Range(0.0f, 1.0f) <= SpecialWaveChance ? zombieData.ZombiesName[Random.Range(0, zombieData.ZombiesName.Count - 1)] : "");

                //Advance to the next wave.
                Wave++;

                //Reset the countdown and return back to the first stage: Countdown.
                Countdown = 60;
                stage = 0;
            }

            //Check all the logics again within a second.
            yield return new WaitForSeconds(delay);
        }

    }

    //This will allow players to skip the wave countdowns, incase they dont need to buy new things and so on.
    public void skipCountdown()
    {
        if (stage == 0 && Countdown > 0)
        {
            announce(Color.green, "Skipping wave countdown...");
            Countdown = 0;
        }
        else
        {
            announce(Color.red, "Cannot skip countdown, as it is not currently countdown stage!");
        }
    }



    //Functions related to the pause menu.
    public void onPause()
    {
        Pause.SetActive(!Pause.activeSelf);
        if (Pause.activeSelf)
        {

            //Update the values and re-execute.
            VolumeSlider.value = AudioListener.volume;
            SensitivitySlider.value = playerMovement.sensitivity;
            onVolumeChange();
            onSensitivityChange();

            ReturnButton.Select();
            ReturnButton.OnSelect(null);
        }

        Cursor.lockState = (Pause.activeSelf ? CursorLockMode.None : CursorLockMode.Locked);

    }

    public void onVolumeChange()
    {
        VolumeTxt.text = "Volume - " + Mathf.Floor(VolumeSlider.value * 100.0f).ToString() + "%";
        AudioListener.volume = VolumeSlider.value;
    }

    public void onSensitivityChange()
    {
        //Always keep it a flat number, using floor function.
        SensitivitySlider.value = Mathf.Floor(SensitivitySlider.value);

        //Update the text and player values.
        SensitivityTxt.text = "Sensitivity - " + SensitivitySlider.value.ToString();
        playerMovement.sensitivity = SensitivitySlider.value;
    }

    public void onReturnClick()
    {
        mainMenu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

}
