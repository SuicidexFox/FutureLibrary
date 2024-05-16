using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpFade : MonoBehaviour
{
    // Public variables for start and end opacity, and duration of the fade
    public float startOpacity = 1f;
    public float endOpacity = 0f;
    public float duration = 2f;
    
    private Material[] materials;
    private float[] startOpacities;
    private float[] currentOpacities;

    void Start()
    {
        // Get all materials of the object
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            materials = renderer.materials;
            startOpacities = new float[materials.Length];
            currentOpacities = new float[materials.Length];

            // Ensure the materials use a shader that supports transparency
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].HasProperty("_Color"))
                {
                    // Set the initial opacity for each material
                    Color color = materials[i].color;
                    startOpacities[i] = color.a;
                    currentOpacities[i] = startOpacities[i];
                }
            }   
        }
    }

    public void TriggerFade()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            StartCoroutine(FadeTo(materials[i], i));
        }
    }

    private IEnumerator FadeTo(Material material, int index)
    {
        float startOpacity = currentOpacities[index];
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newOpacity = Mathf.Lerp(startOpacity, endOpacity, elapsedTime / duration);
            SetOpacity(material, index, newOpacity);
            yield return null;
        }

        SetOpacity(material, index, endOpacity);

        bool allMaterialsZeroOpacity = true;
        for (int i = 0; i < currentOpacities.Length; i++)
        {
            if (currentOpacities[i] != 0f)
            {
                allMaterialsZeroOpacity = false;
                break;
            }
        }

        if (allMaterialsZeroOpacity)
        {
            Destroy(gameObject);
        }
    }

    private void SetOpacity(Material material, int index, float opacity)
    {
        if (material != null && material.HasProperty("_Color"))
        {
            Color color = material.color;
            color.a = opacity;
            material.color = color;
            currentOpacities[index] = opacity;
        }
    }
}