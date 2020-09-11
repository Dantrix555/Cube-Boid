using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cube", menuName = "Create Cube")]
public class CubeProperties : ScriptableObject
{
    public float speed;
    public float repulsionForce;
    public float boundaryDetectDistance;

    [Header("Detection Radius")]
    public float actionRadius;
    public float evadeRadius;
    public float boundaryRadius;

    [Header("Detection Masks")]
    public LayerMask layerMask;
    public LayerMask evadeLayer;
}
