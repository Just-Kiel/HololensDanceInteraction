using PoseTeacher;
using UnityEngine;
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

    void SelectionVFX(int newVFX, int oldVFX)
    {
        if (oldVFX != -1) vfx[oldVFX].GetComponent<VisualEffect>().Stop();
        if (newVFX != -1) vfx[newVFX].GetComponent<VisualEffect>().Play();
    }
}
