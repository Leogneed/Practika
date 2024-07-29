using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Blocks/Normal Block")]
public class BlockInfo : ScriptableObject
{
    public BlockType Type;
    public Vector2 PixelsOffset;
   // public string Description;

    public AudioClip StepSound;
    public float TimetoBreak = 0.3f;

    public virtual Vector2 GetPixelOffset(Vector3Int normal)
    {
        return PixelsOffset;
    }

    public int ID => GetInstanceID();

    [field: SerializeField]
    public string Name { get; set; }

    [field: SerializeField]
    [field: TextArea]
    public string Description { get; set; }

    [field: SerializeField]
    public Sprite ItemImage { get; set; }
}
