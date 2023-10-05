using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

namespace PoseTeacher
{
    public class HandDetection : MonoBehaviour
    {
        /*public GameObject MainObject; // Main GameObject
        public GameObject MenuObject; // Menu new GameObject
        public GameObject buttonPrefab; // Button GameObject that will be instantiated in the course menus
        public GameObject CourseDetails; // CourseDetails gameObject in Menu new GO
        public GameObject CourseButtonCollection; // Button Collection GameObject for specific courses
        public GameObject HandDetectionObject; // GameObject that this script is assigned to*/

        [SerializeField]
        private MixedRealityInputAction grabAction = MixedRealityInputAction.None;

        public string testHand;

        public void OnInputDown(InputEventData eventData)
        {
            /*if (*//*eventData.MixedRealityInputAction == grabAction*//* )
            {
                Debug.LogError("You are grabbing !");
                GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f);
            }*/

            Debug.LogError("You are grabbing !");
        }

        public void OnInputUp(InputEventData eventData)
        {
            /*if (eventData.MixedRealityInputAction == grabAction)
            {
                Debug.LogError("You stopped grabbing !");
                GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f);
            }*/

            Debug.LogError("You stopped grabbing !");
        }
    }
}