using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSequencer : MonoBehaviour {
     
    private Queue<Sprite> textureQueue;
    public bool displayNextImage { get; set; }
    public Sprite[] textures;

    // Use this for initialization
    void Start ()
    {
        textureQueue = new Queue<Sprite>();
	}
	
    public void LoadImages()
    {
        textureQueue.Clear();

        foreach (Sprite texture in textures)
        {
            textureQueue.Enqueue(texture);
        }
    }

    public Sprite DisplayNextImage()
    {
        if (textureQueue.Count == 0)
        {
            return null;
        }

        Sprite texture = textureQueue.Dequeue();

        return texture;
    }
}
