using UnityEngine;

public class Tester : MonoBehaviour
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    [SerializeField] private Renderer ren;
    [SerializeField] private Texture[] textures;
    [SerializeField] private int textureIndex;

    [ContextMenu("ChangeTexture")]
    public void ChangeTexture()
    {
        ren.material.SetTexture(MainTex, textures[textureIndex]);
    }
}