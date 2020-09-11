using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private CubeProperties _cubeProperties;
    [SerializeField] private Vector3 _direction;
    private Coroutine _flocking;

    public Vector3 Direction { get => _direction; }

    /// <summary>
    /// Asigna dirección aleatoria al cubo e inicia la corrutina encargada del flocking
    /// </summary>
    void Start()
    {
        _direction = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));

        if (_flocking != null) { StopAllCoroutines();}
        _flocking = StartCoroutine(Flocking());
    }

    /// <summary>
    /// Realiza el algritmo flocking indefinidamente a partir de 3 parametros, cohesión, alineamiento y separación,
    /// determinando la dirección que debe tomar un cubo
    /// </summary>
    private IEnumerator Flocking()
    {
        _direction = Cohesion() + Alignment() - Separation() /*+ BoundaryRepulsion()*/;
        if (_direction.magnitude > _cubeProperties.speed)
        {
            _direction = _direction.normalized * _cubeProperties.speed;
        }
        transform.position += _direction * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(_direction);
        yield return new WaitForEndOfFrame();
        StartCoroutine(Flocking());
    }

    /// <summary>
    /// Retorna un vector con la nueva posición de un cubo con respecto a un grupo de estos sin separarse
    /// </summary>
    private Vector3 Cohesion()
    {
        Collider[] hitRadius = Physics.OverlapSphere(transform.position, _cubeProperties.actionRadius, _cubeProperties.layerMask);
        int neighbours = 0;
        Vector3 averagePosition = Vector3.zero;
        foreach(Collider cubeCollider in hitRadius)
        {
            averagePosition += cubeCollider.transform.position;
            neighbours++;
        }
        
        if(neighbours > 0)
        {
            averagePosition /= neighbours;
            return Vector3.Lerp(Vector3.zero, averagePosition, averagePosition.magnitude / _cubeProperties.actionRadius);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Retorna un vector con la dirección que debe tomar un cubo con respecto a la dirección del grupo
    /// </summary>
    private Vector3 Alignment()
    {
        Collider[] hitRadius = Physics.OverlapSphere(transform.position, _cubeProperties.actionRadius, _cubeProperties.layerMask);
        int neighbours = 0;
        Vector3 averageDirection = Vector3.zero;
        foreach (Collider cubeCollider in hitRadius)
        {
            averageDirection += cubeCollider.gameObject.GetComponent<Cube>().Direction;
            neighbours++;
        }

        if (neighbours > 0)
        {
            averageDirection /= neighbours;
            return Vector3.Lerp(_direction, averageDirection, Time.deltaTime);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Retorna un vector de qué tanto se debe separar un cubo de otro cuando ambos se encuentran demasiado cerca
    /// </summary>
    private Vector3 Separation()
    {
        Collider[] evadeRadius = Physics.OverlapSphere(transform.position, _cubeProperties.evadeRadius, _cubeProperties.evadeLayer);
        int neighbours = 0;
        Vector3 averagePosition = Vector3.zero;
        foreach (Collider cubeCollider in evadeRadius)
        {
            averagePosition += cubeCollider.transform.position - transform.position;
            neighbours++;
        }
        
        if (neighbours > 0)
        {
            averagePosition /= neighbours;
            return Vector3.Lerp(Vector3.zero, averagePosition, (averagePosition.magnitude / _cubeProperties.evadeRadius)) * _cubeProperties.repulsionForce;
        }
        return Vector3.zero;
    }

    //private Vector3 BoundaryRepulsion()
    //{
    //    if(_direction.magnitude > _cubeProperties.boundaryRadius)
    //    {
    //        return transform.position.normalized * (_cubeProperties.boundaryRadius - transform.position.magnitude) * Time.deltaTime;
    //    }
    //    return Vector3.zero;
    //}
}
