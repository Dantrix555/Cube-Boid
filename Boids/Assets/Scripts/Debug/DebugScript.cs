using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    public Vector3 goalPos;
    public Vector3 velocity;
    public float speed;
    public float radius;
    public float fov;
    public float linesOfView;

    void Start()
    {
        speed = 3f;
        radius = 10;
        fov = 125;
        linesOfView = 10;
        goalPos = new Vector3(1, 1, 1);
    }

    void Update()
    {
        RaycastHit hit;
        transform.position += Vector3.Lerp(transform.position, goalPos, speed) * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(goalPos);
        if(Physics.Raycast(transform.position, transform.forward, out hit, transform.forward.magnitude))
        {
            Debug.Log(hit.collider.gameObject.name);
        }

        //Debug.DrawRay(transform.position, transform.forward, Color.green);

        for(int i = -(int)fov/2; i <= (int)fov/2; i += (int)fov/10)
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward + AngleToDir(i, true) * radius, Color.red);
        }
    }

    Vector3 AngleToDir(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal) { angleInDegrees += transform.eulerAngles.y; }
        return new Vector3(Mathf.Sin(transform.forward.x + angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(transform.forward.z + angleInDegrees * Mathf.Deg2Rad));
    }
}
