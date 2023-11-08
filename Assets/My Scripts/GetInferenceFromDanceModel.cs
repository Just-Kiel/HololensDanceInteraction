using PoseTeacher;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class GetInferenceFromDanceModel : MonoBehaviour
{
    public NNModel modelAsset;
    private Model _runtimeModel;
    private IWorker _engine;
    private JointData[] jointData;
    private float[] array;
    public GameObject Visuallizer;
    private PoseVisuallizer3D _poseVisuallizer;

    static public string[] categories =
    {
        "kick_right_leg",
        "point_left_hand_around_bounce_hips",
        "start_capturing_movement",
        "stop_capturing_movement",
        "sweep_hands_right",
        "turn_180_around_fist_up_sway_hips"
    };
    /*{
        "both_hands_up",
        "left_foot_up",
        "right_hand_up",
        "waiting"
    };*/
    /*{
        "kick_right_leg", 
        "point_left_hand_around_bounce_hips",
        "start_capturing_movement",
        "stop_capturing_movement",
        "sweep_hands_right",
        "turn_180_around_fist_up_sway_hips"
    };*/

    static float[][] MapAzureKinectToMediapipe(float[][] azureKinectJoints)
    {
        float[][] mediapipeJoints = new float[33][] ;

        foreach (var mapping in jointMapping)
        {
                // Perform any necessary coordinate system transformations here if needed
                mediapipeJoints[mapping.Key] = azureKinectJoints[mapping.Value];
         
        }

        // Add logic to handle the extra joint in Mediapipe if needed

        return mediapipeJoints;
    }

    static Dictionary<int, int> jointMapping = new Dictionary<int, int>
    {
        //Mediapipe, Azure Kinect
        { 0, 27 },
        { 1, 28 },
        { 2, 28 },
        { 3, 28 },
        { 4, 30 },
        { 5, 30 },
        { 6, 30 },
        { 7, 29 },
        { 8, 31 },
        { 9, 29 },
        { 10, 31 },
        { 11, 5 },
        { 12, 12 },
        { 13, 6 },
        { 14, 13 },
        { 15, 7 },
        { 16, 14 },
        { 17, 9 },
        { 18, 16 },
        { 19, 9 },
        { 20, 16 },
        { 21, 10 },
        { 22, 17 },
        { 23, 18 },
        { 24, 22 },
        { 25, 19 },
        { 26, 23 },
        { 27, 20 },
        { 28, 24 },
        { 29, 20 },
        { 30, 24 },
        { 31, 21 },
        { 32, 25 },
        // Add more mappings as needed
    };



    [Serializable]
    public struct PredictionCategory
    {
        public string predictedValue;
        public int predictedIndex;
        public float[] predicted;

        public void SetPrediction(Tensor t)
        {
            predicted = t.AsFloats();
            predictedValue = GetInferenceFromDanceModel.categories[Array.IndexOf(predicted, predicted.Max())];
            predictedIndex = Array.IndexOf(predicted, predicted.Max());
            Debug.Log($"Predicted {predictedValue} ");
        }
    }

    public PredictionCategory prediction;
    // Start is called before the first frame update
    void Start()
    {
        _runtimeModel = ModelLoader.Load(modelAsset);

        _engine = WorkerFactory.CreateWorker(_runtimeModel, WorkerFactory.Device.GPU);

        prediction = new PredictionCategory();

        array = new float[3960];

        _poseVisuallizer = Visuallizer.GetComponent<PoseVisuallizer3D>();
    }

    // Update is called once per frame
    void Update()
    {
        //New attempt
        for (int i = 0; i < _poseVisuallizer.detecter.vertexCount + 1; i++)
        {
            float[] temp = {
                _poseVisuallizer.detecter.GetPoseLandmark(i).x,
                _poseVisuallizer.detecter.GetPoseLandmark(i).y,
                _poseVisuallizer.detecter.GetPoseLandmark(i).z,
                _poseVisuallizer.detecter.GetPoseLandmark(i).w
            };

            array = array.Concat(temp).ToArray();
        }

        if (array.Length < 3960) return;
        if (array.Length > 3960)
        {
            array = array.Skip(132).ToArray();
        }

        var inputX = new Tensor(1, 1, 132, 30, array);

        Tensor outputY = _engine.Execute(inputX).PeekOutput();

        inputX.Dispose();
        prediction.SetPrediction(outputY);
    }

    public void UpdatePosition(PoseData CurrentPose)
    {
        /*jointData = CurrentPose.data;

        float[][] azureKinectJoints;
        azureKinectJoints = jointData.Select(joint => new float[] { joint.Position.x, joint.Position.y, joint.Position.z, 1.0F }).ToArray();
        float[][] mediapipeJoints = MapAzureKinectToMediapipe(azureKinectJoints);
        array = array.Concat(mediapipeJoints.SelectMany(x => x)).ToArray();*/

        //Debug.Log($"Right wrist {mediapipeJoints[16][0]}, {mediapipeJoints[16][1]}, {mediapipeJoints[16][2]} ");

        //New attempt
        for (int i = 0; i < _poseVisuallizer.detecter.vertexCount + 1; i++)
        {
            float[] temp = {
                _poseVisuallizer.detecter.GetPoseLandmark(i).x,
                _poseVisuallizer.detecter.GetPoseLandmark(i).y,
                _poseVisuallizer.detecter.GetPoseLandmark(i).z,
                _poseVisuallizer.detecter.GetPoseLandmark(i).w
            };

            array = array.Concat(temp).ToArray();
        }

        if (array.Length < 3960) return;
        if (array.Length > 3960)
        {
            array = array.Skip(132).ToArray();
        }

        var inputX = new Tensor(1, 1, 132, 30, array);

        Tensor outputY = _engine.Execute(inputX).PeekOutput();

        inputX.Dispose();
        prediction.SetPrediction(outputY);
    }

    private void OnDestroy()
    {
        _engine?.Dispose();
    }
}
