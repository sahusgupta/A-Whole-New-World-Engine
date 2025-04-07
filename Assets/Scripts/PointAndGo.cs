using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EarthTerrainZoomer : MonoBehaviour
{
    [Header("Scene Names")]
    public string oceanSceneName = "OceanScene";
    public string plainsSceneName = "PlainsScene";
    public string mountainSceneName = "MountainScene";
    public string desertSceneName = "DesertScene";
    public string snowSceneName = "SnowScene";
    
    [Header("Color Detection Thresholds")]
    public Color waterColor = new Color(0.1f, 0.4f, 0.8f);
    public Color vegetationColor = new Color(0.2f, 0.6f, 0.3f);
    public Color mountainColor = new Color(0.5f, 0.5f, 0.5f);
    public Color desertColor = new Color(0.8f, 0.7f, 0.4f);
    public Color snowColor = new Color(0.9f, 0.9f, 0.9f);
    
    [Range(0.1f, 1.0f)]
    public float colorMatchThreshold = 0.3f;
    
    [Header("Animation")]
    public float zoomDuration = 2.0f;
    public Animator earthAnimator;
    
    private Camera mainCamera;
    private bool isZooming = false;
    private float initialFOV;
    private Material earthMaterial;
    private RaycastHit hit;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found! Please tag your camera as MainCamera.");
            return;
        }
        
        initialFOV = mainCamera.fieldOfView;
        
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            earthMaterial = renderer.material;
        }
        else
        {
            Debug.LogError("No renderer component found on Earth object!");
        }
    }
    
    void Update()
    {
        if (isZooming)
            return;
            
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == this.transform)
                {
                    StartCoroutine(ZoomIntoTerrainAt(hit.point, hit.textureCoord));
                }
            }
        }
    }
    
    IEnumerator ZoomIntoTerrainAt(Vector3 worldPos, Vector2 uv)
    {
        isZooming = true;
        
        string targetScene = DetermineTerrainType(uv);
        
        if (earthAnimator != null)
        {
            earthAnimator.SetTrigger("ZoomIn");
        }
        
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        float startFOV = mainCamera.fieldOfView;
        
        Vector3 targetPosition = worldPos + hit.normal * 0.5f;
        Quaternion targetRotation = Quaternion.LookRotation(-hit.normal);
        float targetFOV = startFOV * 0.1f;
        
        float elapsed = 0;
        while (elapsed < zoomDuration)
        {
            float t = elapsed / zoomDuration;
            
            t = t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
            
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        yield return StartCoroutine(FadeAndLoadScene(targetScene));
        
        mainCamera.transform.position = startPosition;
        mainCamera.transform.rotation = startRotation;
        mainCamera.fieldOfView = initialFOV;
        
        isZooming = false;
    }
    
    IEnumerator FadeAndLoadScene(string sceneName)
    {
        GameObject fadeObj = new GameObject("FadeObject");
        Canvas fadeCanvas = fadeObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999;
        
        UnityEngine.UI.Image fadeImage = fadeObj.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        
        float elapsed = 0;
        float fadeDuration = 1.0f;
        while (elapsed < fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        SceneManager.LoadScene(sceneName);
        
        Destroy(fadeObj);
    }
    
    string DetermineTerrainType(Vector2 uv)
    {
        Color pixelColor = SampleTextureAtPoint(uv);
        Debug.Log($"Sampled color at UV ({uv.x}, {uv.y}): {pixelColor}");
        
        float waterSimilarity = ColorSimilarity(pixelColor, waterColor);
        float vegetationSimilarity = ColorSimilarity(pixelColor, vegetationColor);
        float mountainSimilarity = ColorSimilarity(pixelColor, mountainColor);
        float desertSimilarity = ColorSimilarity(pixelColor, desertColor);
        float snowSimilarity = ColorSimilarity(pixelColor, snowColor);
        
        float maxSimilarity = Mathf.Max(waterSimilarity, vegetationSimilarity, 
                                        mountainSimilarity, desertSimilarity, 
                                        snowSimilarity);
        
        if (maxSimilarity < (1 - colorMatchThreshold))
        {
            Debug.LogWarning($"Color match too weak ({maxSimilarity}), defaulting to Plains");
            return plainsSceneName;
        }
        
        if (maxSimilarity == waterSimilarity)
        {
            Debug.Log("Determined terrain: Water");
            return oceanSceneName;
        }
        else if (maxSimilarity == vegetationSimilarity)
        {
            Debug.Log("Determined terrain: Vegetation/Plains");
            return plainsSceneName;
        }
        else if (maxSimilarity == mountainSimilarity)
        {
            Debug.Log("Determined terrain: Mountains");
            return mountainSceneName;
        }
        else if (maxSimilarity == desertSimilarity)
        {
            Debug.Log("Determined terrain: Desert");
            return desertSceneName;
        }
        else
        {
            Debug.Log("Determined terrain: Snow/Ice");
            return snowSceneName;
        }
    }
    
    Color SampleTextureAtPoint(Vector2 uv)
    {
        Color result = Color.magenta;
        
        if (earthMaterial != null)
        {
            if (earthMaterial.HasProperty("_MainTex"))
            {
                Texture2D mainTexture = earthMaterial.mainTexture as Texture2D;
                if (mainTexture != null)
                {
                    try
                    {
                        result = mainTexture.GetPixelBilinear(uv.x, uv.y);
                    }
                    catch (UnityException e)
                    {
                        Debug.LogError($"Error sampling texture: {e.Message}. Make sure your texture is marked as readable in import settings.");
                    }
                }
            }
            else
            {
                if (earthMaterial.HasProperty("_Color"))
                {
                    result = earthMaterial.color;
                }
            }
        }
        
        return result;
    }
    
    float ColorSimilarity(Color a, Color b)
    {
        float distance = Mathf.Sqrt(
            (a.r - b.r) * (a.r - b.r) +
            (a.g - b.g) * (a.g - b.g) +
            (a.b - b.b) * (a.b - b.b)
        );
        
        return 1f - Mathf.Clamp01(distance / 1.732f);
    }
}