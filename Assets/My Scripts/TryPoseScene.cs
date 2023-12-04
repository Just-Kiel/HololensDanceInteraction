using Mediapipe.BlazePose;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PoseTeacher
{
    // Main script
    public class TryPoseScene : MonoBehaviour
    {
        PoseInputGetter SelfPoseInputGetter;
        public int Action = -1;
        //public GetInferenceFromDanceModel Model;

        // Refrences to containers in scene
        public GameObject avatarContainer; // Only used to get reference from editor Inspector
        List<AvatarContainer> avatarListSelf;
        public GameObject avatarPrefab;


        // State of Main
        //public int recording_mode = 0; // 0: not recording, 1: recording, 2: playback, 3: load_file, 4: reset_recording
        public bool pauseSelf = false;

        // For fake data when emulating input from file
        private readonly string fake_file = "jsondata/2020_05_27-00_01_59.txt";
        public PoseInputSource SelfPoseInputSource = PoseInputSource.KINECT;
        public bool mirroring = false; // can probably be changed to private (if no UI elements use it)

        public PoseVisuallizer3D poseVisuallizer;

        public List<PoseData> recordedPose;
        public List<BlazePoseDetecter> recordedMediapipePose;
        const float unitAround = 100; // in millimeters
        public float threshold = 1;
        public GameObject VFXs;
        public List<AvatarContainer> GetSelfAvatarContainers()
        {
            return avatarListSelf;
        }

        public bool HandsAboveHead(PoseData CurrentPose)
        {
            return CurrentPose.data[8].Position.y < CurrentPose.data[27].Position.y 
                && CurrentPose.data[15].Position.y < CurrentPose.data[27].Position.y;
        }
        public bool HandsElbowsAboveHead(PoseData CurrentPose)
        {
            return CurrentPose.data[8].Position.y < CurrentPose.data[27].Position.y
                && CurrentPose.data[15].Position.y < CurrentPose.data[27].Position.y
                && CurrentPose.data[6].Position.y < CurrentPose.data[27].Position.y
                && CurrentPose.data[13].Position.y < CurrentPose.data[27].Position.y;
        }
        public bool ElbowsAboveHead(PoseData CurrentPose)
        {
            return CurrentPose.data[6].Position.y < CurrentPose.data[27].Position.y
                && CurrentPose.data[13].Position.y < CurrentPose.data[27].Position.y;
        }

        public bool MediapipeHandsAboveHead(PoseVisuallizer3D CurrentPose)
        {
            return CurrentPose.detecter.GetPoseLandmark(15).y > CurrentPose.detecter.GetPoseLandmark(0).y
                && CurrentPose.detecter.GetPoseLandmark(16).y > CurrentPose.detecter.GetPoseLandmark(0).y;
        }

        public bool MediapipeElbowsAboveHead(PoseVisuallizer3D CurrentPose)
        {
            return CurrentPose.detecter.GetPoseLandmark(13).y > CurrentPose.detecter.GetPoseLandmark(0).y
                && CurrentPose.detecter.GetPoseLandmark(14).y > CurrentPose.detecter.GetPoseLandmark(0).y;
        }
        //Add copy of self or techer to scene
        public void AddAvatar(bool self)
        {
            if (self)
            {
                GameObject newAvatar = Instantiate(avatarPrefab);
                AvatarContainer newAvatarCont = new AvatarContainer(newAvatar, mirroring);
                avatarListSelf.Add(newAvatarCont);
                newAvatar.SetActive(true);
                newAvatarCont.ChangeActiveType(avatarListSelf[0].activeType);
                newAvatarCont.MovePerson(SelfPoseInputGetter.CurrentPose);
                //newAvatar.transform.position = avatarListSelf[avatarListSelf.Count - 1].avatarContainer.transform.position;
                //newAvatar.transform.position = newAvatar.transform.position + new Vector3(1,0,0);
                //newAvatar.transform.position = new Vector3(1, 0, 0);
            }
        }
        public void DeleteAvatar(bool self)
        {
            if (self && avatarListSelf.Count > 1)
            {
                AvatarContainer avatar = avatarListSelf[avatarListSelf.Count - 1];
                avatar.avatarContainer.SetActive(false);
                avatarListSelf.Remove(avatar);
                Destroy(avatar.avatarContainer.gameObject);
            }
        }


        // Mirror all avatar containers
        public void do_mirror()
        {

            if (mirroring == true)
            {
                mirroring = false;
                // TODO figure out how to handle selection of mirrored avatar
                foreach (AvatarContainer avatar in avatarListSelf)
                {
                    avatar.Mirror();
                }
            }
            else
            {
                mirroring = true;
                foreach (AvatarContainer avatar in avatarListSelf)
                {
                    avatar.Mirror();
                }
            }
        }

        // Setters used in UI, configured in Inspector tab of respective buttons

        public void set_recording_mode(int rec)
        {
            // 0: pause,  2: play
            // code using this should be refactored, numbers were used in older verison of project

            switch (rec)
            {
                case 0:
                    pauseSelf = false;
                    break;
                case 2:
                    pauseSelf = false;
                    break;
                default:
                    Debug.Log("Assigned unused recording_mode");
                    break;
            }
        }

        // Set the avatar type for all AvatarGos
        public void SetAvatarTypes(AvatarType avatarType)
        {
            foreach (AvatarContainer avatar in avatarListSelf)
            {
                avatar.ChangeActiveType(avatarType);
            }
        }

        // Do once on scene startup
        private void Start()
        {
            // Initialize the respective AvatarContainer classes
            avatarListSelf = new List<AvatarContainer>();
            avatarListSelf.Add(new AvatarContainer(avatarContainer));

            SelfPoseInputGetter = new PoseInputGetter(SelfPoseInputSource) { ReadDataPath = fake_file };
            SelfPoseInputGetter.loop = true;

            // Default is to have a mirrored view
            do_mirror();

            //recordedPose = null;

            switch (SelfPoseInputSource)
            {
                case PoseInputSource.KINECT:
                    if (File.Exists(Application.persistentDataPath + "/recordedMoves.json"))
                    {
                        Positions arrayDataPose;
                        string currentMoves = File.ReadAllText(Application.persistentDataPath + "/recordedMoves.json");
                        arrayDataPose = JsonUtility.FromJson<Positions>(currentMoves);
                        recordedPose = arrayDataPose.poses.ToList();
                    }
                    break;
                case PoseInputSource.MEDIAPIPE:
                    /*if (File.Exists(Application.persistentDataPath + "/recordedMediapipeMoves.json"))
                    {
                        MediapipePositions arrayDataPose;
                        string currentMoves = File.ReadAllText(Application.persistentDataPath + "/recordedMediapipeMoves.json");
                        arrayDataPose = JsonUtility.FromJson<MediapipePositions>(currentMoves);
                        recordedMediapipePose = arrayDataPose.poses.ToList();
                    } else*/
                    /*{*/
                        recordedMediapipePose = new List<BlazePoseDetecter>();
                    /*}*/
                    break;
            }
        }

        // Done at each application update
        void Update()
        {
            CheckKeyInput();

            if (!pauseSelf)
            {
                AnimateSelf(SelfPoseInputGetter.GetNextPose());
                //Model.UpdatePosition(SelfPoseInputGetter.GetNextPose());

                //Debug.Log(SelfPoseInputGetter.GetNextPose().data[0].Position);

                switch (SelfPoseInputSource)
                {
                    case PoseInputSource.KINECT:

                        /*if (HandsElbowsAboveHead(SelfPoseInputGetter.GetNextPose()))
                        {
                            Action = 0;
                            Debug.Log("Arms above head !");
                        }
                        else if (ElbowsAboveHead(SelfPoseInputGetter.GetNextPose()))
                        {
                            Action = 1;
                            Debug.Log("Elbows above head !");
                        }
                        else */
                        /*if (HandsAboveHead(SelfPoseInputGetter.GetNextPose()))
                        {
                            Action = 2;
                            Debug.Log("Hands above head !");
                        }
                        else
                        {
                            Action = -1;
                        }*/

                        if(recordedPose.Count != 0)
                        {
                            PoseData poseToRecord = SelfPoseInputGetter.GetNextPose();

                            for (int i = 0; i < poseToRecord.data.Length; i++)
                            {
                                poseToRecord.data[i].Position -= poseToRecord.data[0].Position;
                                poseToRecord.data[i].Orientation = Quaternion.Inverse(poseToRecord.data[0].Orientation) * poseToRecord.data[i].Orientation;
                            }
                            int index = 0;
                            /*if (SelfPoseInputGetter.GetNextPose().data[27].Position == recordedPose.data[27].Position)*/
                            foreach(PoseData poseData in recordedPose)
                            {
                                /*if (poseToRecord.ComparePosition(poseData, threshold * unitAround) >= 0.8)
                                {
                                    Action = index % VFXs.GetComponent<SelectVFX>().vfx.Length;
                                    Debug.Log("Pose accorded !");
                                    break;
                                }*/

                                if (poseToRecord.CompareRotation(poseData, 25.0f) >= 0.8)
                                {
                                    Action = index % VFXs.GetComponent<SelectVFX>().vfx.Length;
                                    Debug.Log("Pose accorded !");
                                    break;
                                }
                                else
                                {
                                    Action = -1;
                                }
                                index++;
                            }
                        }
                        else
                        {
                            Action = -1;
                        }

                        // Save position
                        if (Input.GetMouseButtonDown(1))
                        {
                            Debug.Log("Recording position !");
                            PoseData poseToRecord = SelfPoseInputGetter.GetNextPose();

                            for (int i = 0; i < poseToRecord.data.Length; i++)
                            {
                                poseToRecord.data[i].Position -= poseToRecord.data[0].Position;
                                poseToRecord.data[i].Orientation = Quaternion.Inverse(poseToRecord.data[0].Orientation) * poseToRecord.data[i].Orientation;
                            }

                            recordedPose.Add(poseToRecord);
                            Debug.Log("Record position !");
                        }

                        // Delete all positions
                        if (Input.GetMouseButtonDown(2))
                        {
                            recordedPose.Clear();
                            Debug.Log("Positions deleted !");
                        }
                            break;

                    // Case of MEDIAPIPE
                    case PoseInputSource.MEDIAPIPE:
                        if (MediapipeElbowsAboveHead(poseVisuallizer))
                        {
                            Action = 0;
                            Debug.Log("Elbows above head !");
                        }
                        else if (MediapipeHandsAboveHead(poseVisuallizer))
                        {
                            Action = 1;
                            Debug.Log("Hands above head !");
                        }
                        else
                        {
                            Action = -1;
                        }

                        /*if (recordedMediapipePose.Count != 0)
                        {
                            PoseData poseToRecord = PoseData.ConvertMediapipeToPose(poseVisuallizer.detecter);

                            Debug.Log($"{poseToRecord.data.Length}");
                            int index = 0;
                            foreach (PoseData poseData in MediapipePositions.ConvertMediapipeToData(recordedMediapipePose).poses)
                            {
                                if (poseToRecord.ComparePosition(poseData, threshold * unitAround) >= 0.8)
                                {
                                    Action = index % VFXs.GetComponent<SelectVFX>().vfx.Length;

                                    // Not working correctly
                                    Debug.Log("Pose accorded !");
                                    break;
                                }
                                else
                                {
                                    Action = -1;
                                }
                                index++;
                            }
                        }
                        else
                        {
                            Action = -1;
                        }*/

                        // Save position
                        if (Input.GetMouseButtonDown(1))
                        {
                            Debug.Log("Recording position !");
                            BlazePoseDetecter poseToRecord = poseVisuallizer.detecter;

                            recordedMediapipePose.Add(poseToRecord);
                            Debug.Log("Record position !");
                        }


                        if (Input.GetMouseButtonDown(2))
                        {
                            recordedMediapipePose.Clear();
                            Debug.Log("Positions deleted !");
                        }
                        break;
                }
                
            }
        }


        // Actions to do before quitting application
        private void OnApplicationQuit()
        {
            string jsonData = "";
            switch (SelfPoseInputSource)
            {
                case PoseInputSource.KINECT:
                    Positions arrayDataPose = new Positions();
                    arrayDataPose.poses = recordedPose.ToArray();
                    jsonData = JsonUtility.ToJson(arrayDataPose);

                    File.WriteAllText(Application.persistentDataPath + "/recordedMoves.json", jsonData);
                    break;

                case PoseInputSource.MEDIAPIPE:
                    MediapipePositions arrayMediapipeDataPose = MediapipePositions.ConvertMediapipeToData(recordedMediapipePose);

                    jsonData = JsonUtility.ToJson(arrayMediapipeDataPose);
                    File.WriteAllText(Application.persistentDataPath + "/recordedMediapipeMoves.json", jsonData);
                    break;
            }
            Debug.Log($"{jsonData}");

            SelfPoseInputGetter.Dispose();
        }

        // Change recording mode via keyboard input for debugging and to not need menu
        void CheckKeyInput()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("P - toggle self pause");
                pauseSelf = !pauseSelf;
            }


        }

        // Animates all self avatars based on the JointData provided
        void AnimateSelf(PoseData live_data)
        {
            // MovePerson() considers which container to move
            foreach (AvatarContainer avatar in avatarListSelf)
            {
                avatar.MovePerson(live_data);
            }
        }


        #region Functions for external changes

        public void ActivateIndicators()
        {
            foreach (AvatarContainer avatar in avatarListSelf)
            {
                avatar.MoveIndicators(true);
            }
        }

        public void StartRecordingMode(bool temporary)
        {
            Debug.Log("Start recording mode");
            if (temporary)
            {
                //SelfPoseInputGetter.WriteDataPath = "jsondata/temporary.txt";
                //SelfPoseInputGetter.ResetRecording();
                SelfPoseInputGetter.GenNewFilename();
            }
            else
            {
                // Generate new filename with timestamp
                SelfPoseInputGetter.GenNewFilename();
            }

            SelfPoseInputGetter.recording = true;
        }
    }

    #endregion
}
