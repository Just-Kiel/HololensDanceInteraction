using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Video;

public class GetInferenceFromDanceModel : MonoBehaviour
{
    public VideoClip clip;
    public NNModel modelAsset;
    private Model _runtimeModel;
    private IWorker _engine;
    private int[] ints = new int[2];

    [Serializable]
    public struct PredictionCategory
    {
        public int predictedValue;
        public float[] predicted;

        public void SetPrediction(Tensor t)
        {
            predicted = t.AsFloats();
            predictedValue = Array.IndexOf(predicted, predicted.Max());
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

        
        ints[0] = (int)clip.width;
        ints[1] = (int)clip.height;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // making a tensor out of grayscale texture
            var channelCount = 1; //grayscale; 3=color; 4=rgba
            var inputX = new Tensor(1, (int)clip.height, (int)clip.width, channelCount, clip.originalPath);

            Tensor outputY = _engine.Execute(inputX).PeekOutput();

            inputX.Dispose();
            prediction.SetPrediction(outputY);
        }
        
    }

    private void OnDestroy()
    {
        _engine?.Dispose();
    }
}
