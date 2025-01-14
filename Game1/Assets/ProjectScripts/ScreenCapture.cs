﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenCapture : MonoBehaviour
{
    private static ScreenCapture instance;

    private Camera captureCamera;
    private bool takeScreenShotOnNextFrame;
    private int picCounter = 0;

    private void Awake()
    {
        instance = this;
        captureCamera = gameObject.GetComponent<Camera>(); //the camera taking pictures (1st person)

        //Create the pohoto directory
        DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath + "/" + "ScreenshotFolder"); //create a folder named screenshotfolder (not to assets)
        if (!dirInf.Exists) {
            Debug.Log("Creating subdirectory");
            dirInf.Create(); //if the directory doesnt exist, create it
        }
    }

    private void OnPostRender()
    {
        if(takeScreenShotOnNextFrame)
        {
            takeScreenShotOnNextFrame = false;
            RenderTexture renderTexture = captureCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            picCounter++;

            byte[] byteArray = renderResult.EncodeToPNG();

            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/ScreenshotFolder/" + picCounter + "CameraScreenshot.png", byteArray); //path to screenshot folder created to appdata folders
            Debug.Log("Saved " + picCounter + " CameraScreenshot.png");

            RenderTexture.ReleaseTemporary(renderTexture);
            captureCamera.targetTexture = null;
        }
    }

    private void TakeScreenshot(int width, int height)
    {
        captureCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenShotOnNextFrame = true;
    }

    public static void TakeScreenshot_Static(int width, int height) //function used in camera script to take screenshots
    {
        instance.TakeScreenshot(width, height);
    }
}
