using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cube", menuName = "Create Cube")]
public class CubeProperties : ScriptableObject
{
    public float speed;
    public float repulsionForce;
    public float actionRadius;
    public float evadeRadius;
    public float boundaryRadius;
    public LayerMask layerMask;
    public LayerMask evadeLayer;
}
