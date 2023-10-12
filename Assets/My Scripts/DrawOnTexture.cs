using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnTexture : MonoBehaviour
{
    public Texture2D texture;

    // Update is called once per frame
    void Update()
    {
        DoMouseDrawing();
        
    }

    private void DoMouseDrawing()
    {
        if (Camera.main == null)
        {
            throw new System.Exception("Cannot see");
        }

        //is mouse being pressed
        if(!Input.GetMouseButton(0) && !Input.GetMouseButton (1))
        {
            return;
        }

        Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(mouseRay, out hit)) return;
        if (hit.collider.transform != transform) return;

        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= texture.width;
        pixelUV.y *= texture.height;

        Color colorToSet = Input.GetMouseButton(0) ? Color.white : Color.black;

        texture.SetPixel((int) pixelUV.x, (int) pixelUV.y, colorToSet);
        texture.Apply();
    }
}
