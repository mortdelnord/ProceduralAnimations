using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderArmV2 : MonoBehaviour
{
    [Header("Arm References")]
    public Transform cornerArmL;
    public Transform cornerArmR;

    public Transform opCornerArmL;
    public Transform opCornerArmR;

    [Header("Main Body Reference")]
    public Transform body;

    [Header("Raycast origins")]

    public Transform rightRaycastOrigin;
    public Transform leftRaycastOrigin;
    public Transform opRightRaycastOrigin;
    public Transform opLeftRaycastOrigin;

    [Header("Distance Limits")]
    [Range(0f, 1f)]
    public float maxTargetDis;
    [Range(0f, 1f)]
    public float minBodyDis;
    [Range(0f, 1f)]
    public float maxBodyDis;

    [Range(0.1f, 10f)]
    public float raycastDis;

    [Header("Leg Movement")]

    public float legSpeed;
    public float centerOffset;
    public float maxStepTime;

    private float stepTime = 0f;
    private float opStepTime = 0f;

    // private variables now
    private Vector3 LtargetPos;
    private Vector3 LnewPos;
    private Vector3 opLtargetPos;
    private Vector3 opLnewPos;
    private Vector3 RtargetPos;
    private Vector3 RnewPos;
    private Vector3 opRtargetPos;
    private Vector3 opRnewPos;

    private bool isGrounded = true;
    public bool moveFirstPair = true;

    private void Awake()
    {
        cornerArmL.position = LtargetPos;
        cornerArmR.position = RtargetPos;
        opCornerArmL.position = opLtargetPos;
        opCornerArmR.position = opRtargetPos;
    }

    private void Update()
    {
        // moveLegs(cornerArm1, leftRaycastOrigin, LtargetPos, LnewPos);
        // moveLegs(cornerArm2, rightRaycastOrigin, RtargetPos, RnewPos);
        RaycastHit leftHit;
        RaycastHit rightHit;

        Physics.Raycast(leftRaycastOrigin.position, Vector3.down, out leftHit, raycastDis);
        Debug.DrawRay(leftRaycastOrigin.position,  Vector3.down, Color.blue);

        Physics.Raycast(rightRaycastOrigin.position, Vector3.down, out rightHit, raycastDis);
        Debug.DrawRay(rightRaycastOrigin.position,  Vector3.down, Color.blue);

        Vector3 leftGroundPos = leftHit.point;
        Vector3 rightGroundPos = rightHit.point;

        RaycastHit opLeftHit;
        RaycastHit opRightHit;

        Physics.Raycast(opLeftRaycastOrigin.position, Vector3.down, out opLeftHit, raycastDis);
        Debug.DrawRay(opLeftRaycastOrigin.position,  Vector3.down, Color.blue);

        Physics.Raycast(opRightRaycastOrigin.position, Vector3.down, out opRightHit, raycastDis);
        Debug.DrawRay(opRightRaycastOrigin.position,  Vector3.down, Color.blue);

        Vector3 opLeftGroundPos = opLeftHit.point;
        Vector3 opRightGroundPos = opRightHit.point;

        if (moveFirstPair)
        {
            if (Vector3.Distance(leftGroundPos, LtargetPos) >= maxTargetDis || Vector3.Distance(cornerArmL.position, body.position) <= minBodyDis)
            {
                if (isGrounded)
                {
                    isGrounded = false;
                    LnewPos = leftGroundPos;
                    RnewPos = rightGroundPos;
                }
            }

            if (Vector3.Distance(cornerArmL.position, body.position) >= maxBodyDis)
            {
                if (isGrounded)
                {
                    isGrounded = false;
                    LnewPos = leftGroundPos;
                    RnewPos = rightGroundPos;
                }
            }

            if (Vector3.Distance(rightGroundPos, RtargetPos) >= maxTargetDis || Vector3.Distance(cornerArmR.position, body.position) <= minBodyDis)
            {
                if (isGrounded)
                {
                    isGrounded = false;
                    RnewPos = rightGroundPos;
                    LnewPos = leftGroundPos;
                }
            }

            if (Vector3.Distance(cornerArmR.position, body.position) >= maxBodyDis)
            {
                if (isGrounded)
                {
                    isGrounded = false;
                    RnewPos = rightGroundPos;
                    LnewPos = leftGroundPos;
                }
            }

        }else
        {
            if (Vector3.Distance(opLeftGroundPos, opLtargetPos) >= maxTargetDis || Vector3.Distance(opCornerArmL.position, body.position) <= minBodyDis)
            {
                if (isGrounded)
                {
                    isGrounded = false;
                    opLnewPos = opLeftGroundPos;
                    opRnewPos = opRightGroundPos;
                }
            }

            if (Vector3.Distance(opCornerArmL.position, body.position) >= maxBodyDis)
            {
                if (isGrounded)
                {
                    isGrounded = false;
                    opLnewPos = opLeftGroundPos;
                    opRnewPos = opRightGroundPos;
                }
            }

            if (Vector3.Distance(opRightGroundPos, RtargetPos) >= maxTargetDis || Vector3.Distance(opCornerArmR.position, body.position) <= minBodyDis)
            {
                if (isGrounded)
                {
                    isGrounded = false;
                    opRnewPos = opRightGroundPos;
                    opLnewPos = opLeftGroundPos;
                }
            }

            if (Vector3.Distance(opCornerArmR.position, body.position) >= maxBodyDis)
            {
                if (isGrounded)
                {
                    isGrounded = false;
                    opRnewPos = opRightGroundPos;
                    opLnewPos = opLeftGroundPos;
                }
            }

        }


        // Other Legs


        if (!isGrounded)
        {
            if (moveFirstPair)
            {
                Vector3 Lcenter = (LnewPos + LtargetPos) * 0.5f;
                Lcenter -= new Vector3(0, centerOffset, 0);

                Vector3 LstartPosCenter = LtargetPos - Lcenter;
                Vector3 LendPosCenter = LnewPos - Lcenter;

                Vector3 Rcenter = (RnewPos + RtargetPos) * 0.5f;
                Rcenter -= new Vector3(0, centerOffset, 0);

                Vector3 RstartPosCenter = RtargetPos - Rcenter;
                Vector3 RendPosCenter = RnewPos - Rcenter;


                stepTime += Time.deltaTime * legSpeed;
                float stepFraction = stepTime/maxStepTime;
                
                cornerArmL.position = Vector3.Slerp(LstartPosCenter, LendPosCenter, stepFraction);
                cornerArmL.position += Lcenter;

                cornerArmR.position = Vector3.Slerp(RstartPosCenter, RendPosCenter, stepFraction);
                cornerArmR.position += Rcenter;

                if (stepFraction >= 1f)
                {
                    stepTime = 0f;

                    isGrounded = true;
                    LtargetPos = LnewPos;
                    RtargetPos = RnewPos;
                    //moveFirstPair = false;
                    //StartCoroutine(moveFirstPairFalse());
                     Invoke(nameof(ChangeToFalse), 0.1f);
                }

            }else
            {
                Vector3 opLcenter = (opLnewPos + opLtargetPos) * 0.5f;
                opLcenter -= new Vector3(0, centerOffset, 0);

                Vector3 opLstartPosCenter = opLtargetPos - opLcenter;
                Vector3 opLendPosCenter = opLnewPos - opLcenter;

                Vector3 opRcenter = (opRnewPos + opRtargetPos) * 0.5f;
                opRcenter -= new Vector3(0, centerOffset, 0);

                Vector3 opRstartPosCenter = opRtargetPos - opRcenter;
                Vector3 opRendPosCenter = opRnewPos - opRcenter;


                opStepTime += Time.deltaTime * legSpeed;
                float stepFraction = opStepTime/maxStepTime;
                opCornerArmL.position = Vector3.Slerp(opLstartPosCenter, opLendPosCenter, stepFraction);
                opCornerArmL.position += opLcenter;

                opCornerArmR.position = Vector3.Slerp(opRstartPosCenter, opRendPosCenter, stepFraction);
                opCornerArmR.position += opRcenter;

                if (stepFraction >= 1f)
                {
                    opStepTime = 0f;

                    isGrounded = true;
                    opLtargetPos = opLnewPos;
                    opRtargetPos = opRnewPos;
                    //moveFirstPair = true;
                    //StartCoroutine(moveFirstPairTrue());
                    Invoke(nameof(ChangeToTrue), 0.1f);
                }
            }

        }else
        {
            cornerArmL.position = LtargetPos;
            cornerArmR.position = RtargetPos;
            opCornerArmL.position = opLtargetPos;
            opCornerArmR.position = opRtargetPos;
        }


        //Debug.Log(isGrounded);


    }

    private void ChangeToTrue()
    {
        moveFirstPair = true;
    }
    private void ChangeToFalse()
    {
        moveFirstPair = false;
    }

    // private IEnumerator moveFirstPairFalse()
    // {
    //     yield return new WaitForEndOfFrame();
    //     moveFirstPair = false;
    // }
    // private IEnumerator moveFirstPairTrue()
    // {
    //     yield return new WaitForEndOfFrame();
    //     moveFirstPair = true;
    // }

    private void moveLegs(Transform leg, Transform raycastOrigin, Vector3 targetPos, Vector3 newPos)
    {
        RaycastHit hit;
        Physics.Raycast(raycastOrigin.position, Vector3.down, out hit, raycastDis);

        Debug.DrawRay(raycastOrigin.position,  Vector3.down, Color.blue);

        Vector3 raycastGround;
        raycastGround = hit.point;

        if (Vector3.Distance(raycastGround, targetPos) >= maxTargetDis || Vector3.Distance(leg.position, body.position) <= minBodyDis)
        {
            if (isGrounded)
            {
                isGrounded = false;
                newPos = raycastGround;
            }
        }

        if (Vector3.Distance(leg.position, body.position) >= maxBodyDis)
        {
            if (isGrounded)
            {
                isGrounded = false;
                newPos = raycastGround;
            }
        }

        if (!isGrounded)
        {
            Vector3 center = (newPos + targetPos) * 0.5f;
            center -= new Vector3(0, centerOffset, 0);

            Vector3 startPosCenter = targetPos - center;
            Vector3 endPosCenter = newPos - center;

            stepTime += Time.deltaTime * legSpeed;
            float stepFraction = stepTime/maxStepTime;

            leg.position = Vector3.Slerp(startPosCenter, endPosCenter, stepFraction);
            leg.position += center;

            if (stepFraction >= 1f)
            {
                stepTime = 0f;

                isGrounded = true;
                targetPos = newPos;
            }

        }else
        {
            leg.position = targetPos;
        }

    }

}
