using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLocations : MonoBehaviour
{
    [Header ("Leg Positions")]
    public Transform leftFrontLeg;
    public Transform rightFrontLeg;
    public Transform leftBackLeg;
    public Transform rightBackLeg;

    private Vector3 averagePos;

    private Vector3 prevPos;
    private Vector3 velocity;
    public float bodySpeed;

    [Range (0f, 1f)]
    public float bodyHeightOffset;
    public float angleMult;
    public float rotateSpeed;
    private void Awake()
    {
        prevPos = transform.position;
    }

    private void Update()
    {
        velocity = (transform.position - prevPos)/ Time.deltaTime;
        bodySpeed = velocity.magnitude;
        prevPos = transform.position;
        //Debug.Log(bodySpeed);
        //Debug.Log(FindAngle());
        //transform.rotation = FindAngle();
        transform.rotation = Quaternion.Lerp(transform.rotation, FindAngle(), Time.deltaTime * rotateSpeed);

        averagePos = (leftBackLeg.position + leftFrontLeg.position + rightBackLeg.position + rightFrontLeg.position) / 4f;
        transform.position = new Vector3(transform.position.x, averagePos.y + bodyHeightOffset, transform.position.z);



    }

    private Quaternion FindAngle()
    {
        //Finding Normal
        Vector3 n1 = FindNormal(leftFrontLeg.position, rightFrontLeg.position, leftBackLeg.position);
        Vector3 n2 = FindNormal(leftFrontLeg.position, rightFrontLeg.position, rightBackLeg.position);
        Vector3 n3 = FindNormal(leftBackLeg.position, rightBackLeg.position, rightFrontLeg.position);
        Vector3 n4 = FindNormal(leftFrontLeg.position, leftBackLeg.position, rightBackLeg.position);

        Vector3 averageAngle = (n1 + n2 + n3 + n4)/ 4;
        averageAngle.x = averageAngle.x * angleMult;
        averageAngle.y = averageAngle.y * angleMult;
        averageAngle.z = averageAngle.z * angleMult;
        //Vector3 averageAngle = (n1 + n2 + n3 + n4);
        Quaternion newQ = Quaternion.Euler(averageAngle);
        return newQ;
        
    }

    private Vector3 FindNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Plane plane = new Plane(p1, p2, p3);
        Vector3 normal = plane.normal;
        return normal;
        // Vector3 A = p2 - p1;
        // Vector3 B = p3 -p1;
        // float Nx = A.y * B.z - A.z * B.y;
        // float Ny = A.z * B.x - A.x * B.z;
        // float Nz = A.x * B.y - A.y * B.x;
        // return new Vector3(Nx, Ny, Nz);
    }

}
