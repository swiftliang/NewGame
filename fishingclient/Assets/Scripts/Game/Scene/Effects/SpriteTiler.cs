using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class SpriteTiler : MonoBehaviour
{
    protected Material _material;
    // Use this for initialization
    void Start()
    {
        var spriteRender = GetComponent<SpriteRenderer>();
        _material = new Material(spriteRender.sharedMaterial);
        spriteRender.sharedMaterial = _material;
        UpdateTiling();
    }
    public void UpdateTiling()
    {
        _material.SetFloat("RepeatX", transform.lossyScale.x);
        _material.SetFloat("RepeatY", transform.lossyScale.y);
    }
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(transform.hasChanged) UpdateTiling();
#endif
    }
}
