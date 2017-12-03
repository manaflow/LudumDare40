using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Needs fixing

// Uses: loading, changing scenes, flashbacks
public class TransitionFade : MonoBehaviour {

    public enum TransitionFadeType
    {
        FadeIn,
        FadeOut,
    };

    public enum Style
    {
        Color,
        Texture,
        // Gradient is possible
    };

    public Style fadeType = Style.Color;
    public Color transitionColor;
    public Texture2D transitionTexture;

    public float transitionSpeed = 1.0f;
    private TransitionFadeType transitionDirection = TransitionFadeType.FadeIn;
    private int drawDepth = -1000;
    private float alpha = 1.0f;


    void OnGUI()
    {
        if(fadeType == Style.Texture)
        {
            
        }

        else if(fadeType == Style.Color)
        {
            transitionTexture = CreateImageWithColor(transitionColor);
        }

        if(transitionDirection == TransitionFadeType.FadeIn)
            transitionColor.a += transitionSpeed * -1 * Time.deltaTime;        

        else if (transitionDirection == TransitionFadeType.FadeOut)
            transitionColor.a += transitionSpeed * 1 * Time.deltaTime;

        transitionColor.a = Mathf.Clamp01(transitionColor.a);
        

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), transitionTexture);
    }

    Texture2D CreateImageWithColor(Color color)
    {
        Texture2D temp;
        temp = new Texture2D(2, 2, TextureFormat.ARGB32, false);

        temp.SetPixel(0, 0, transitionColor);
        temp.SetPixel(1, 0, transitionColor);
        temp.SetPixel(0, 1, transitionColor);
        temp.SetPixel(1, 1, transitionColor);

        temp.Apply();
        return temp;
    }

    public float BeginTransition (TransitionFadeType direction)
    {
        transitionDirection = direction;

        return transitionSpeed;
    }

    void OnLoadCallback()
    {
        BeginTransition(TransitionFadeType.FadeOut);
    }
}
