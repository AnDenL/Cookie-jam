using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ChunkRender : MonoBehaviour
{
    private static ChunkRender instance;

    private Camera RenderCamera;

    private void Awake()
    {
        instance = this;
        RenderCamera = GetComponent<Camera>();
        
        instance.RenderCamera.orthographicSize = 16 / 2f;
    }

    public static Sprite Render(Vector2Int position)
    {
        instance.RenderCamera.enabled = true;
        instance.transform.position = (Vector2)position * 16;

        RenderTexture rt = new(16, 16, 16)
        {
            filterMode = FilterMode.Point
        };

        instance.RenderCamera.targetTexture = rt; 

        instance.RenderCamera.gameObject.SetActive(true);

        instance.RenderCamera.Render();

        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D texture2D = new(rt.width, rt.height, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        texture2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentActiveRT;
        instance.RenderCamera.enabled = false;

        instance.RenderCamera.targetTexture = null;
        rt.Release();
        
        return Sprite.Create(texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 1);
    }
}