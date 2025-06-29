using System;
using TMPro;
using UnityEngine;

public class ToggleFloor : MonoBehaviour
{
    [SerializeField]
    GameObject upFloorObject;

    [SerializeField]
    GameObject downFloorObject;

    int currentFloor = -1;
    int targetFloor = -1;


    [SerializeField]
    public TMP_Text textUPstairUPop;

    [SerializeField]
    public TMP_Text textDOWNstairUPop;



    [SerializeField]
    public TMP_Text textUPstairDWop;

    [SerializeField]
    public TMP_Text textDOWNstairDWop;

    public void Start()
    {
       
    }
    public void writeCurrentFloor(int currentFloor)
    {
        this.currentFloor = currentFloor;
        if(targetFloor != -1)
        {
            if (currentFloor < targetFloor)
            {
                setDownfloorOFF();
                setUpfloorON(targetFloor.ToString(), currentFloor.ToString());
            }
            else if (currentFloor > targetFloor)
            {
                setUpfloorOFF();
                setDownfloorON(currentFloor.ToString(), targetFloor.ToString());
            }
            else
            {
                setDownfloorOFF();
                setUpfloorOFF();
            }
        }
    }
    public void writeTragetFloor(int targetFloor)
    {
        this.targetFloor = targetFloor;
        Console.WriteLine(targetFloor.ToString());
        if (currentFloor < targetFloor)
        {
            setDownfloorOFF();
            setUpfloorON(targetFloor.ToString(), currentFloor.ToString());
        }
        else if(currentFloor > targetFloor)
        {
            setUpfloorOFF();
            setDownfloorON(currentFloor.ToString(), targetFloor.ToString());
        }
    }

    void setUpfloorON(string upStair , string downStair) {
       
        upFloorObject.SetActive(true);
        textUPstairUPop.text = upStair;
        textDOWNstairUPop.text = downStair;
        
    }

    void setUpfloorOFF(){
        upFloorObject.SetActive(false);
    }

    void setDownfloorON(string upStair, string downStair)
    {
        downFloorObject.SetActive(true);
        textUPstairDWop.text = upStair;
        textDOWNstairDWop.text = downStair;
       
    }

    void setDownfloorOFF(){
        downFloorObject.SetActive(false);
    }





}
