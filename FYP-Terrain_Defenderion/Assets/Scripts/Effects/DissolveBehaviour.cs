using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveBehaviour : ConsecutiveActionBehaviour
{
    [SerializeField] private Material UnionMaterial;
    [SerializeField] private Material DissolveMaterial;
    [SerializeField] private float time;
    Renderer meshRenderer;
    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        mat1 = new Material(UnionMaterial);
        mat2 = new Material(DissolveMaterial);
        originMat = meshRenderer.materials;
    }
    public bool isEnumerator = false;
    private void Update()
    {
        isEnumerator = Coroutine != null;
    }
    public override void StartActions()
    {
        StopAllCoroutines();
        BuildActions();
        base.StartActions();
    }
    private void StartUnionEnumerator()
    {
        StartCoroutine(UnionEnumerator());
    }
    private Material mat1;
    private Material mat2;
    private Material[] originMat;
    private IEnumerator UnionEnumerator()
    {
        Material[] materials = new Material[1+originMat.Length];
        for(int i=0; i<originMat.Length; i++)
        {
            materials[i] = originMat[i];
        }
        
        materials[materials.Length-1] = mat1;
        meshRenderer.materials = materials;
        float timer = 0f;
        while(timer < time) {
            timer += Time.deltaTime;

                float t = timer / Actions[0].TimeToNextAction;
                t = Mathf.Lerp(0, 1, t);
            mat1.color = new Color(mat1.color.r, mat1.color.g, mat1.color.b, t);
                yield return null;
            
        }
        
    }
    private void StartDissolveEnumerator()
    {
        StartCoroutine(DissolveEnumerator());
    }
    private IEnumerator DissolveEnumerator()
    {
        meshRenderer.materials = new Material[1];
        
        meshRenderer.material = mat2;
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;

                float t = timer / time;
                t = Mathf.Lerp(0, 1, t);
            mat2.SetFloat("_DissolveValue", t);
                yield return null;

            
        }
        meshRenderer.materials = originMat;
    }
    private void BuildActions()
    {
        if (Actions.Count == 0 || Actions.Count != 2)
        {
            Actions = new List<Action>();
            Actions.Add(new Action());
            Actions.Add(new Action());
            foreach (Action action in Actions)
            {
                action.TimeToNextAction = time;
            }
        }
        Actions[0].Events.AddListener(StartUnionEnumerator);
        Actions[1].Events.AddListener(StartDissolveEnumerator);

    }

}
