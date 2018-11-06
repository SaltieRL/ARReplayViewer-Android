//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class HelloARController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject AndyPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a feature point.
        /// </summary>
        public GameObject AndyPointPrefab;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject SearchingForPlaneUI;

        /// <summary>
        /// The rotation in degrees need to apply to model when the Andy model is placed.
        /// </summary>
        private const float k_ModelRotation = 180.0f;

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        //private static string BASE_URL = "https://calculated.gg/api/replay";
        private static string BASE_URL = "https://calculated.gg/api/replay/EACC3A5811E8E0AE177575BC6D034FCF/positions";

        // Settable parameters
        public GameObject prefab;
        public GameObject field;
        public GameObject ball;
        public GameObject namePrefab;
        public float scaleFactor = 1000f;
        public Material orangeCar;
        public Material blueCar;
        public GameObject planeGenerator;

        // Instance objects
        private GameObject ballObject;
        private GameObject fieldObject;
        private RootObject gameData;
        private float currentTime = 0f;
        private int currentFrame = 0;
        private List<GameObject> cars = new List<GameObject>();
        private List<GameObject> names = new List<GameObject>();
        private float hSliderValue = 1000f;
        public void Start()
        {
            BASE_URL = StaticReplayScript.URL;
            WWW req = new WWW(BASE_URL);
            StartCoroutine("GetData", req);
            //dynamic dyn = JsonUtility.FromJson(res);

        }


        IEnumerator GetData(WWW req)
        {

            yield return req;
            if (req.error == null)
            {
                string text = req.text;
                Debug.Log(text.Substring(0, 100));
                RootObject r = JsonConvert.DeserializeObject<RootObject>(text);
                Debug.Log(r.id);
                Debug.Log(r.colors[0]);
                Debug.Log(r.frames[0][0]);
                Debug.Log(r.ball[0][0]);
                Debug.Log(r.players[0][0][0]);
                this.gameData = r;
            }
            else
            {
                Debug.Log(req.error);
            }
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _UpdateApplicationLifecycle();

            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<DetectedPlane>(m_AllPlanes);
            bool showSearchingUI = true;
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    showSearchingUI = false;
                    break;
                }
            }

            SearchingForPlaneUI.SetActive(showSearchingUI);
            SearchingForPlaneUI.GetComponentInChildren<UnityEngine.UI.Text>().text = StaticReplayScript.URL;

            // If the player has not touched the screen, we are done with this update.
            if (fieldObject == null)
            {
                Touch touch;
                if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
                {
                    return;
                }

                // Raycast against the location the player touched to search for planes.
                TrackableHit hit;
                TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                                                  TrackableHitFlags.FeaturePointWithSurfaceNormal;

                if ((fieldObject == null) && (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit)))
                {
                    // Use hit pose and camera pose to check if hittest is from the
                    // back of the plane, if it is, no need to create the anchor.
                    if ((hit.Trackable is DetectedPlane) &&
                        Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                            hit.Pose.rotation*Vector3.up) < 0)
                    {
                        Debug.Log("Hit at back of the current DetectedPlane");
                    }
                    else
                    {
                        // Choose the Andy model for the Trackable that got hit.

                        // Instantiate Andy model at the hit pose.
                        fieldObject = Instantiate(this.field, hit.Pose.position, hit.Pose.rotation);

                        // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
                        fieldObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                        // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                        // world evolves.
                        var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                        var emptyGameObject = new GameObject("Field/Car Anchor");
                        // Make Andy model a child of the anchor.
                        emptyGameObject.transform.parent = anchor.transform;
                        fieldObject.transform.parent = emptyGameObject.transform;

                        fieldObject.transform.localScale = new Vector3(100/scaleFactor, 100 / scaleFactor, 100 / scaleFactor);

                        for (int i = 0; i < gameData.players.Count; i++)
                        {
                            float x = (float) (double) gameData.players[i][0][0]/scaleFactor;
                            float y = (float) (double) gameData.players[i][0][2]/scaleFactor;
                            float z = (float) (double) gameData.players[i][0][1]/scaleFactor;
                            var car = Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                            car.transform.parent = emptyGameObject.transform;
                            car.transform.localPosition = new Vector3(x, y, z);
                            car.GetComponent<Renderer>().material = gameData.colors[i] == 0 ? blueCar : orangeCar;
                            cars.Add(car);


                            var name = Instantiate(namePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                            name.GetComponent<TextMesh>().text = gameData.names[i];
                            name.transform.parent = fieldObject.transform;
                            name.transform.localPosition = new Vector3(x, y + 1.0f, z);
                            names.Add(name); // needed so we can look at camera all the time
                        }
                        ballObject = Instantiate(ball, new Vector3(0f, 0f, 0f), Quaternion.identity);
                        ballObject.transform.parent = fieldObject.transform;
                        ballObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                        currentTime = (float) gameData.frames[0][2];
                    }
                }
            }
            if (fieldObject != null)
            {
                if (planeGenerator.GetComponent<DetectedPlaneGenerator>().RendererEnabled)
                {
                    planeGenerator.GetComponent<DetectedPlaneGenerator>().RendererEnabled = false;
                    planeGenerator.GetComponent<DetectedPlaneGenerator>().DestroyPlanes();
                }
                for (int i = 0; i < cars.Count; i++)
                {
                    var frameData = gameData.players[i][currentFrame];
                    float x = (float)(double)frameData[0] / scaleFactor;
                    float y = (float)(double)frameData[2] / scaleFactor;
                    float z = (float)(double)frameData[1] / scaleFactor;

                    var rotX = (double) frameData[3] * 180 / 3.14159265;
                    var rotY = (double) frameData[4] * 180 / 3.14159265;
                    var rotZ = (double) frameData[5] * 180 / 3.14159265;

                    cars[i].transform.localPosition = new Vector3(x, y, z);
                    cars[i].transform.localEulerAngles = new Vector3((float) rotX, (float) rotY, (float) rotZ);
                    names[i].transform.localPosition = new Vector3(x, y + 0.5f, z);
                    names[i].transform.LookAt(FirstPersonCamera.transform);
                    names[i].transform.Rotate(Vector3.up - new Vector3(0, 180, 0));
                }
                float ballx = (float)gameData.ball[currentFrame][0] / scaleFactor;
                float bally = (float)gameData.ball[currentFrame][2] / scaleFactor;
                float ballz = (float)gameData.ball[currentFrame][1] / scaleFactor;
                ballObject.transform.localPosition = new Vector3(ballx, bally, ballz);

                // Time management
                currentTime += Time.deltaTime;

                var currentFrameTime = gameData.frames[currentFrame][2];
                var nextFrameTime = gameData.frames[currentFrame + 1][2];
                Debug.Log(currentFrameTime.ToString());
                if (currentTime > nextFrameTime)
                {
                    currentFrame += 1;
                }
            }
        }



        public void OnGUI()
        {
            var zoomOut = GUI.Button(new Rect(25 + 150, 25, 30, 30), "-");
            var zoomIn = GUI.Button(new Rect(25 + 150 + 50, 25, 30, 30), "+");
            if (zoomOut)
            {
                scaleFactor += 100;
            }
            else if (zoomIn)
            {
                scaleFactor -= 100;
            }

            if (gameData != null)
            {
                currentTime = GUI.HorizontalSlider(new Rect(25 + 150, 35, 200, 30), currentTime, (float)gameData.frames[0][2], (float) gameData.frames[gameData.frames.Count - 1][2]);
               
            }


            // Button
            var back = GUI.Button(new Rect(25, 25, 100, 30), "Back to QR");
            if (back)
            {
                SceneManager.LoadScene("QRCode");
            }
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}

[System.Serializable]
public class RootObject
{
    public List<List<double>> ball;
    public List<List<List<object>>> players;
    public List<int> colors;
    public List<string> names;
    public List<List<double>> frames;
    public string id;
}