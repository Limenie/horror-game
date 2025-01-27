using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] public GameObject invHolder; //place inventory canvas here 
    [SerializeField] public bool usingInv;
    [SerializeField] GameObject movement;
    public static Inventory inventory;
    public int[] collectableItems = { 0, 0 };
    [SerializeField] public Text pickupText; //text from counterui
    private float timeToAppear = 2f;
    private float timeWhenDisappear;

    public bool picInv;
    public GameObject picHolder; //place picturecanvas here

    public GameObject keyUI; //place counterui here

    GameObject getCamera;

    void Start()
    {
        inventory = this;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) //Press I to activate inventory
        {
            usingInv = true;
            ShowInv();
            disableCamera();
            Time.timeScale = 0f;
            Cursor.visible = true; //show cursor if inventory is on

        }
        else if (Input.GetKeyDown(KeyCode.Escape) && usingInv) //Press esc to cancel inventory
        {
            usingInv = false;
            HideInv();
            allowCamera();
            Time.timeScale = 1f;
            Cursor.visible = false; //hide cursor
        }

        if (pickupText.enabled && (Time.time >= timeWhenDisappear)) //pickup text disappearance time
        {
            pickupText.enabled = false;
        }

        if(Input.GetKeyDown(KeyCode.G)) //pic inventory
        {
            picInv = true;
            ShowPics(); //show pic inventory
            Time.timeScale = 0f; //stop game time
            HideKeyUI(); //hide key ui
            disableCamera(); //stop camera rotation
            Cursor.visible = true;

        }
        else if(Input.GetKeyDown(KeyCode.Escape) && picInv) //hide pic inventory
        {
            picInv = false;
            HidePics(); //hide picture canvas
            Time.timeScale = 1f; //allow game time to move
            ShowKeyUI(); //show key ui
            allowCamera(); //allow camera rotation
            Cursor.visible = false;
        }

    }

    void ShowInv() //show and hide functions for inventory canvas
    {
        invHolder.SetActive(true);
    }

    void HideInv()
    {
        invHolder.SetActive(false);
    }

    void ShowPics() //show and hide functions for picture canvas
    {
        picHolder.SetActive(true);
    }

    void HidePics()
    {
        picHolder.SetActive(false);
    }

    public void ShowKeyUI() //show and hide keyUI (made public so other classes can access
    {
        keyUI.SetActive(true);
    }

    public void HideKeyUI()
    {
        keyUI.SetActive(false);
    }

    void allowCamera()
    {
        getCamera = GameObject.Find("1stPCamera");
        getCamera.GetComponent<cameraController>().enabled = true;
    }

    void disableCamera()
    {
        getCamera = GameObject.Find("1stPCamera");
        getCamera.GetComponent<cameraController>().enabled = false;
    }

    public void AddItem(string ItemID, GameObject Object) //adding item from interaction
    {
        try
        {
            /* if(ItemID == GameObject.FindWithTag("Key").ToString()) //if an item has a tag "key"
             {
                 collectableItems[0]++;
             } */

            textAnimation(Object.ToString()); //(ItemID); //calling the pickup text. uses the objects actual name
        }
        catch
        {
            Debug.Log("Nothing to add");
        }
        }

    void textAnimation (string ItemID) //let text appear when item has been picked with the item name
    {
        pickupText.text = "Picked up a " + ItemID;
        pickupText.enabled = true;
        timeWhenDisappear = Time.time + timeToAppear;
    } 
}
