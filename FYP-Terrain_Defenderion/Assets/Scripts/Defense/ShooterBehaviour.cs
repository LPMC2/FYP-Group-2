using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject originShooter;
    [SerializeField] private GameObject shootObject;
    public GameObject ShootObject { get { return shootObject; } private set { shootObject = value; } }
    [SerializeField] private float shootingSpeed = 1f;
    [SerializeField] private float objectSpeed = 1f;
    [SerializeField] private int shootCount = 1;
    [SerializeField] private float shootSpeedPerCount = 0.1f;
    private void Awake()
    {
        if(originShooter == null)
        {
            originShooter = gameObject;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
