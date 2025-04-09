using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PointAndGo : MonoBehaviour
{
    [Header("Scene Names")]
    public string oceanSceneName = "Ocean";
    public string plainsSceneName = "Plains";
    
    [Header("Color Detection Thresholds")]
    public Color waterColor = new Color(0.1f, 0.4f, 0.8f);
    public Color vegetationColor = new Color(0.2f, 0.6f, 0.3f);
    
    [Range(0.1f, 1.0f)]
    public float colorMatchThreshold = 0.3f;
    
    [Header("Animation")]
    public float zoomSpeed = 2.5f;
    private float minZoom = 1f;
    private float maxZoom = 50f;
    private float currentZoom = 1.0f;
    private float zoomDuration = 2.0f;
    public bool simulateZoomOnly = false; // If true, don't change scenes, just simulate zoom
    
    private Camera mainCamera;
    private bool isZooming = false;
    private float initialFOV;
    private Material earthMaterial;
    private RaycastHit hit;
    private MeshRenderer meshRenderer;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found! Please tag your camera as MainCamera.");
            return;
        }
        
        initialFOV = mainCamera.fieldOfView;
        
        // Get mesh renderer
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("No MeshRenderer component found on Earth object!");
            return;
        }
        
        // Get or create the material
        earthMaterial = meshRenderer.material;
        
        // Ensure the object has a collider for raycasting
        if (GetComponent<Collider>() == null)
        {
            // Add a sphere collider if none exists
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            Debug.Log("Added SphereCollider component to Earth object since none was found.");
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
                // Check if the ray hit THIS game object (the sphere)
                if (hit.transform == this.transform)
                {
                    Debug.Log("Clicked on Earth sphere!");
                    StartCoroutine(ZoomIntoTerrainAt(hit.point, hit.textureCoord));
                }
            }
        }
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        
        if (mainCamera.orthographic)
        {
            float newSize = mainCamera.orthographicSize - (scrollInput * zoomSpeed);
            mainCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
        else
        {
            float newFOV = mainCamera.fieldOfView - (scrollInput * zoomSpeed * 10f);
            mainCamera.fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
        }
    }
    
    IEnumerator ZoomIntoTerrainAt(Vector3 worldPos, Vector2 uv)
    {
        isZooming = true;
        
        // Determine scene based on color at click point
        string targetScene = DetermineTerrainType(uv);
        Debug.Log($"Target scene determined: {targetScene}");
        
        // Get original scale
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f; // Increase size by 20%
        
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        float startFOV = mainCamera.fieldOfView;
        
        Vector3 targetPosition = worldPos + hit.normal * 0.5f;
        Quaternion targetRotation = Quaternion.LookRotation(-hit.normal);
        float targetFOV = startFOV * 0.1f;
        
        // Get original color (for fading)
        Color originalColor = meshRenderer.material.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f);
        
        // Perform zoom animation
        float elapsed = 0;
        while (elapsed < zoomDuration)
        {
            float t = elapsed / zoomDuration;
            
            // Apply easing function
            t = t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
            
            // Update position and camera
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            
            // Update mesh scale and transparency
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // If we're just simulating the zoom, reset and return
        if (simulateZoomOnly)
        {
            // Reset camera
            mainCamera.transform.position = startPosition; 
            mainCamera.transform.rotation = startRotation;
            mainCamera.fieldOfView = initialFOV;
            transform.localScale = originalScale;
            
            isZooming = false;
            yield break;
        }
        
        // Otherwise load the new scene
        yield return StartCoroutine(FadeAndLoadScene(targetScene));
        
        // Find the main camera in the new scene
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found in the new scene!");
        }
        
        // Adjust position in the new scene if needed
        // You might want to position the Earth at a specific location in each scene
        // transform.position = new Vector3(x, y, z); // Set specific position for the Earth in the new scene
        
        isZooming = false;
    }
    
    IEnumerator FadeAndLoadScene(string sceneName)
    {
        // Save references to cameras and other objects that will be destroyed
        Camera previousCamera = mainCamera;
        
        // Create a canvas for the fade effect
        GameObject fadeObj = new GameObject("FadeObject");
        Canvas fadeCanvas = fadeObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999;
        
        // Add a CanvasScaler for consistent UI scaling
        UnityEngine.UI.CanvasScaler scaler = fadeObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Add a RectTransform and set it to fill the screen
        RectTransform rectTransform = fadeObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        
        // Add an image component for the fade
        UnityEngine.UI.Image fadeImage = fadeObj.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        
        // Make sure this object persists through scene loading
        DontDestroyOnLoad(fadeObj);
        
        // Fade to black
        float elapsed = 0;
        float fadeDuration = 1.0f;
        while (elapsed < fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Keep screen black during scene change
        fadeImage.color = Color.black;
        
        // Debug available scenes
        Debug.Log("Available scenes in build settings:");
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"Scene {i}: {sceneNameFromBuild}");
        }
        
        // Verify the scene exists before trying to load it
        bool sceneExists = false;
        int sceneIndex = -1;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneNameFromBuild == sceneName)
            {
                sceneExists = true;
                sceneIndex = i;
                break;
            }
        }
        
        // Load the scene if it exists
        if (sceneExists)
        {
            Debug.Log($"Loading scene: {sceneName} (index {sceneIndex})");
        
        
            // Load scene by index to avoid any name issues
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
            
            if (asyncLoad == null)
            {
                Debug.LogError("LoadSceneAsync returned null. Scene loading failed.");
            }
            else
            {
                // Set allowSceneActivation to true to ensure scene actually activates
                asyncLoad.allowSceneActivation = true;
                
                // Wait until the scene is fully loaded
                while (!asyncLoad.isDone)
                {
                    Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");
                    yield return null;
                }
                
                Debug.Log($"Scene {sceneName} loaded successfully!");
            }

        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found in build settings! Check that the scene is added to build settings.");
            // Fade back in since we're not changing scenes
            elapsed = 0;
            while (elapsed < fadeDuration)
            {
                fadeImage.color = new Color(0, 0, 0, 1 - (elapsed / fadeDuration));
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        // Fade back in after scene is loaded (or if scene loading failed)
        elapsed = 0;
        while (elapsed < fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, 1 - (elapsed / fadeDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Clean up the fade object
        Destroy(fadeObj);
    }
    
    string DetermineTerrainType(Vector2 uv)
    {
        Color pixelColor = SampleTextureAtPoint(uv);
        Debug.Log($"Sampled color at UV ({uv.x}, {uv.y}): {pixelColor}");
        
        float waterSimilarity = ColorSimilarity(pixelColor, waterColor);
        float vegetationSimilarity = ColorSimilarity(pixelColor, vegetationColor);
        
        Debug.Log($"Water similarity: {waterSimilarity}, Vegetation similarity: {vegetationSimilarity}");
        
        float maxSimilarity = Mathf.Max(waterSimilarity, vegetationSimilarity);
        
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
        else
        {
            Debug.Log("Determined terrain: Vegetation/Plains");
            return plainsSceneName;
        }
    }
    
    Color SampleTextureAtPoint(Vector2 uv)
    {
        Color result = Color.magenta; // Default "error" color
        
        if (earthMaterial != null)
        {
            if (earthMaterial.HasProperty("_MainTex"))
            {
                Texture2D mainTexture = earthMaterial.mainTexture as Texture2D;
                if (mainTexture != null)
                {
                    try
                    {
                        // Check if texture is readable
                        if (!mainTexture.isReadable)
                        {
                            Debug.LogError("Earth texture is not readable! You must enable 'Read/Write Enabled' in the texture import settings.");
                            return result;
                        }
                        
                        result = mainTexture.GetPixelBilinear(uv.x, uv.y);
                    }
                    catch (UnityException e)
                    {
                        Debug.LogError($"Error sampling texture: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogError("Earth material has _MainTex property but texture is null!");
                }
            }
            else if (earthMaterial.HasProperty("_Color"))
            {
                result = earthMaterial.color;
                Debug.Log("No texture found, using material color instead.");
            }
            else
            {
                Debug.LogError("Earth material has neither _MainTex nor _Color properties!");
            }
        }
        
        return result;
    }
    
    float ColorSimilarity(Color a, Color b)
    {
        // Calculate Euclidean distance in RGB space
        float distance = Mathf.Sqrt(
            (a.r - b.r) * (a.r - b.r) +
            (a.g - b.g) * (a.g - b.g) +
            (a.b - b.b) * (a.b - b.b)
        );
        
        // Convert to similarity (1.732 is max possible distance in RGB space)
        return 1f - Mathf.Clamp01(distance / 1.732f);
    }
    
#if UNITY_EDITOR
    // Debug visualization in editor
    void OnDrawGizmosSelected()
    {
        // Draw sphere to represent clickable area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>()?.radius ?? 1f);
        
        // Draw detection colors for reference
        Gizmos.color = waterColor;
        Gizmos.DrawCube(transform.position + Vector3.right * 1.5f, Vector3.one * 0.2f);
        
        Gizmos.color = vegetationColor;
        Gizmos.DrawCube(transform.position + Vector3.right * 2f, Vector3.one * 0.2f);
    }
#endif
}