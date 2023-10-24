using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class SelectVFX : MonoBehaviour
{
    public GameObject[] vfx;
    int currentVFX;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var vfx in vfx)
        {
            vfx.GetComponent<VisualEffect>().Stop();
        }

        currentVFX = Random.Range(0, vfx.Length);
    }

    // Update is called once per frame
    void Update()
    {
        int newVFX = Random.Range(0, vfx.Length);
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
