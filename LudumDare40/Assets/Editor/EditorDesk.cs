using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DeskControl))]
public class EditorDesk : Editor
{
    DeskControl desk;
    SpriteRenderer sr;

    private void OnEnable()
    {
        desk = (DeskControl)target;

        sr = desk.GetComponent<SpriteRenderer>();
    }
    public override void OnInspectorGUI()
    {
        GUILayout.Label("Direction of desk");
        desk.direction = (Direction) EditorGUILayout.EnumPopup(desk.direction, GUILayout.Width(100));

        desk.leftSprite = (Sprite) EditorGUILayout.ObjectField("Left Sprite", desk.leftSprite, typeof(Sprite), false);
        desk.rightSprite = (Sprite)EditorGUILayout.ObjectField("Right Sprite", desk.rightSprite, typeof(Sprite), false);
        desk.downSprite = (Sprite)EditorGUILayout.ObjectField("Down Sprite", desk.downSprite, typeof(Sprite), false);
        desk.upSprite = (Sprite)EditorGUILayout.ObjectField("Up Sprite", desk.upSprite, typeof(Sprite), false);

        if(sr != null)
        {
            if (desk.direction == Direction.Down && desk.downSprite != null) sr.sprite = desk.downSprite;
            else if (desk.direction == Direction.Left && desk.leftSprite != null) sr.sprite = desk.leftSprite;
            else if (desk.direction == Direction.Right && desk.rightSprite != null) sr.sprite = desk.rightSprite;
            else if (desk.direction == Direction.Up && desk.upSprite != null) sr.sprite = desk.upSprite;
        }

        EditorUtility.SetDirty(desk); // save
    }
}
