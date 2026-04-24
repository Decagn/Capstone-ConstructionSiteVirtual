using UnityEditor;
using UnityEngine;

/*
 * [BUG/ERROR FIX]
 * This script is essential for edge/vertex detection of any sort.
 * By default, meshes may not be readable, 
 * so we can't get the triangles that make up the mesh to find its edges.
 * 
 * If a "not allowed to access vertices on mesh" error occurs,
 * Select all assets (you can use Crtl+A in the Assets folder),
 * right click, then click reimport.
 * This script will automatically fix this error.
 */
public class MakeMeshesReadable : AssetPostprocessor
{
    private void OnPreprocessModel()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter != null)
        {
            modelImporter.isReadable = true;
        }
    }
}
