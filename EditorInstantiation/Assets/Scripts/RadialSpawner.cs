using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] bool button = false;
    [SerializeField] int seed = 0;

    [Header("Spawn Config")]
    [SerializeField] int spawnCount = 3;
    [SerializeField] float startRadius = 0f;
    [SerializeField] float endRadius = 5f;
    [SerializeField] Vector2 revolutionDegreeRange;
    [SerializeField] float aspectRatio = 1f;
    [SerializeField] Vector2 randomOffset;

    private void OnValidate()
    {
        if (button)
        {
            Generate();
            button = false;
        }
    }

    [ContextMenu("Generate")]
    private void Generate()
    {
        print("generating...");

        EditorHelper.DestroyChildren(transform, this);

        Random.State oldState = Random.state;
        Random.InitState(seed);

        try
        {
            for (int i = 0; i < spawnCount; i++)
            {
                float t = i / ((float)spawnCount-1);
                float angle = Mathf.Lerp(revolutionDegreeRange.x, revolutionDegreeRange.y, t) * Mathf.Deg2Rad;

                var localPos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle) * aspectRatio) * Mathf.Lerp(startRadius, endRadius, t);
                localPos += new Vector3(Random.Range(-1f, 1f) * randomOffset.x, 0f, Random.Range(-1f, 1f) * randomOffset.y);

                //var newTransform = Instantiate(prefab).transform; //This standard instantiation function does not keep the prefab link
                var newTransform = EditorHelper.InstantiatePrefab(prefab).transform;
                newTransform.SetParent(transform);
                newTransform.localPosition = localPos;
                newTransform.localRotation = transform.rotation;                
            }
        }
        finally
        {
            Random.state = oldState;
        }
    }
}
