﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionCounterScript : MonoBehaviour
{
    public static InteractionCounterScript interactioncounterscript;

    public Camera camera; //the players camera that faces the objects
    public float interactDistance = 15f;
    public GameObject interactingGameObject;
    public string interactingObjectName;
    public bool interact;
    public RaycastHit hit;

    [SerializeField] public GameObject itemsDB; //place itemsdb object here
    [SerializeField] public GameObject inventorySlots; //place invitemspace from inventory canvas here
    public RawImage[] slots;

    [SerializeField] public GameObject paperTextHolder; //place canvasforpapertext here
    [SerializeField] public bool usingPaperText;
    public Text PaperText; //papertext from canvasforpapertext

    public Text pressEText; //text to let the player know they can interact by pressing e (or any other button). this text object is inside counterui canvas
    private float timeToAppear = 0.001f;
    private float timeWhenDisappear;

    GameObject camObj;

    CharacterController charCtrl;

    public GameObject keyPadCanvasHolder; //set the keypad canvas here
    public bool usingKeyPad;

    [Header("Data")]
    public int data_amount_key = 0;
    public Text data_text_key; //place textcounter from counterui canvas here

    public bool GotLobbyKey;
    public bool GotLabyrKey;
    public bool GotKeysFor2ndFDoor;

    public static bool GameIsPaused;

    public GameObject monsterLibrary; //insert library monster here
    public AudioSource monsterLibSound; //insert library monster here (it has an audio source)

    public GameObject crawlerBath; //insert crawler here
    public AudioSource crawlerBathSound; // insert crawler's audio source here
    bool crawlersOn;

    public GameObject kitchenWallMonsters; //insert kitchenwallmonsters object here
    public AudioSource kitchenWallMonstersSound; //insert the audio you want (test audio is inside the first kitchenwallmonster
    bool kitchenWallMonstersOn;

    private float timeToAppearM = 15f; //set a time for the monsters to disappear
    private float timeToDisappearM;

    public AudioSource lastPartAudioSource; //the last soundtrack when the last key is picked

    public GameObject EndCreditsHolder;

    bool CreditsAreOn;

    public GameObject windowMonster;

    AudioSource pianoSound;

    void Start()
    {
        //InvokeRepeating("search", 0f, 0.5f); //item search
        data_text_key.text = data_amount_key.ToString(); //number text matches the amount of keys

        camObj = GameObject.Find("1stPCamera");

        interactioncounterscript = this;

        windowMonster.SetActive(false);
        monsterLibrary.SetActive(false); //hide library monster
        crawlerBath.SetActive(false); // hide crawlers in the bathroom
        kitchenWallMonsters.SetActive(false); //hide monster hands in the kitchen

        pianoSound = GameObject.Find("piano").GetComponent<AudioSource>(); //stop the piano sound
        pianoSound.Stop();

        lastPartAudioSource = GameObject.Find("LastPartAudioSource").GetComponent<AudioSource>();

    }

    public void Update()
    {
        camera.transform.localRotation = Quaternion.identity; //the magical fix to raycasting
        camera.transform.localPosition = Vector3.zero;
        search();

        Debug.DrawRay(camera.transform.position, camera.transform.forward, Color.yellow); // ray debug

        try //catch the no object errors w try catch
        {
            if (Input.GetKeyDown(KeyCode.E)) //if E is pressed, item will be picked
            {
                if (hit.collider.tag == "Key") //if the hit collider has a tag "key" (keys for the 2nd floor door)
                {
                    Debug.Log("I tried to pick up a " + interactingObjectName);

                    data_amount_key++; //key added, number goes up
                    data_text_key.text = data_amount_key.ToString();

                    Inventory.inventory.AddItem(interactingObjectName, interactingGameObject);
                    hit.transform.SetParent(itemsDB.transform);
                    AddToInventory(hit.transform);

                    clearData(); //deleting the item from scene
                    return;
                }

                if (hit.collider.tag == "librarykey") //if u pick up the library key, a monstah appears w sound
                {
                    Debug.Log("I tried to pick up a " + interactingObjectName);

                    data_amount_key++; //key added, number goes up
                    data_text_key.text = data_amount_key.ToString();

                    Inventory.inventory.AddItem(interactingObjectName, interactingGameObject);
                    hit.transform.SetParent(itemsDB.transform);
                    AddToInventory(hit.transform);

                    monsterLibrary.SetActive(true);
                    monsterLibSound.Play();

                    clearData(); //deleting the item from scene
                    return;
                }

                if(hit.collider.tag == "labkey") //key to open the labyrinth door [notice tags]
                {
                    GotLabyrKey = true;

                    Inventory.inventory.AddItem(interactingObjectName, interactingGameObject);
                    hit.transform.SetParent(itemsDB.transform);
                    AddToInventory(hit.transform);

                    crawlerBath.SetActive(true);
                    crawlerBathSound.Play();
                    //crawlerBathSound.loop = true;
                    crawlersOn = true;
                    timeToDisappearM = Time.time + timeToAppearM; //time for disappearing so we're not keeping assets active that are not needed

                    clearData(); //deleting the item from scene
                    return;
                }

                if (hit.collider.tag == "kitchenkey") //if u pick up the kitchen key [notice tags]
                {
                    Debug.Log("I tried to pick up a " + interactingObjectName);

                    data_amount_key++; //key added, number goes up
                    data_text_key.text = data_amount_key.ToString();

                    Inventory.inventory.AddItem(interactingObjectName, interactingGameObject);
                    hit.transform.SetParent(itemsDB.transform);
                    AddToInventory(hit.transform);

                    kitchenWallMonsters.SetActive(true);
                    kitchenWallMonstersSound.loop = true;
                    kitchenWallMonstersOn = true;

                    timeToDisappearM = Time.time + timeToAppearM;

                    clearData(); //deleting the item from scene
                    return;
                }
                
                if (hit.collider.tag == "lastkey") //if we pick up the last key, the soundtrack changes
                {
                    Debug.Log("I tried to pick up a " + interactingObjectName);

                    data_amount_key++; //key added, number goes up
                    data_text_key.text = data_amount_key.ToString();

                    Inventory.inventory.AddItem(interactingObjectName, interactingGameObject);
                    hit.transform.SetParent(itemsDB.transform);
                    AddToInventory(hit.transform);

                    CharacterMovement.charactermovement.backgroundmusic.Stop();

                    lastPartAudioSource.Play(); //change the music
                    lastPartAudioSource.loop = true; 


                    clearData(); //deleting the item from scene
                    return;
                }

                if (hit.collider.tag == "Item") //if the object has a tag "item" -> can be instered to any game object. remember to add box colliders (2), one has the trigger option and is sized correctly and one keeps the gravity for the object. then add a raw image and the texture = icon of the item. shows up in the inventory
                {
                    Debug.Log("I tried to pick up a " + interactingGameObject.ToString()); //gives u the whole object name
                    Inventory.inventory.AddItem(interactingObjectName, interactingGameObject);
                    hit.transform.SetParent(itemsDB.transform);
                    AddToInventory(hit.transform);

                    clearData(); //deleting the item from scene
                    return;

                }

                if (hit.collider.tag == "door") //regular door interaction
                {
                        doorToggle doorSc = interactingGameObject.GetComponent<doorToggle>();
                        doorSc.toggleDoor();
                }

                if (hit.collider.tag == "corridorlockeddoor") //corridor door interaction
                {
                    if (GotLobbyKey == true)
                    {
                        Destroy(GameObject.Find("lockforcorridor")); //destroy the lock object when opening the locked door

                        doorToggle doorSc = interactingGameObject.GetComponent<doorToggle>();
                        doorSc.toggleDoor();

                        slots = inventorySlots.GetComponentsInChildren<RawImage>();
                        slots[0].texture = null; //take the key away from the inventory slot. we're assuming that its the first texture item you pick up
                    }
                }
                if (hit.collider.tag == "lablockeddoor") //labyrinth door interaction
                {
                    if (GotLabyrKey == true)
                    {
                        Destroy(GameObject.Find("lockforlabyrinth")); //destroy the lock of labyrinth door

                        doorToggle doorSc = interactingGameObject.GetComponent<doorToggle>();
                        doorSc.toggleDoor();

                        slots = inventorySlots.GetComponentsInChildren<RawImage>();
                        slots[0].texture = null; //take the key away from the inventory slot. we're assuming that its the first texture item you pick up
                    }
                }
                if (hit.collider.tag == "lockedbigdoor") //open big door in the second floor if all the keys have been collected (tagged as keys)
                {
                    //if (GotKeysFor2ndFDoor == true)
                    Debug.Log("hit bigdoor");

                    if (data_amount_key == 4)
                    {
                        Debug.Log("we have the key amount");
                        //GameObject.Find("EndCredits").SetActive(true); //show end credits if player opens the last door (canvas)
                        EndCreditsHolder.SetActive(true);
                        Time.timeScale = 0f;
                        camObj.GetComponent<cameraController>().enabled = false;
                        Inventory.inventory.HideKeyUI();
                        lastPartAudioSource.Stop(); //stop the last soundtrack
                        CharacterMovement.charactermovement.backgroundmusic.Stop();
                        GameObject.Find("EndCredits").GetComponent<AudioSource>().Play(); //play the soundtrack thats attached to the endcredits
                        CreditsAreOn = true;
                    }
                }

                if(hit.collider.tag == "safewithkeypad") //the safe under the paining, between the staircase. we're activating the canvas, the actual process happens inside KeyPadSystem script
                {
                    keyPadCanvasHolder.SetActive(true);
                    usingKeyPad = true;
                    Cursor.visible = true;
                    Time.timeScale = 0f;
                    camObj.GetComponent<cameraController>().enabled = false;
                }

            }
            else if (Input.GetKeyDown(KeyCode.Escape) && CreditsAreOn) //if credits are on and we have pressed esc
            {
                Application.Quit(); //doesnt do anything yet
            }

            if (hit.collider.tag == "piano")
            {
                //interactingGameObject.GetComponent<AudioSource>().Play(); //play the audio piano has
                pianoSound.Play();
                windowMonster.SetActive(true);
            } 

            //remember to add text component (hidden) to the paper object! and write the text you want to appear
            if (hit.collider.tag == "paper") //also remember to set a triggered box collider to the paper object. make sure the character's interaction camera can see the paper collider from close distance
            {
               // pressEText.enabled = true;
               // pressEText.text = "press E to interact"; //let the player know they can interact with this object
               // timeWhenDisappear = Time.time + timeToAppear;

                if (Input.GetKeyDown(KeyCode.E)) //press e to interact
                {
                      pressEText.enabled = false;
                      usingPaperText = true;
                      paperTextHolder.SetActive(true); //set the paper canvas active
                      PaperText.text = interactingGameObject.GetComponent<Text>().text.ToString(); //gets the text component from the object you're interacting with and shows it
                      PaperText.enabled = true;  //text of the paper
                      Time.timeScale = 0f; // <- if we want to stop the character. camera needs a separated stop function
                      camObj.GetComponent<cameraController>().enabled = false; //no camera movement allowed
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape) && usingPaperText) // esct -> get away from paper screen
            {
                usingPaperText = false;
                paperTextHolder.SetActive(false);
                PaperText.enabled = false;
                Time.timeScale = 1f;
                camObj.GetComponent<cameraController>().enabled = true;
                Cursor.visible = false;
            }

            if (pressEText.enabled && (Time.time >= timeWhenDisappear)) //pickup text disappearance time
            {
                pressEText.enabled = false;
            }

            if (kitchenWallMonstersOn = true && (Time.time >= timeToDisappearM)) //disappearance time
            {
                kitchenWallMonsters.SetActive(false);
            }

            if (crawlersOn = true && (Time.time >= timeToDisappearM)) //disappearance time
            {
                crawlerBath.SetActive(false);
            }

        } catch (System.Exception ex)
        {
            Debug.Log("No object tagged" + ex);
        }
    }

    void search ()
    {

        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit) && hit.distance <= interactDistance) //cameras distance and ray from interactable items

        {
            hit.distance = interactDistance;
            resetData();
            interact = true;

            interactingObjectName = hit.collider.tag; //object name = tag the object has
            interactingGameObject = hit.transform.gameObject; //object = the object the player cam is facing
        }
        else
        {
            interact = false;
            resetData();
        }
    }

    void resetData() //if theres no item, reset
    {
        if (interactingGameObject == null) return;

        interactingGameObject = null;
        interactingObjectName = null;
    }

    void clearData() //if there is an item (in cameras view), destroy the item (taking)
    {
        if (interactingGameObject != null)
        {
             Destroy(interactingGameObject);
            //interactingGameObject.SetActive(false);
        }
        interactingGameObject = null;
    }

    public void AddToInventory(Transform item)
    {
        Debug.Log("Adding to inv..");
        slots = inventorySlots.GetComponentsInChildren<RawImage>(); //slots match the rawimages from slot objects

        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].texture == null)
            {
                slots[i].texture = item.GetComponent<RawImage>().texture; //replacing slot texture with the texture of the picked object (icon)
                return;
            }
        }
    }
}
