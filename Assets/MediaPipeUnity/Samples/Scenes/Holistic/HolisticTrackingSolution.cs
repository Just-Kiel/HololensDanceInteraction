// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mediapipe.Unity.Holistic
{
  public class HolisticTrackingSolution : ImageSourceSolution<HolisticTrackingGraph>
  {
    [SerializeField] private RectTransform _worldAnnotationArea;
    [SerializeField] private DetectionAnnotationController _poseDetectionAnnotationController;
    [SerializeField] private HolisticLandmarkListAnnotationController _holisticAnnotationController;
    [SerializeField] private PoseWorldLandmarkListAnnotationController _poseWorldLandmarksAnnotationController;
    [SerializeField] private MaskAnnotationController _segmentationMaskAnnotationController;
    [SerializeField] private NormalizedRectAnnotationController _poseRoiAnnotationController;
        public Vector4[] landmarks;
        public HolisticTrackingGraph.ModelComplexity modelComplexity
    {
      get => graphRunner.modelComplexity;
      set => graphRunner.modelComplexity = value;
    }

    public bool smoothLandmarks
    {
      get => graphRunner.smoothLandmarks;
      set => graphRunner.smoothLandmarks = value;
    }

    public bool refineFaceLandmarks
    {
      get => graphRunner.refineFaceLandmarks;
      set => graphRunner.refineFaceLandmarks = value;
    }

    public bool enableSegmentation
    {
      get => graphRunner.enableSegmentation;
      set => graphRunner.enableSegmentation = value;
    }

    public bool smoothSegmentation
    {
      get => graphRunner.smoothSegmentation;
      set => graphRunner.smoothSegmentation = value;
    }

    public float minDetectionConfidence
    {
      get => graphRunner.minDetectionConfidence;
      set => graphRunner.minDetectionConfidence = value;
    }

    public float minTrackingConfidence
    {
      get => graphRunner.minTrackingConfidence;
      set => graphRunner.minTrackingConfidence = value;
    }

    protected override void SetupScreen(ImageSource imageSource)
    {
      base.SetupScreen(imageSource);
      _worldAnnotationArea.localEulerAngles = imageSource.rotation.Reverse().GetEulerAngles();
    }

    protected override void OnStartRun()
    {
      if (!runningMode.IsSynchronous())
      {
        graphRunner.OnPoseDetectionOutput += OnPoseDetectionOutput;
        graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
        graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
        graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
        graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;
        graphRunner.OnPoseWorldLandmarksOutput += OnPoseWorldLandmarksOutput;
        graphRunner.OnSegmentationMaskOutput += OnSegmentationMaskOutput;
        graphRunner.OnPoseRoiOutput += OnPoseRoiOutput;
      }

      var imageSource = ImageSourceProvider.ImageSource;
      SetupAnnotationController(_poseDetectionAnnotationController, imageSource);
      SetupAnnotationController(_holisticAnnotationController, imageSource);
      SetupAnnotationController(_poseWorldLandmarksAnnotationController, imageSource);
      SetupAnnotationController(_segmentationMaskAnnotationController, imageSource);
      _segmentationMaskAnnotationController.InitScreen(imageSource.textureWidth, imageSource.textureHeight);
      SetupAnnotationController(_poseRoiAnnotationController, imageSource);
    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
      graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
      Detection poseDetection = null;
      NormalizedLandmarkList faceLandmarks = null;
      NormalizedLandmarkList poseLandmarks = null;
      NormalizedLandmarkList leftHandLandmarks = null;
      NormalizedLandmarkList rightHandLandmarks = null;
      LandmarkList poseWorldLandmarks = null;
      ImageFrame segmentationMask = null;
      NormalizedRect poseRoi = null;

      if (runningMode == RunningMode.Sync)
      {
        var _ = graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out faceLandmarks, out leftHandLandmarks, out rightHandLandmarks, out poseWorldLandmarks, out segmentationMask, out poseRoi, true);
      }
      else if (runningMode == RunningMode.NonBlockingSync)
      {
        yield return new WaitUntil(() =>
          graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out faceLandmarks, out leftHandLandmarks, out rightHandLandmarks, out poseWorldLandmarks, out segmentationMask, out poseRoi, false));
      }

      _poseDetectionAnnotationController.DrawNow(poseDetection);
      _holisticAnnotationController.DrawNow(faceLandmarks, poseLandmarks, leftHandLandmarks, rightHandLandmarks);
      _poseWorldLandmarksAnnotationController.DrawNow(poseWorldLandmarks);
      _segmentationMaskAnnotationController.DrawNow(segmentationMask);
      _poseRoiAnnotationController.DrawNow(poseRoi);
    }

    private void OnPoseDetectionOutput(object stream, OutputEventArgs<Detection> eventArgs)
    {
      _poseDetectionAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnFaceLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
      _holisticAnnotationController.DrawFaceLandmarkListLater(eventArgs.value);
    }

        public Vector4[] LandmarksToVector4(LandmarkList nll)  // where nll = eventArgs.value of LandmarkDetection output
        {
            Vector4[] vectorpoints = null;
            if (nll != null)
            {
                List<Landmark> normalizedLandmarks = nll.Landmark.ToList();

                vectorpoints = new Vector4[normalizedLandmarks.Count];
                for (int i = 0; i < normalizedLandmarks.Count; i++)
                {              
                    vectorpoints[i] = new Vector4(normalizedLandmarks[i].X, normalizedLandmarks[i].Y, normalizedLandmarks[i].Z, normalizedLandmarks[i].Visibility);
                }
            }
            return vectorpoints;
        }

        private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
      _holisticAnnotationController.DrawPoseLandmarkListLater(eventArgs.value);
      /*landmarks = LandmarksToVector4(eventArgs.value);
            if (landmarks != null)
            {
                Debug.Log(landmarks.Length);
            }*/
    }

    private void OnLeftHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
      _holisticAnnotationController.DrawLeftHandLandmarkListLater(eventArgs.value);
    }

    private void OnRightHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
      _holisticAnnotationController.DrawRightHandLandmarkListLater(eventArgs.value);
    }

    private void OnPoseWorldLandmarksOutput(object stream, OutputEventArgs<LandmarkList> eventArgs)
    {
      _poseWorldLandmarksAnnotationController.DrawLater(eventArgs.value);
        landmarks = LandmarksToVector4(eventArgs.value);
        /*if (landmarks != null)
        {
                Debug.Log(landmarks[0]);
        }*/
    }

    private void OnSegmentationMaskOutput(object stream, OutputEventArgs<ImageFrame> eventArgs)
    {
      _segmentationMaskAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnPoseRoiOutput(object stream, OutputEventArgs<NormalizedRect> eventArgs)
    {
      _poseRoiAnnotationController.DrawLater(eventArgs.value);
    }
  }
}
