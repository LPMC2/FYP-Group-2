using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialBehaviour : MonoBehaviour
{
    List<Material> originalMats = new List<Material>();
    MeshRenderer meshRenderer;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        Material[] materials = meshRenderer.materials;

        for (int i = 0; i < materials.Length; i++)
        {
            originalMats.Add(materials[i]);
        }
    }
    public void SetMaterial(Material newMaterial)
    {
        if(meshRenderer == null)
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        Material[] materials = meshRenderer.materials;

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = newMaterial;
        }

        meshRenderer.materials = materials;
    }
    public void SetInitialMaterial(Material material)
    {

        for (int i = 0; i < originalMats.Count; i++)
        {
            originalMats[i] = material;
        }
    }
    public void ResetMat()
    {
        if (meshRenderer == null)
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (originalMats.Count == 0 || originalMats == null) return;
        Material[] materials = meshRenderer.materials;

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = originalMats[i];
        }

        meshRenderer.materials = materials;
    }
}
