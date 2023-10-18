using System.Collections.Generic;
using UnityEngine;

namespace PoseTeacher
{
    // Main script
    public class TryPoseScene : MonoBehaviour
    {
        PoseInputGetter SelfPoseInputGetter;
        public GetInferenceFromDanceModel Model;

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

        public List<AvatarContainer> GetSelfAvatarContainers()
        {
            return avatarListSelf;
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

        }

        // Done at each application update
        void Update()
        {
            CheckKeyInput();

            if (!pauseSelf)
            {
                AnimateSelf(SelfPoseInputGetter.GetNextPose());
                Model.UpdatePosition(SelfPoseInputGetter.GetNextPose());

                //Debug.Log(SelfPoseInputGetter.GetNextPose().data[0].Position);
            }
        }


        // Actions to do before quitting application
        private void OnApplicationQuit()
        {
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
