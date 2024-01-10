using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterManager : MonoBehaviour
{
    [SerializeField] private GameObject m_ShooterObject;
    [SerializeField] private ShooterBehaviour m_shooterBehaviour;
    public GameObject ShooterObj { get { return m_ShooterObject; } }
    public ShooterBehaviour shooterBehaviour { get { return m_shooterBehaviour;} set { m_shooterBehaviour = value; } }
}
