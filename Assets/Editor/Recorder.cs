using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ManualSpriteSheetTool : EditorWindow
{
    [Header("Capture Settings")]
    public Camera captureCamera;
    public int frameWidth = 256;
    public int frameHeight = 256;
    public bool transparentBackground = true;

    // Holds the raw pixel data of each captured frame in memory
    private List<Color[]> capturedFrames = new List<Color[]>();

    [MenuItem("Tools/Manual Spritesheet Creator")]
    public static void ShowWindow()
    {
        GetWindow<ManualSpriteSheetTool>("Manual Capture");
    }

    private void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        captureCamera = (Camera)EditorGUILayout.ObjectField("Capture Camera", captureCamera, typeof(Camera), true);
        frameWidth = Mathf.Max(16, EditorGUILayout.IntField("Frame Width", frameWidth));
        frameHeight = Mathf.Max(16, EditorGUILayout.IntField("Frame Height", frameHeight));
        transparentBackground = EditorGUILayout.Toggle("Transparent Background", transparentBackground);

        GUILayout.Space(20);

        // UI for current memory status
        EditorGUILayout.HelpBox($"Frames in Memory: {capturedFrames.Count}", MessageType.Info);

        GUILayout.Space(10);
        
        // --- BUTTON 1: CAPTURE ---
        GUI.color = Color.cyan;
        if (GUILayout.Button("1. CAPTURE CURRENT FRAME", GUILayout.Height(50)))
        {
            CaptureFrame();
        }

        GUILayout.Space(10);

        // --- BUTTON 2: SAVE ---
        GUI.color = Color.green;
        GUI.enabled = capturedFrames.Count > 0; // Only enable if we have frames
        if (GUILayout.Button("2. SAVE SPRITESHEET", GUILayout.Height(50)))
        {
            SaveSpriteSheet();
        }
        GUI.enabled = true;

        GUILayout.Space(20);

        // Optional: Clear Memory button
        GUI.color = new Color(1f, 0.5f, 0.5f);
        if (GUILayout.Button("Clear Memory (Discard Frames)"))
        {
            capturedFrames.Clear();
            Debug.Log("Memory cleared.");
        }
        GUI.color = Color.white;
    }

    private void CaptureFrame()
    {
        if (captureCamera == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a Capture Camera first.", "OK");
            return;
        }

        // Cache original camera settings
        CameraClearFlags originalFlags = captureCamera.clearFlags;
        Color originalColor = captureCamera.backgroundColor;

        // Force transparent background if requested
        if (transparentBackground)
        {
            captureCamera.clearFlags = CameraClearFlags.SolidColor;
            captureCamera.backgroundColor = new Color(0, 0, 0, 0);
        }

        // Setup Render Texture
        RenderTexture rt = new RenderTexture(frameWidth, frameHeight, 24, RenderTextureFormat.ARGB32);
        captureCamera.targetTexture = rt;

        // Render exactly what the camera sees RIGHT NOW
        captureCamera.Render();

        // Read pixels into memory
        RenderTexture.active = rt;
        Texture2D tempTex = new Texture2D(frameWidth, frameHeight, TextureFormat.ARGB32, false);
        tempTex.ReadPixels(new Rect(0, 0, frameWidth, frameHeight), 0, 0);
        tempTex.Apply();

        // Store raw color data in our list
        capturedFrames.Add(tempTex.GetPixels());

        // Cleanup
        RenderTexture.active = null;
        captureCamera.targetTexture = null;
        DestroyImmediate(tempTex);
        DestroyImmediate(rt);

        // Restore camera settings
        captureCamera.clearFlags = originalFlags;
        captureCamera.backgroundColor = originalColor;

        Debug.Log($"Captured frame {capturedFrames.Count}");
    }

    private void SaveSpriteSheet()
    {
        // Calculate a square grid so we don't hit texture width limits
        int frameCount = capturedFrames.Count;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(frameCount));
        int rows = Mathf.CeilToInt((float)frameCount / columns);

        int sheetWidth = columns * frameWidth;
        int sheetHeight = rows * frameHeight;

        // Create Master Texture
        Texture2D spriteSheet = new Texture2D(sheetWidth, sheetHeight, TextureFormat.ARGB32, false);

        // Clear background with transparent pixels
        Color[] clearColors = new Color[sheetWidth * sheetHeight];
        for (int i = 0; i < clearColors.Length; i++) clearColors[i] = new Color(0, 0, 0, 0);
        spriteSheet.SetPixels(clearColors);

        // Stitch the frames together
        for (int i = 0; i < frameCount; i++)
        {
            int col = i % columns;
            int row = i / columns;

            // X and Y offset (Unity Y is bottom-to-top, so we invert Y so Frame 1 is top-left)
            int xOffset = col * frameWidth;
            int yOffset = (rows - 1 - row) * frameHeight;

            spriteSheet.SetPixels(xOffset, yOffset, frameWidth, frameHeight, capturedFrames[i]);
        }

        spriteSheet.Apply();

        // Ask user where to save it
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Spritesheet",
            "New_Manual_Spritesheet",
            "png",
            "Save your custom spritesheet"
        );

        if (!string.IsNullOrEmpty(path))
        {
            byte[] bytes = spriteSheet.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            
            Debug.Log($"Successfully saved {frameCount} frames to {path}");
            
            // Clear memory automatically after successful save
            capturedFrames.Clear();
        }

        DestroyImmediate(spriteSheet);
    }
}