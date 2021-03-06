﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class movementController : MonoBehaviour
{
    public UDPClient client;
 
    public GameObject ProfileManager;

    private string nickName;
    private string rotate;
    private int toRotate;
    public Camera mainCamera;

    public float walkSpeed;
    public float runSpeed;
    public float turnSpeed;
    
    public string inputFront;
    public string inputBack;
    public string inputLeft;
    public string inputRight;
    public string inputAttack;
    public string inputShield;
    public string inputInventory;

    public Vector3 jumpSpeed;
    public float sensitivity;

    Animation animations;
    CapsuleCollider playerCollider;
    private bool GUI;

    private profileManager profile;

    private bool inMovement;

    Dictionary<string, Action<string[]>> opcodesPtr;

    void Start()
    {
        initializeOpcodes();

        GUI = false;
        client = Instantiate(client);
        animations = gameObject.GetComponent<Animation>();
        playerCollider = gameObject.GetComponent<CapsuleCollider>();

        profile = ProfileManager.GetComponent<profileManager>();
        ProfileManager.SetActive(GUI);

        inMovement = false;

        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
    }

    void Update()
    {
        string toExecute = this.client.ReceiveData();
        if (toExecute != null)
        {
            string[] isValidCommand = toExecute.Split(':');
            Debug.Log(isValidCommand[0]);
            opcodesPtr[isValidCommand[0]](isValidCommand);
            toExecute = null;
        }

        if (Input.GetKey(inputFront))
        {
            if (rotate != inputFront)
            {
                rotate = inputFront;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            mainCamera.transform.Translate(0, 0, walkSpeed * Time.deltaTime);
            transform.Translate(0,0, walkSpeed * Time.deltaTime);
            animations.Play("walk");
        } 
        if (Input.GetKey(inputBack))
        {
            if (rotate != inputBack)
            {
                rotate = inputBack;
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            mainCamera.transform.Translate(0, 0, -walkSpeed * Time.deltaTime);
            transform.Translate(0, 0, walkSpeed * Time.deltaTime);
            animations.Play("walk");
        }
        if (Input.GetKey(inputLeft))
        {
            if (rotate != inputLeft)
            {
                rotate = inputLeft;
                transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            mainCamera.transform.Translate(-walkSpeed * Time.deltaTime, 0, 0);
            transform.Translate(0, 0, walkSpeed * Time.deltaTime);
            animations.Play("walk");
        }
        if (Input.GetKey(inputRight))
        {
            if (rotate != inputRight)
            {
                rotate = inputRight;
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            mainCamera.transform.Translate(walkSpeed * Time.deltaTime, 0, 0);
            transform.Translate(0, 0, walkSpeed * Time.deltaTime);
            animations.Play("walk");
        }

        if (Input.GetKey(inputAttack))
        {
            animations.Play("attack");
        }

        if (Input.GetKey(inputShield))
        {
            animations.Play("resist");
        }

        if (Input.GetKeyDown(inputInventory))
        {
            GUI = !GUI;
            ProfileManager.SetActive(GUI);
        }
    }

    private void manageOPcodes()
    {
    }

    private void FixedUpdate()
    {
        /*float rotateHorizontal = Input.GetAxis("Mouse X");
        float rotateVertical = Input.GetAxis("Mouse Y");

        mainCamera.transform.RotateAround(mainCamera.transform.position, -Vector3.up, rotateHorizontal * sensitivity);
        mainCamera.transform.RotateAround(Vector3.zero, mainCamera.transform.right, rotateVertical * sensitivity);*/
    }

    private void sendPosition(string[] chainList)
    {
        string query = "S_GETCURRENTPOSITION:";

        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        query += client.nickName + ':' + x + ':' + y + ':' + z;

        client.SendData(query);
    }

    private void getProfile(string[] chainList)
    {
        profile.displayInventory(chainList);
    }

    private void loginData(string[] chainList)
    {
        transform.position = new Vector3(float.Parse(chainList[2], System.Globalization.CultureInfo.InvariantCulture), float.Parse(chainList[3], System.Globalization.CultureInfo.InvariantCulture), float.Parse(chainList[4], System.Globalization.CultureInfo.InvariantCulture));
        mainCamera.transform.position = new Vector3(float.Parse(chainList[2], System.Globalization.CultureInfo.InvariantCulture) + 1, float.Parse(chainList[3], System.Globalization.CultureInfo.InvariantCulture) + 2, float.Parse(chainList[4], System.Globalization.CultureInfo.InvariantCulture) - 2);
    }

    private void correctPosition(string[] chainList)
    {
        if (chainList[1] == "z")
        {
            transform.position = new Vector3((float)transform.position.x, (float)transform.position.y, float.Parse(chainList[2]));
            mainCamera.transform.position = new Vector3((float)mainCamera.transform.position.x, (float)mainCamera.transform.position.y, float.Parse(chainList[2], System.Globalization.CultureInfo.InvariantCulture) - 2);
        }
        if (chainList[1] == "x")
        {
            transform.position = new Vector3(float.Parse(chainList[2]), (float)transform.position.y, (float)transform.position.z);
            mainCamera.transform.position = new Vector3(float.Parse(chainList[2], System.Globalization.CultureInfo.InvariantCulture) + 1, (float)mainCamera.transform.position.y, (float)mainCamera.transform.position.z);
        }
    }

    private void initializeOpcodes()
    {
        opcodesPtr = new Dictionary<string, Action<string[]>>();
        opcodesPtr["C_SENDCURRENTPOSITION"] = sendPosition;
        opcodesPtr["C_GETPROFILE"] = getProfile;
        opcodesPtr["C_LOGIN_DATA"] = loginData;
        opcodesPtr["C_CORRECTPOSITION"] = correctPosition;
    }
}