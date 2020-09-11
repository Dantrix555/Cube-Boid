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
        _direction = CalculateDirection().normalized * _cubeProperties.speed;

        //Detecta si pueden existir colisiones con obstaculos, en caso afirmativo cambia la dirección del cubo
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, _cubeProperties.boundaryRadius, _direction, out hit, _cubeProperties.boundaryDetectDistance, _cubeProperties.evadeLayer))
        {
            _direction = BoundaryRepulsion().normalized * _cubeProperties.speed;
        }

        //Evita sobrepasar el límite de velocidad
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
    /// Retorna vector con la suma de vectores de cohesion, alineamiento y separación para determinar la 
    /// dirección de un cubo
    /// </summary>
    private Vector3 CalculateDirection()
    {
        Collider[] hitRadius = Physics.OverlapSphere(transform.position, _cubeProperties.actionRadius, _cubeProperties.layerMask);
        Collider[] evadeRadius = Physics.OverlapSphere(transform.position, _cubeProperties.evadeRadius, _cubeProperties.layerMask);
        int neighbors = 0;
        Vector3 averagePosition = Vector3.zero; // Vector de posición para la cohesión
        Vector3 averageDirection = Vector3.zero; // Vector de dirección para el alineamiento
        Vector3 averageSeparation = Vector3.zero; // Vector de posición para la separación

        //Busca el promedio de cohesión y de alineamiento
        foreach (Collider cubeCollider in hitRadius)
        {
            averagePosition += cubeCollider.transform.position;
            averageDirection += cubeCollider.gameObject.GetComponent<Cube>().Direction;
            if (cubeCollider != gameObject.GetComponent<Collider>())
                neighbors++;
        }

        if(neighbors > 0)
        {
            //Calcula la nueva posición de cohesión
            averagePosition /= neighbors;
            averagePosition = Vector3.Lerp(Vector3.zero, averagePosition, averagePosition.magnitude / _cubeProperties.actionRadius);

            //Calcula la nueva dirección para el alineamiento
            averageDirection /= neighbors;
            averageDirection = Vector3.Lerp(_direction, averageDirection, Time.deltaTime);
        }

        neighbors = 0;

        //Busca el promedio de separación
        foreach(Collider cubeCollider in evadeRadius)
        {
            averageSeparation += cubeCollider.transform.position - transform.position;
            if (cubeCollider != this.gameObject.GetComponent<Collider>())
                neighbors++;
        }

        if(neighbors > 0)
        {
            //Calcula la nueva posición de separación
            averageSeparation /= neighbors;
            averageSeparation = Vector3.Lerp(Vector3.zero, averagePosition, (averagePosition.magnitude / _cubeProperties.evadeRadius)) * _cubeProperties.repulsionForce;
        }

        return averagePosition + averageDirection - averageSeparation;
    }

    /// <summary>
    /// Crea un vector que devuelve la nueva dirección del cubo en caso de encontrar una dirección
    /// donde no hayan colisiones con un obstaculo mediante un rayo
    /// Código tomado de: https://github.com/SebLague/Boids/blob/master/Assets/Scripts/Boid.cs
    /// </summary>
    private Vector3 BoundaryRepulsion()
    {
        Vector3[] rayDirections = CubeObstacleView.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(transform.position, dir);
            if (!Physics.SphereCast(ray, _cubeProperties.boundaryRadius, _cubeProperties.boundaryDetectDistance, _cubeProperties.evadeLayer))
            {
                return dir;
            }
        }
        return transform.forward;
    }
}

/// <summary>
/// Clase tomada de: https://github.com/SebLague/Boids/blob/master/Assets/Scripts/BoidHelper.cs
/// En esta clase se crea un array de vectores similares a un cono de visión para el cubo
/// </summary>
public static class CubeObstacleView
{
    const int numViewDirections = 300;
    public static readonly Vector3[] directions;

    static CubeObstacleView()
    {
        directions = new Vector3[numViewDirections];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numViewDirections; i++)
        {
            float t = (float)i / numViewDirections;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            directions[i] = new Vector3(x, y, z);
        }
    }
}
