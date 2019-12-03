﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PhotoView : MonoBehaviour
{
    GameObject[] gameObj;
    Texture2D[] textList; //list for textures created from images

    string[] files;
    string pathPreFix;

    private RawImage img;

    void Awake()
    {
        deletePics(); //deletes previous pictures every time game is started (clears the path folder) the first time you open the pic inventory
    }


    void Update()
    {
        string path = @"C:\Users\alex\Desktop\ExamplePictureFolder"; //the path to picture folder

        pathPreFix = @"file://";

        files = System.IO.Directory.GetFiles(path, "*.png");

        gameObj = GameObject.FindGameObjectsWithTag("Pics"); //objects for pictures

        StartCoroutine(LoadImages());
        
    }

    private IEnumerator LoadImages() //can load 8 pics now. crashes when its over 8
    {
        textList = new Texture2D[files.Length];

        int index = 0;
        foreach (string tstring in files)
        {
            string pathTemp = pathPreFix + tstring;
            WWW www = new WWW(pathTemp);
            yield return www;
            Texture2D texTmp = new Texture2D(1024, 1024, TextureFormat.DXT1, false); //turning images to texture
            www.LoadImageIntoTexture(texTmp);
            textList[index] = texTmp;

            img = gameObj[index].GetComponent<RawImage>();
            img.texture = texTmp; //setting the texture to raw image
            index++;

           /* if(index > 7)
             {

             }  */
        }

    }

    void deletePics()
    {
        System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\Users\alex\Desktop\ExamplePictureFolder");

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
    }
}