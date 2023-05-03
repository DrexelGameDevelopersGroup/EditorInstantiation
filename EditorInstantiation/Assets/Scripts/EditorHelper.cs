using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

//Written by Colin Page
//You may freely use this in any project
public static class EditorHelper
{
    #region Instantiation
    /// <summary>
    /// Instantiate an instance of a prefab in the scene, maintaining a link to its prefab asset
    /// </summary>
    public static GameObject InstantiatePrefab(GameObject prefab)
    {
#if UNITY_EDITOR
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene()); //This tells the Unity scene that we changed something, and that it needs to be saved
        return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
#else
        return Instantiate(prefab);
#endif
    }

    public static GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject go = InstantiatePrefab(prefab);
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.transform.SetParent(parent);
        return go;
    }

    #endregion

    #region Destruction
    /// <summary>
    /// Safely destroy a gameobject, calling either Destroy() or DestroyImmediate() as appropriate
    /// </summary>
    /// <param name="toDestroy">The object to destroy</param>
    /// <param name="caller">The monobehavior responsible for destroying that object</param>
    public static void DestroySafe(GameObject toDestroy, MonoBehaviour caller)
    {
#if UNITY_EDITOR //The following block of code will not be included when building this project.
        
        if (Application.isPlaying)
        {
            Object.Destroy(toDestroy);
        }
        else
        {
            //We know that we are in the Editor and the game is not playing. This is the only time DestroyImmediate() can be called safely.
            caller.StartCoroutine(DestroyNextFrame(toDestroy));
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
#else //In a build, this code would run instead:
        Object.Destroy(toDestroy);
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// Unity does not allow destroying objects from an OnValidate call. Instead, we wait a frame before destroying it.
    /// </summary>
    /// <param name="go">The object to destroy</param>
    private static IEnumerator DestroyNextFrame(GameObject go)
    {
        yield return null;
        Object.DestroyImmediate(go);
    }
#endif

    public static void DestroyChildren(Transform parent, MonoBehaviour caller) //   >:)
    {
        for(int c = parent.childCount-1; c >= 0; --c) //We must traverse backwards through the children as we destroy them
        {
            DestroySafe(parent.GetChild(c).gameObject, caller);
        }
    }
    #endregion
}
