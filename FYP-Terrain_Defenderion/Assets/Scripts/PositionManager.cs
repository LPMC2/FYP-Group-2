using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PositionManager : MonoBehaviour
{
    [SerializeField] private bool m_SetValueAtStart = true;
    [SerializeField] protected Vector3 initialPos;
    public Vector3 InitialPos { get; }
    [SerializeField] private Quaternion initialRotation;
    [SerializeField] private Vector3 initialScale;
    //public NetworkVariable<bool> isCloned = new NetworkVariable<bool>();
    //public void Awake()
    //{
    //    isCloned.Value = false;
    //}
    // Start is called before the first frame update
    void Start()
    {
        if (m_SetValueAtStart)
        {
            initialPos = gameObject.transform.position;
            initialRotation = gameObject.transform.rotation;
            initialScale = gameObject.transform.localScale;
        }else
        {
            gameObject.transform.position = initialPos;
            gameObject.transform.rotation = initialRotation;
            gameObject.transform.localScale = initialScale;
        }
    }

    public void ResetTransform()
    {
        transform.position = initialPos;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
        //if (!isCloned.Value)
        //    gameObject.SetActive(true);

    }
    public void SetScale(float Value)
    {
        gameObject.transform.localScale = new Vector3(Value, Value, Value);

    }
    public Vector3 GetInitialPosition()
    {
        return initialPos;
    }
}
