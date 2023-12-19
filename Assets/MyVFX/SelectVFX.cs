using PoseTeacher;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;
using System.Threading.Tasks;
using static UnityEditor.BaseShaderGUI;
using System.Collections;

public class SelectVFX : MonoBehaviour
{
    public GameObject[] vfx;
    public int currentVFX;
    public GameObject ActionDetected;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start of FX");
        foreach (var vfx in vfx)
        {
            if (vfx.GetComponent<VisualEffect>() != null) vfx.GetComponent<VisualEffect>().Stop();
            else
            {
                foreach (var material in vfx.GetComponent<Renderer>().materials)
                {
                    material.SetFloat("_StrengthAlpha", 0);

                    Debug.Log(material.name);
                }
            }
        }

        currentVFX = -1;
        /*currentVFX = ActionDetected.GetComponent<GetInferenceFromDanceModel>().prediction.predictedIndex;
        vfx[currentVFX].GetComponent<VisualEffect>().Play();*/
    }

    // Update is called once per frame
    void Update()
    {
        //int newVFX = ActionDetected.GetComponent<GetInferenceFromDanceModel>().prediction.predictedIndex;
        int newVFX = ActionDetected.GetComponent<TryPoseScene>().Action;
        if (currentVFX != newVFX)
        {
            Debug.Log($"New effect : {newVFX}");
            SelectionVFX(newVFX, currentVFX);
            currentVFX = newVFX;
        }
    }

    private IEnumerator FadeObjectOut(Renderer FadingObject)
    {
        float time = 0;

        while (FadingObject.materials[0].GetFloat("_StrengthAlpha")> 0)
        {
            foreach (Material material in FadingObject.materials)
            {
                material.SetFloat("_StrengthAlpha", material.GetFloat("_StrengthAlpha") - time);
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeObjectIn(Renderer FadingObject)
    {
        float time = 0;
        while (FadingObject.materials[0].GetFloat("_StrengthAlpha") < 1)
        {
            foreach (Material material in FadingObject.materials)
            {
                material.SetFloat("_StrengthAlpha", material.GetFloat("_StrengthAlpha") + time);
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    async void SelectionVFX(int newVFX, int oldVFX)
    {
        if (oldVFX != -1)
        {
            await Task.Delay(3000);
            if (vfx[oldVFX].GetComponent<VisualEffect>() != null)  vfx[oldVFX].GetComponent<VisualEffect>().Stop();
            else
            {
                StartCoroutine(FadeObjectOut(vfx[oldVFX].GetComponent<Renderer>()));
            }
        }

        if (newVFX != -1)
        {
            if (vfx[newVFX].GetComponent<VisualEffect>() != null) vfx[newVFX].GetComponent<VisualEffect>().Play();
            else
            {
                StartCoroutine(FadeObjectIn(vfx[newVFX].GetComponent<Renderer>()));
            }
        }
    }
}
