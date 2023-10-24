using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class SelectVFX : MonoBehaviour
{
    public GameObject[] vfx;
    public int currentVFX;
    public GameObject ActionDetected;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var vfx in vfx)
        {
            vfx.GetComponent<VisualEffect>().Stop();
        }

        //currentVFX = Random.Range(0, vfx.Length);
        currentVFX = ActionDetected.GetComponent<GetInferenceFromDanceModel>().prediction.predictedIndex;
        vfx[currentVFX].GetComponent<VisualEffect>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        int newVFX = ActionDetected.GetComponent<GetInferenceFromDanceModel>().prediction.predictedIndex;
        if (currentVFX != newVFX)
        {
            Debug.Log($"New effect : {newVFX}");
            SelectionVFX(newVFX, currentVFX);
            currentVFX = newVFX;
        }
    }

    void SelectionVFX(int newVFX, int oldVFX)
    {
        vfx[oldVFX].GetComponent<VisualEffect>().Stop();
        vfx[newVFX].GetComponent<VisualEffect>().Play();
    }
}
