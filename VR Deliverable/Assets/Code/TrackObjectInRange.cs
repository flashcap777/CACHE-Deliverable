using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/**
 * Smoothly increase the look target weight to
 * the camera, when it enters a certain range
 * and decreases it when the look target is behind
 * this Transform's Forward direction.
 */
public class TrackObjectInRange : MonoBehaviour
{
    [SerializeField] Transform TargetObject;
    [SerializeField] Transform LookAtTarget;
    [SerializeField] Transform ViewTransform;
    [SerializeField] float ActiveRange;
    [SerializeField] float WeightLerpSpeed;
    [SerializeField] float LookAtTurnSpeed;
    [SerializeField] float BlendToStopSpeed;
    [SerializeField] float SphereCastSize = 1f;
    [SerializeField] bool KeepMovingForward;
    [SerializeField] bool Sitting;
    float StoppingProgress;
    Rig MyRig;
    Animator MyAnimator;

    Vector3 EyeLevelTransformPosition() 
    {
        return ViewTransform.position;
    }
    float DetermineAngle() 
    {
        if (Sitting)
            return 60;
        if (!KeepMovingForward)
            return 70;
        else
            return 100f;
    }
    // Validate target can be looked at
    bool CanLookAtTarget() 
    {
        var myPos = EyeLevelTransformPosition();
        var lookAtPos = LookAtTarget.position;
        var offset = EyeLevelTransformPosition() - TargetObject.position;
        var vtForward = ViewTransform.forward;
        var ltForward = LookAtTarget.forward;
        vtForward.y = ltForward.y;
        offset.y = myPos.y;
        RaycastHit hit = new RaycastHit();
        if (KeepMovingForward && !Physics.SphereCast(lookAtPos, SphereCastSize, LookAtTarget.forward, out hit, ActiveRange, ~LayerMask.GetMask("Ignore Raycast")))
            return false;
        else if (hit.transform != null)
            Debug.Log(hit.transform.gameObject);
        if (Vector3.Distance(myPos, lookAtPos) <= ActiveRange
            && Vector3.Angle(ViewTransform.forward, offset) >= DetermineAngle())
        {
            if (Sitting)
            {
                vtForward.y = 0f;
                ltForward.y = 0f;
            }
            if (Vector3.Angle(vtForward, ltForward) >= 142f)
                return true;
        }
        return false;
    }
    void Awake()
    {
        MyRig = GetComponentInChildren<Rig>();
        MyAnimator = GetComponent<Animator>();
        MyRig.weight = 0f;
        if (Sitting)
            KeepMovingForward = false;
    }
    void Update()
    {
        Debug.DrawRay(ViewTransform.position, ViewTransform.forward, Color.yellow, 0.3f);
        TargetObject.position = Vector3.Lerp(TargetObject.position, LookAtTarget.position, LookAtTurnSpeed * Time.deltaTime);
        if (CanLookAtTarget())
        {
            MyRig.weight = Mathf.Lerp(MyRig.weight, 1f, WeightLerpSpeed * Time.deltaTime);
            if (StoppingProgress <= 1f)
                StoppingProgress += Time.deltaTime;
            else if (!KeepMovingForward)
                MyAnimator.SetBool("Stop", true);
        }
        else
        {
            MyRig.weight = Mathf.Lerp(MyRig.weight, 0f, 0.3f * WeightLerpSpeed * Time.deltaTime);
            StoppingProgress = Mathf.Lerp(StoppingProgress, 0f, 10f * Time.deltaTime);
            MyAnimator.SetBool("Stop", false);
        }
        if (!KeepMovingForward)
            MyAnimator.SetFloat("Blend", StoppingProgress);
    }
}
