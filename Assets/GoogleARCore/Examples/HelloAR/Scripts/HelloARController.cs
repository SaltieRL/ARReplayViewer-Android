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
using UnityEditor;
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
        private GameObject rootObject;
        private float currentTime = 0f;
        private int currentFrame = 0;
        private List<GameObject> cars = new List<GameObject>();
        private List<GameObject> names = new List<GameObject>();
        private float hSliderValue = 1000f;
        private float localScale = 15f;


        private string dataMsg;
        private string protoMsg;

        private void Spawn(Vector3 position, Quaternion rotation, Transform parent)
        {
            var gameData = StaticReplayScript.gameData;
            // Choose the Andy model for the Trackable that got hit.
            // Instantiate Andy model at the hit pose.
            fieldObject = Instantiate(this.field, position, rotation);

            // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
            fieldObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

            // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
            // world evolves.

            rootObject = new GameObject("Field/Car Anchor");
            // Make Andy model a child of the anchor.
            rootObject.transform.parent = parent;
            rootObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            rootObject.transform.localScale = new Vector3(1 / scaleFactor, 1 / scaleFactor, 1 / scaleFactor);
            fieldObject.transform.parent = rootObject.transform;
            fieldObject.transform.localPosition = new Vector3(0f, -10f, 0f);
            fieldObject.transform.localScale = new Vector3(63 * 1000 / scaleFactor, 63 * 1000 / scaleFactor, 63 * 1000 / scaleFactor);
            fieldObject.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            for (int i = 0; i < gameData.players.Count; i++)
            {
                float x = (float)(double)gameData.players[i][0][0] / localScale;
                float y = (float)(double)gameData.players[i][0][2] / localScale;
                float z = (float)(double)gameData.players[i][0][1] / localScale;
                var car = Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                car.transform.parent = rootObject.transform;
                car.transform.localPosition = new Vector3(x, y, z);
                //car.transform.localScale = new Vector3(120 * 1000 / scaleFactor, 120 * 1000 / scaleFactor, 120 * 1000 / scaleFactor);
                car.GetComponent<Renderer>().material = gameData.colors[i] == 0 ? blueCar : orangeCar;
                cars.Add(car);


                var name = Instantiate(namePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                name.GetComponent<TextMesh>().text = gameData.names[i];
                name.transform.parent = rootObject.transform;
                name.transform.localPosition = new Vector3(x, y + 1.0f, z);
                names.Add(name); // needed so we can look at camera all the time
            }
            ballObject = Instantiate(ball, new Vector3(0f, 0f, 0f), Quaternion.identity);
            ballObject.transform.parent = rootObject.transform;
            ballObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            ballObject.transform.localScale = new Vector3(10 * 1000 / scaleFactor, 10 * 1000 / scaleFactor, 10 * 1000 / scaleFactor);
            currentTime = (float)gameData.frames[0][2];
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _UpdateApplicationLifecycle();
            var gameData = StaticReplayScript.gameData;
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

            bool testing = false;
            // If the player has not touched the screen, we are done with this update.
            if (fieldObject == null)
            {
                if (testing && gameData != null)
                {
                    GameObject empty = new GameObject("Empty");
                    Spawn(new Vector3(0f, 0f, 0f), Quaternion.identity, empty.transform);
                    GameObject device = GameObject.Find("ARCore Device");
                    scaleFactor = 100000f;
                    device.transform.position = new Vector3(1000000f, 1000000f, 1000000f);
                    device.transform.LookAt(empty.transform);
                }
                else
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
                                hit.Pose.rotation * Vector3.up) < 0)
                        {
                            Debug.Log("Hit at back of the current DetectedPlane");
                        }
                        else
                        {
                            var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                            Spawn(hit.Pose.position, hit.Pose.rotation, anchor.transform);
                            
                        }
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
                    float x = (float) (double) frameData[0] / localScale;
                    float y = (float) (double) frameData[2] / localScale;
                    float z = (float) (double) frameData[1] / localScale;

                    var rotX = (double) frameData[3] * 180 / 3.14159265;
                    var rotY = (double) frameData[4] * 180 / 3.14159265;
                    var rotZ = (double) frameData[5] * 180 / 3.14159265;

                    //var rotation = Quaternion.AngleAxis((float) rotY, Vector3.up) * // yaw
                      //  Quaternion.AngleAxis((float) rotX, Vector3.right) *
                        //Quaternion.AngleAxis((float) rotZ, Vector3.forward);
                    cars[i].transform.localPosition = new Vector3(x, y, z);
                    cars[i].transform.localEulerAngles = new Vector3((float) rotZ, (float) rotY, (float) rotX);
                    //cars[i].transform.localRotation = rotation;
                    names[i].transform.localPosition = new Vector3(x, y + 0.5f, z);
                    names[i].transform.LookAt(FirstPersonCamera.transform);
                    names[i].transform.Rotate(Vector3.up - new Vector3(0, 180, 0));
                }
                
                float ballx = (float)gameData.ball[currentFrame][0] / localScale;
                float bally = (float)gameData.ball[currentFrame][2] / localScale;
                float ballz = (float)gameData.ball[currentFrame][1] / localScale;
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

        void DrawQuad(Rect position, Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            GUI.Box(position, GUIContent.none);
        }

        public void OnGUI()
        {
            var width = Screen.width;
            var zoomOut = GUI.Button(new Rect(width * 2/10, 25, width / 25, width / 25), "-");
            var zoomIn = GUI.Button(new Rect(width * 3 / 10, 25, width / 25, width / 25), "+");
            if (zoomOut)
            {
                scaleFactor += 100;
                rootObject.transform.localScale = new Vector3(1 / scaleFactor, 1 / scaleFactor, 1 / scaleFactor);
            }
            else if (zoomIn)
            {
                scaleFactor -= 100;
                rootObject.transform.localScale = new Vector3(1 / scaleFactor, 1 / scaleFactor, 1 / scaleFactor);
            }


            // Scoreboard
            int offset = 100;
            Color orange = new Color(1F, 0.64F, 0F);
            var halfway = width/2;
            GUIStyle timeStyle = new GUIStyle();
            timeStyle.fontSize = 30;
            timeStyle.fontStyle = FontStyle.Bold;

            timeStyle.normal.textColor = Color.gray;
            timeStyle.alignment = TextAnchor.MiddleCenter;
            int timeRemaining = 300;
            if (StaticReplayScript.gameData != null)
            {
                timeRemaining = (int)StaticReplayScript.gameData.frames[currentFrame][1];
            }
            //int timeRemaining = 175;
            int minRemaining = timeRemaining/60;
            GUI.Label(new Rect(halfway, 35, 50, 50), String.Format("{0}:{1:D2}", minRemaining, timeRemaining % (minRemaining * 60)), timeStyle);
            for (int i = 0; i < 2; i++)
            {
                Color teamColor = (i == 0) ? Color.blue : (orange);

                var offsetX = (float)width / 2 - offset + (i * offset * 2);
                Rect position = new Rect(offsetX, 35, 50, 50);
                DrawQuad(position, teamColor);


                GUIStyle style = new GUIStyle();
                ///style.font = new Font("Liberation Sans");
                style.fontSize = 30;
                style.fontStyle = FontStyle.Bold;
                
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                
                GUI.Label(position, i.ToString(), style);
                //Color oldColor = GUI.backgroundColor;
                //GUI.backgroundColor = (i == 0) ? Color.blue : Color.red*Color.yellow;
                //GUI.backgroundColor = oldColor;



            }
            RootObject gameData = StaticReplayScript.gameData;
            if (gameData != null)
            {
                currentTime = GUI.HorizontalSlider(new Rect(250 + 150, 35, 200, 30), currentTime, (float)gameData.frames[0][2], (float) gameData.frames[gameData.frames.Count - 1][2]);

            }


            // Button
            var back = GUI.Button(new Rect(25, 25, width / 10, width / 25), "Back to Menu");
            if (back)
            {
                SceneManager.LoadScene("Menu");
            }


            if (dataMsg != null)
            {
                var style = new GUIStyle();
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                GUI.Label(new Rect(0, width/10, width / 5, width / 10), dataMsg, style);
            }
            if (protoMsg != null)
            {
                var style = new GUIStyle();
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                GUI.Label(new Rect(0, 0, width / 5, width / 10), protoMsg, style);
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
