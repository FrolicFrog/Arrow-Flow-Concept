using UnityEngine;
using UnityEditor;
using System.IO;

public class StylizedMaterialBaker : EditorWindow
{
    private Renderer targetRenderer;
    private int resolution = 2048;
    private int edgePadding = 8;
    
    private int[] resolutions = { 256, 512, 1024, 2048, 4096 };
    private string[] resLabels = { "256", "512", "1024", "2048", "4096" };

    [MenuItem("Tools/Stylized Material Baker")]
    public static void ShowWindow()
    {
        GetWindow<StylizedMaterialBaker>("Material Baker").minSize = new Vector2(350, 200);
    }

    private void OnGUI()
    {
        GUILayout.Label("Bake Stylized Materials to Unlit Texture", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        targetRenderer = (Renderer)EditorGUILayout.ObjectField("Target Renderer", targetRenderer, typeof(Renderer), true);
        
        if (targetRenderer != null && !(targetRenderer is MeshRenderer) && !(targetRenderer is SkinnedMeshRenderer))
        {
            EditorGUILayout.HelpBox("Please select a MeshRenderer or SkinnedMeshRenderer.", MessageType.Error);
            targetRenderer = null;
        }

        resolution = EditorGUILayout.IntPopup("Texture Resolution", resolution, resLabels, resolutions);
        edgePadding = EditorGUILayout.IntSlider("Edge Padding (Dilation)", edgePadding, 0, 32);

        EditorGUILayout.Space();

        if (GUILayout.Button("Bake to Texture", GUILayout.Height(40)))
        {
            if (targetRenderer == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a valid Renderer.", "OK");
                return;
            }

            string path = EditorUtility.SaveFilePanelInProject("Save Baked Texture", targetRenderer.name + "_BakedAlbedo", "png", "Save the baked texture.");
            if (!string.IsNullOrEmpty(path))
            {
                BakeTexture(path);
            }
        }
    }

    private void BakeTexture(string savePath)
    {
        Mesh originalMesh = null;
        if (targetRenderer is MeshRenderer)
        {
            MeshFilter filter = targetRenderer.GetComponent<MeshFilter>();
            if (filter != null) originalMesh = filter.sharedMesh;
        }
        else if (targetRenderer is SkinnedMeshRenderer smr)
        {
            originalMesh = new Mesh();
            smr.BakeMesh(originalMesh); // Captures current pose
        }

        if (originalMesh == null || originalMesh.uv.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Mesh is missing or has no UVs.", "OK");
            return;
        }

        // 1. Create Flattened UV Mesh
        Mesh flatMesh = CreateFlatUVMesh(originalMesh);

        // 2. Setup Render targets and temporary objects
        int layer = 31; // Use an obscure layer to avoid scene objects
        RenderTexture rt = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32);
        
        GameObject tempObj = new GameObject("TempBakeObj");
        tempObj.layer = layer;
        tempObj.transform.position = targetRenderer.transform.position; // Keep world position for lighting
        tempObj.transform.rotation = targetRenderer.transform.rotation;

        MeshFilter mf = tempObj.AddComponent<MeshFilter>();
        mf.sharedMesh = flatMesh;
        MeshRenderer mr = tempObj.AddComponent<MeshRenderer>();
        mr.sharedMaterials = targetRenderer.sharedMaterials;

        GameObject camObj = new GameObject("TempBakeCam");
        Camera bakeCam = camObj.AddComponent<Camera>();
        bakeCam.orthographic = true;
        bakeCam.orthographicSize = 1f;
        bakeCam.aspect = 1f;
        bakeCam.clearFlags = CameraClearFlags.SolidColor;
        bakeCam.backgroundColor = Color.clear; // Clear alpha is important for dilation
        bakeCam.cullingMask = 1 << layer; // Only see the temp object
        bakeCam.targetTexture = rt;
        
        // Position camera exactly in front of the flat mesh
        bakeCam.transform.position = tempObj.transform.position + tempObj.transform.forward * -2f;
        bakeCam.transform.rotation = tempObj.transform.rotation;
        bakeCam.nearClipPlane = 0.1f;
        bakeCam.farClipPlane = 5f;

        // 3. Render
        RenderTexture.active = rt;
        bakeCam.Render();

        // 4. Read to Texture2D
        Texture2D bakedTex = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
        bakedTex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        bakedTex.Apply();

        // 5. Apply Edge Padding (Dilation) to prevent seam bleeding
        if (edgePadding > 0)
        {
            DilateTexture(bakedTex, edgePadding);
        }

        // 6. Save File
        byte[] bytes = bakedTex.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);

        // 7. Clean up
        RenderTexture.active = null;
        rt.Release();
        DestroyImmediate(tempObj);
        DestroyImmediate(camObj);
        if (targetRenderer is SkinnedMeshRenderer) DestroyImmediate(originalMesh);

        AssetDatabase.Refresh();
        
        // Ensure the imported texture has an alpha channel enabled correctly
        TextureImporter importer = AssetImporter.GetAtPath(savePath) as TextureImporter;
        if (importer != null)
        {
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        EditorUtility.DisplayDialog("Success", "Texture Baked successfully!\n\nYou can now create a new Material using the 'Unlit/Texture' shader and apply this texture to it.", "OK");
    }

    private Mesh CreateFlatUVMesh(Mesh original)
    {
        Mesh flatMesh = new Mesh();
        flatMesh.name = "FlatUVMesh";

        Vector3[] verts = new Vector3[original.vertexCount];
        Vector2[] uvs = original.uv;

        // Map UVs (0 to 1) to Clip Space (-1 to 1)
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = new Vector3(uvs[i].x * 2f - 1f, uvs[i].y * 2f - 1f, 0f);
        }

        flatMesh.vertices = verts;
        flatMesh.uv = uvs;
        
        // Copy original 3D data so lighting still evaluates properly
        flatMesh.normals = original.normals;
        flatMesh.tangents = original.tangents;
        flatMesh.colors = original.colors;
        flatMesh.uv2 = original.uv2;

        flatMesh.subMeshCount = original.subMeshCount;
        for (int i = 0; i < original.subMeshCount; i++)
        {
            int[] origTris = original.GetTriangles(i);
            
            // Double the triangles and reverse winding order on the second half.
            // This ensures UV islands that are mirrored (flipped horizontally/vertically) 
            // don't get accidentally culled by Backface Culling during the bake!
            int[] doubleTris = new int[origTris.Length * 2];
            origTris.CopyTo(doubleTris, 0);
            
            for (int j = 0; j < origTris.Length; j += 3)
            {
                doubleTris[origTris.Length + j] = origTris[j];
                doubleTris[origTris.Length + j + 1] = origTris[j + 2];
                doubleTris[origTris.Length + j + 2] = origTris[j + 1];
            }
            
            flatMesh.SetTriangles(doubleTris, i);
        }

        return flatMesh;
    }

    private void DilateTexture(Texture2D tex, int padding)
    {
        Color[] pixels = tex.GetPixels();
        int width = tex.width;
        int height = tex.height;

        for (int i = 0; i < padding; i++)
        {
            Color[] newPixels = new Color[pixels.Length];
            System.Array.Copy(pixels, newPixels, pixels.Length);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = y * width + x;
                    if (pixels[idx].a < 0.1f) // If pixel is empty
                    {
                        // Look at neighbors for color
                        if (x > 0 && pixels[idx - 1].a > 0.1f) newPixels[idx] = pixels[idx - 1];
                        else if (x < width - 1 && pixels[idx + 1].a > 0.1f) newPixels[idx] = pixels[idx + 1];
                        else if (y > 0 && pixels[idx - width].a > 0.1f) newPixels[idx] = pixels[idx - width];
                        else if (y < height - 1 && pixels[idx + width].a > 0.1f) newPixels[idx] = pixels[idx + width];
                        
                        // Force solid alpha so it can spread to the next pixel in the next iteration
                        newPixels[idx].a = 1f; 
                    }
                }
            }
            pixels = newPixels;
        }

        tex.SetPixels(pixels);
        tex.Apply();
    }
}