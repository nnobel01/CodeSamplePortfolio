using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StageController : MonoBehaviour
{

    public float timeRemaining = 3;
    public float stringTime = 0;
    float freeString;
    public float stringScore;

    GameObject bulletmark;


    public GameObject sessionButton, FinalTime, TimeText;

    [SerializeField]
    private GameObject _BulletFX;
    [SerializeField]
    private GameObject _stopPlate;

    public float first = 0, second = 0, third = 0, fourth = 0, fifth = 0;





    public bool timeIsRunning = false;
    public bool stringIsRunning = false;
    public bool stopPlateTriggered = false;


    public TextMeshProUGUI timeText;
    public TextMeshProUGUI stringTimeText;

    //Audio goes here
    [SerializeField]
    AudioClip gunFire;
    [SerializeField]
    AudioClip metalPing;
    [SerializeField]
    AudioClip buzzer;




    private void Start()
    {
        FinalTime.SetActive(false);
    }

    private void Update()
    {
        if (UIManager.isSession == true)
        {
            StartTime();
        }
        if(first != 0 && second != 0 && third != 0 && fourth != 0 && fifth != 0 && stringScore == 0)
        {
              
           CalculateStringScore();
            Debug.Log(stringScore);
            TimeText.SetActive(false);
            FinalTime.SetActive(true);
            stringTimeText.text = "Final Time: " + stringScore.ToString("##.##");

        } 




    }
    void DisplayTime(float timetoDisplay)
    {
        float minutes = Mathf.FloorToInt(timetoDisplay / 60);
        float seconds = Mathf.FloorToInt(timetoDisplay % 60);
        float milliseconds = (timetoDisplay % 1) * 1000;

        timeText.text = string.Format("{0:00} : {1:00} : {2:000}", minutes, seconds, milliseconds);

    }
    public void StartTime()
    {

        timeIsRunning = true;
        stringIsRunning = false;
       


        if (timeRemaining > 0 && timeIsRunning == true)
        {
            timeRemaining -= Time.deltaTime;

            DisplayTime(timeRemaining);
        }
        if (timeRemaining <= 0 && timeIsRunning == true)
        {
            timeIsRunning = false;
            stringIsRunning = true;
            Debug.Log("String is running");

            //Buzzer Audio Plays
           // AudioSource.PlayClipAtPoint(buzzer, Camera.main.transform.position);



        }
        if (stringIsRunning == true && stopPlateTriggered == false)
        {
            stringTime += Time.deltaTime;

            DisplayTime(stringTime);


            if (_stopPlate.transform.Find("BulletMark(Clone)"))
            {
                bulletmark = _stopPlate.transform.Find("BulletMark(Clone)").gameObject;
                Debug.Log("Stop Plate Hit");
                stopPlateTriggered = true;
                UIManager.isSession = false;
                sessionButton.SetActive(true);
                timeRemaining = 3;
                Destroy(bulletmark);
                
                
            }

            if (stringTime >= 30 || stopPlateTriggered == true)
            {
                stringIsRunning = false;
                if (first == 0)
                {
                    first = stringTime;
                    Debug.Log("First String is: " + first);
                    UIManager.isSession = false;
                    stringTime = 0;
                    stopPlateTriggered = false;
                }
                else if (first != 0 && second == 0)
                {
                    second = stringTime;
                    Debug.Log("Second String is: " + second);
                    UIManager.isSession = false;
                    stringTime = 0;
                    stopPlateTriggered = false;
                }
                else if (first != 0 && second != 0 && third == 0)
                {
                    third = stringTime;
                    Debug.Log("Third String is: " + third);
                    UIManager.isSession = false;
                    stringTime = 0;
                    stopPlateTriggered = false;
                }
                else if (first != 0 && second != 0 && third != 0 && fourth == 0)
                {
                    fourth = stringTime;
                    Debug.Log("Fourth String is: " + fourth);
                    UIManager.isSession = false;
                    stringTime = 0;
                    stopPlateTriggered = false;
                }
                else if (first != 0 && second != 0 && third != 0 && fourth != 0 && fifth == 0)
                {
               
                    fifth = stringTime;
                    Debug.Log("Fifth String is: " + fifth);
                    UIManager.isSession = false;
                    stringTime = 0;
                    stopPlateTriggered = false;
                }

            }

        }

    }


    public float CalculateStringScore()
    {
        freeString = Mathf.Max(first, second, third, fourth, fifth);
        stringScore = (first + second + third + fourth + fifth) - freeString;

        return stringScore;
    }

    public void NextStage()
    {
        stringScore = 0;
        first = 0;
        second = 0;
        third = 0;
        fourth = 0;
        fifth = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void ResetStage()
    {
        Scene curScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("curScene");
    }

}
