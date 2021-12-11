using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.BulletDecals.Scripts.Bullets;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class UIManager : MonoBehaviour
{
    public static PointerEventData pointerEventData;

    [SerializeField]
    private TextMeshProUGUI titleTMPUGUI;

    [SerializeField]
    private Button toggleSessionButton, toggleBulletMarkImageButton;

    [SerializeField]
    private Slider isoSlider, shutterSlider, zoomSlider;

    [SerializeField]
    private TextMeshProUGUI shotDetectionStatusTMPUGUI, shotCountTMPUGUI, isoSliderTMPUGUI, shutterSliderTMPUGUI, zoomSliderTMPUGUI, cameraFPSTMPUGUI, inputImageFPSTMPUGUI, processingFPSTMPUGUI;

    private float _timerStart;
    
    [SerializeField]
    private List<TextMeshProUGUI> shotCountListTMPUGUI; 
    // Change crosshairContainer variable name to panlLanesContainer and delete old CrosshairContainer GameObject in all of the the scenes
    [SerializeField]
    private GameObject bulletMarkImage, canvas, targetNotFound, autoAdjustToggleContainer, cameraSelectionPanel, crosshairContainer, lockTargetButton;

    public static GameObject zoomInButton, zoomOutButton;

    public GameObject _instructionPanel;
    public List<Image> crosshair = new List<Image>();

    public Camera MainCamera;

    public List<TextMeshProUGUI> crosshairCoords = new List<TextMeshProUGUI>(), meanCoords = new List<TextMeshProUGUI>(), shotOffset = new List<TextMeshProUGUI>();
    public TextMeshProUGUI shotCoords;

    private bool didInitializeSliderValues = false;

    public static bool isZeroed;

    private float cameraFPS, inputImageFPS, processingFPS;

    public static List<bool> isUsingZero = new List<bool>(), isLaneZeroed = new List<bool>();

    private bool isBulletMarkImageEnabled = true, timerStarted = false;

    private static List<Vector2> crosshairPos = new List<Vector2>();

    [SerializeField]
    private int lanes;

    private List<int> shotCount = new List<int>();
    public static List<Vector2> Sum = new List<Vector2>();
    public static List<Vector2> Average = new List<Vector2>();
    public static List<Vector2> Offset = new List<Vector2>();
    public TextMeshProUGUI HitDetectionDelay, TogglesessionText;
    public static bool isLaserPointed;
    public static float shotDelay;



    public static bool isSession, isDisplayingHelp, didHideAllChildren, isDisplayingTargets, isDisplayingInstructions = false;

    public Color StartColor;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name.Contains("SubMenu"))
        {
            //shotCountListTMPUGUI.Clear();
            Offset.Clear();
            Sum.Clear();
            Average.Clear();
            crosshairPos.Clear();
            isLaneZeroed.Clear();
            isUsingZero.Clear();
        }
        shotCount.Clear();
        isLaserPointed = false;
        shotDelay = 0f;
        _timerStart = 3.5f;
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif

    }

    private void Start()
    {
        pointerEventData = new PointerEventData(EventSystem.current);

        toggleSessionButton.onClick.AddListener(ToggleSession);
#if UNITY_ANDROID


#if UNITY_ANDROID
        zoomInButton = GameObject.FindGameObjectWithTag("ZoomInButton");
        zoomOutButton = GameObject.FindGameObjectWithTag("ZoomOutButton");

        shutterSlider.value = AndroidPluginManager.androidCameraInterface.Call<int>("getShutterSpeed");
        isoSlider.value = AndroidPluginManager.androidCameraInterface.Call<int>("getISO");
        zoomSlider.value = AndroidPluginManager.androidCameraInterface.Call<int>("getZoom");
#endif

#endif

        zoomSliderTMPUGUI.text = "Zoom: " + zoomSlider.value;
        SetSlidersText();

        didInitializeSliderValues = true;
        if (SceneManager.GetActiveScene().name.Contains("SubMenu"))
        {
            isZeroed = false;
            for (int i = 0; i < lanes; i++)
            {

                Offset.Add(new Vector2(0f, 0f));
                Sum.Add(new Vector2(0f, 0f));
                Average.Add(new Vector2(0f, 0f));
                crosshairPos.Add(new Vector2(0f, 0f));
                isUsingZero.Add(false);
                isLaneZeroed.Add(false);

            }
        }
        for (int i = 0; i < lanes; i++)
        {
            //shotCountListTMPUGUI.Add(new TextMeshProUGUI());
            shotCount.Add(0);
        }
    }

    void Update()
    {

        StartCoroutine(TitleFadeAnimation());
        
               /* if (timerStarted)
                {
                    TogglesessionText.fontSize = 90;
                    TogglesessionText.text = _timerStart.ToString("F0");
                    _timerStart -= Time.deltaTime;
                    if(_timerStart < 0.5f)
                    {
                        toggleSessionButton.image.color = StartColor;
                        TogglesessionText.text = "GO!";
                    }
                    if (_timerStart < 0.0f)
                    {
        #if UNITY_ANDROID
                        AndroidPluginManager.androidCameraInterface.Call("setSessionEnabled", false);
        #endif
                        isSession = true;

                        toggleSessionButton.gameObject.SetActive(false);
                    }

                        toggleSessionButton.gameObject.SetActive(false);
                        if (isDisplayingInstructions)
                        {
                            isDisplayingInstructions = false;
                            _instructionPanel.SetActive(false);
                        }
                    timerStarted = false;
                    }              
                
                */        
#if UNITY_ANDROID
        for (int i = 0; i < lanes; i++)
        {
            crosshairPos[i] = crosshair[i].rectTransform.position;
            crosshairCoords[i].text = (crosshairPos[i]).ToString();
            shotOffset[i].text = Offset[i].ToString();

        }





/*
        try
        {
            string jsonStringCamera = AndroidPluginManager.androidCameraInterface.Call<string>("getFrameDataString");
            JSONObject jsonObjectCamera = new JSONObject(jsonStringCamera);
            cameraFPS = float.Parse(jsonObjectCamera["cameraFPS"].ToString());
            inputImageFPS = float.Parse(jsonObjectCamera["inputImageFPS"].ToString());
            processingFPS = float.Parse(jsonObjectCamera["processingFPS"].ToString());

            cameraFPSTMPUGUI.text = cameraFPS.ToString();
            inputImageFPSTMPUGUI.text = inputImageFPS.ToString();
            processingFPSTMPUGUI.text = processingFPS.ToString();

        }
        catch (Exception error)
        {
            Debug.Log(error);
        }
        */

        if (AndroidPluginManager.didSelectCamera && AndroidPluginManager.didStartCamera)
        {
#endif

        if (SceneManager.GetActiveScene().name == "CameraCalibration")
        {
            if (!didHideAllChildren)
            {
                int index = 0;

                foreach (Transform eachChild in cameraSelectionPanel.transform)
                {
                    cameraSelectionPanel.transform.GetChild(index).gameObject.SetActive(false);

                    index++;
                }

                didHideAllChildren = true;
#if UNITY_ANDROID
                    DisplayHelp();
#endif
                lockTargetButton.SetActive(true);
            }

        }
        else if (SceneManager.GetActiveScene().name.Contains("SubMenu"))
        {
            if (isUsingZero.Contains(false))
            {
                isZeroed = false;
            }
            else
            {
                isZeroed = true;
            }

            if (isZeroed && crosshairContainer.activeSelf)
            {
                foreach (Transform canvasElement in canvas.transform)
                {
                    if (canvasElement.CompareTag("SceneButton"))
                    {
                        canvasElement.gameObject.SetActive(true);
                    }
                }
            }

            if (crosshairContainer.activeSelf)
            {
                isSession = true;

            }
            else
            {
                isSession = false;
            }
        }
#if UNITY_ANDROID
        }


        if (SceneManager.GetActiveScene().name.Contains("SubMenu") || (SceneManager.GetActiveScene().name.Contains("Identify1")))
        {
            //HitDetectionDelay.text = referencenumber.ToString();
            if (isLaserPointed)
            {
                //timer stops
                shotDelay += Time.deltaTime;
            }
            else
            {
                //timer starts ticking
                HitDetectionDelay.text = shotDelay.ToString("F3");

            }

        }


        if (AndroidPluginManager.didStartCamera)
        {
#endif
        if (isSession)
        {
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            foreach (var result in raycastResults)
            {
                if (pointerEventData.position != Vector2.zero)
                {
                    int laneIndex = 0;
                    switch (result.gameObject.tag)
                    {
                        case "Lane1":
                            laneIndex = 0;
                            CalculateCoordinateOffset(laneIndex, result);
                            break;
                        case "Lane2":
                            laneIndex = 1;
                            CalculateCoordinateOffset(laneIndex, result);
                            break;
                        case "Lane3":
                            laneIndex = 2;
                            CalculateCoordinateOffset(laneIndex, result);
                            break;
                        case "Lane4":
                            laneIndex = 3;
                            CalculateCoordinateOffset(laneIndex, result);
                            break;
                        case "Lane5":
                            laneIndex = 4;
                            CalculateCoordinateOffset(laneIndex, result);
                            break;
                        default:
                            InstantiateBulletMarkImage();
                            Gun.didDetectLaser = true;
                            break;
                    }

                    pointerEventData.position = new Vector2(0, 0);
                }
            }

        }

        for (int i = 0; i < lanes; i++)
        {
            shotCountListTMPUGUI[i].text = shotCount[i].ToString();
        }


#if UNITY_ANDROID
            string jsonString = AndroidPluginManager.androidCameraInterface.Call<string>("getTargets");
            JSONObject jsonObject = new JSONObject(jsonString);

            if (jsonObject.Count > 0)
            {
                targetNotFound.SetActive(false);
            }
            else
            {
                targetNotFound.SetActive(true);
            }
#endif

#if UNITY_EDITOR
        if (lanes == 1 && Gun.didDetectLaser)
            ++shotCount[0];
#endif

#if UNITY_ANDROID
        shotDetectionStatusTMPUGUI.text = AndroidPluginManager.androidCameraInterface.Call<string>("getShotDetectionStatus");

            if (AndroidPluginManager.androidCameraInterface.Call<bool>("isAutoAdjusting"))
            {
                isoSlider.value = AndroidPluginManager.androidCameraInterface.Call<int>("getISO");
                shutterSlider.value = AndroidPluginManager.androidCameraInterface.Call<int>("getShutterSpeed");

                SetSlidersText();
            }
        }
#endif
    }


#if UNITY_ANDROID
    public void RGBSelected()
    {
        AndroidPluginManager.didChooseIRCamera = false;
        AndroidPluginManager.androidCameraInterface.Call("setIRRequired", false);
    }

    public void IRSelected()
    {
        AndroidPluginManager.didChooseIRCamera = true;
        AndroidPluginManager.androidCameraInterface.Call("setIRRequired", true);
    }

    public void DoneCameraSelect()
    {
        AndroidPluginManager.didSelectCamera = true;
    }
#endif
    public void ToggleSession()
    {
#if UNITY_ANDROID
        //AndroidPluginManager.androidCameraInterface.Call("setSessionEnabled", false);
#endif
        isSession = true;

        toggleSessionButton.gameObject.SetActive(false);
        if (isDisplayingInstructions)
        {
            isDisplayingInstructions = false;
            _instructionPanel.SetActive(false);
        }

        //timer for 3 seconds
        //if (timerStarted == false)
        //   timerStarted = true;
        /*if (_timerStart == 0f)
        {
            timerStarted = false;
            isSession = true;

            toggleSessionButton.gameObject.SetActive(false);
        }
        if (isDisplayingInstructions)
        {
            isDisplayingInstructions = false;
            _instructionPanel.SetActive(false);
        }*/
    }


    public void ClearSession()
    {
#if UNITY_ANDROID
        AndroidPluginManager.androidCameraInterface.Call("clearData");
        AndroidPluginManager.androidCameraInterface.Call("setSessionEnabled", true);
#endif
        isSession = false;
        Addressables.LoadSceneAsync("Levels/" + SceneManager.GetActiveScene().name + ".unity", LoadSceneMode.Single).Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
                Debug.Log("Successfully loaded scene.");
        };
    }


    public void ToggleBulletMarkImage()
    {
        if (isBulletMarkImageEnabled)
        {
            isBulletMarkImageEnabled = false;
        }
        else
        {
            isBulletMarkImageEnabled = true;
        }
    }
#if UNITY_ANDROID
    public void AutoAdjustingHandler()
    {
        if (AndroidPluginManager.androidCameraInterface.Call<bool>("isAutoAdjusting"))
        {
            AndroidPluginManager.androidCameraInterface.Call("setAutoAdjustCameraExposure", false);
        }
        else
        {
            AndroidPluginManager.androidCameraInterface.Call("setAutoAdjustCameraExposure", true);
        }
    }

    public void SetSlidersValueManual()
    {
        if (!didInitializeSliderValues)
        {
            return;
        }

        if (!AndroidPluginManager.androidCameraInterface.Call<bool>("isAutoAdjusting"))
        {
            SetSlidersText();

            AndroidPluginManager.androidCameraInterface.Call("setISO", (int)isoSlider.value);
            AndroidPluginManager.androidCameraInterface.Call("setShutterSpeed", (int)shutterSlider.value);
        }

        zoomSliderTMPUGUI.text = "Zoom: " + zoomSlider.value;

        AndroidPluginManager.androidCameraInterface.Call("setZoom", (int)zoomSlider.value);
    }


    public void DisplayHelp()
    {
        if (isDisplayingHelp)
        {
            AndroidPluginManager.androidCameraInterface.Call("setCameraSize", 0, 0, 0);

            isDisplayingHelp = false;
            autoAdjustToggleContainer.SetActive(false);
            toggleBulletMarkImageButton.gameObject.SetActive(false);
        }
        else
        {
            AndroidPluginManager.androidCameraInterface.Call("setCameraSize", AndroidPluginManager.screenWidth / 4, AndroidPluginManager.screenHeight / 4, 0);

            isDisplayingHelp = true;
            autoAdjustToggleContainer.SetActive(true);
            toggleBulletMarkImageButton.gameObject.SetActive(true);
        }
    }

    public void LockTarget()
    {
        if (!targetNotFound.activeSelf)
        {
            AndroidPluginManager.androidCameraInterface.Call("setTargetDetection", false);

            cameraSelectionPanel.SetActive(false);
            lockTargetButton.SetActive(false);
            DisplayHelp();

            SceneHandler.scene = "MainMenu";
        }
    }
#endif
    void CalculateCoordinateOffset(int laneIndex, RaycastResult result)
    {
        Vector2 pos = result.screenPosition;
        if (!isUsingZero[laneIndex])
        {

            crosshairPos[laneIndex] = crosshair[laneIndex].rectTransform.position;
            crosshairCoords[laneIndex].text = (crosshairPos[laneIndex]).ToString();

            shotCoords.text = pos.ToString();

            Sum[laneIndex] = Sum[laneIndex] + pos;
            Average[laneIndex] = Sum[laneIndex] / (++shotCount[laneIndex]);
            meanCoords[laneIndex].text = (Average[laneIndex]).ToString();
            Offset[laneIndex] = crosshairPos[laneIndex] - Average[laneIndex];
            shotOffset[laneIndex].text = Offset[laneIndex].ToString();


        }
        else
        {
            Bullet.laserCoordinates = pos + Offset[laneIndex];
            ++shotCount[laneIndex];
        }


        if (shotCount[laneIndex] > 2 && !isUsingZero[laneIndex])
        {
            foreach (Transform canvasElement in canvas.transform)
            {
                if (canvasElement.CompareTag("ApplyResetLane" + (laneIndex + 1)))
                {
                    canvasElement.gameObject.SetActive(true);
                }
            }
            isUsingZero[laneIndex] = true;

        }

        InstantiateBulletMarkImage();

        Gun.didDetectLaser = true;
    }
#if UNITY_ANDROID

    private static void StopHitDelayCalculator()
    {
        isLaserPointed = false;
    }
#endif
    private void InstantiateBulletMarkImage()
    {
        if (isBulletMarkImageEnabled && isSession)
        {
            GameObject tempBulletMarkImage = Instantiate(bulletMarkImage);
            tempBulletMarkImage.transform.SetParent(canvas.transform);
            tempBulletMarkImage.transform.position = Bullet.laserCoordinates;
        }
    }


    private void SetSlidersText()
    {
        isoSliderTMPUGUI.text = "ISO: " + isoSlider.value;
        shutterSliderTMPUGUI.text = "Shutter: " + shutterSlider.value;
    }
            
    private IEnumerator TitleFadeAnimation()
    {
        yield return new WaitForSecondsRealtime(3);
        titleTMPUGUI.CrossFadeAlpha(0.0f, 0.5f, false);
    }

    public void EndZero(int lane)
    {
        if (isUsingZero[lane])
        {
            isLaneZeroed[lane] = true;
        }
        isUsingZero[lane] = true;



    }
    public void ResetZero(int lane)
    {
        Offset[lane] = new Vector2(0f, 0f);
        Sum[lane] = new Vector2(0f, 0f);
        shotOffset[lane].text = Offset[lane].ToString();
        shotCount[lane] = 0;
        isUsingZero[lane] = false;
        isLaneZeroed[lane] = false;
        foreach (Transform canvasElement in canvas.transform)
        {
            if (canvasElement.CompareTag("ApplyResetLane" + (lane + 1)))
            {
                canvasElement.gameObject.SetActive(false);
            }
            if (canvasElement.CompareTag("SceneButton"))
            {
                canvasElement.gameObject.SetActive(false);
            }
        }

    }

    public void DisplayInstructions()
    {

        if (isDisplayingInstructions)
        {
            isDisplayingInstructions = false;
            _instructionPanel.SetActive(false);
        }

        else
        {
            _instructionPanel.SetActive(true);
            isDisplayingInstructions = true;
        }
    }
}
