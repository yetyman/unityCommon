using Assets.GenericClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Animation;
using UnityEngine.Events;
using Assets.Scripts;

public class Connector : MonoBehaviour
{
    public UnityEvent ResetConnectionDistance;
    public UnityEvent Connected;

    bool EndpointsChanged = false;
    private GameObject LastConnectedBegin;
    private GameObject LastConnectedEnd;

    public GameObject Begin;
    public GameObject End;
    public GameObject EndPoint;
    public GameObject BeginPoint;
    public GameObject ScaledTransform;
    [SerializeField]
    public Vec3 OverrideStart;
    [SerializeField]
    public Vec3 OverrideEnd;
    public bool StartAtZeroLength;
    public float TimeToExtend;

    public Camera ForwardCamera;
    [SerializeField]
    public Vec3 OverrideForwardCamera;


    bool hasStart => CurrentStart != null;
    bool hasEnd => CurrentEnd != null;
    Vector3? CurrentStart => OverrideStart.HasValue ? OverrideStart.Value : Begin?.transform?.localPosition;
    Vector3? CurrentEnd => OverrideEnd.HasValue ? OverrideEnd.Value : End?.transform?.localPosition;
    Vector3? CurrentForward => OverrideForwardCamera.HasValue ? OverrideForwardCamera.Value : ForwardCamera?.transform?.localPosition;

    Vector3 LastStart;
    Vector3 LastEnd;
    Vector3 LastForward;

    EasingCurves EasingCurve = EasingCurves.Linear;

    private float ConnectionDistancePercentage = -1;
    private float OldConnectionDistancePercentage;
    // Start is called before the first frame update
    void Start()
    {
        if(ForwardCamera == null)
        {
            ForwardCamera = Camera.main;
        }
        if (!hasStart || !hasEnd)
        {

            Debug.LogError("You must assign start and end to a connector");
        }
        else
        {
            if (StartAtZeroLength)
            {
                Move(0, 0);//set it up at the start location
                Move(1, TimeToExtend);//animate it extending to the end location.
            }
            else
            {
                Move(1, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(CurrentStart != LastStart || CurrentEnd != LastEnd)
        {
            Move();
        }
    }

    public void Extend()
    {
        Move(1, TimeToExtend);//animate it extending to the end location.
    }
    public void Retract()
    {
        Move(0, TimeToExtend);
    }

    void Move(float initializedDistance = 1f, float transitiontime = 0)
    {
        if (hasStart && hasEnd)
        {
            LastStart = CurrentStart.Value;
            LastEnd = CurrentEnd.Value;
            LastForward = CurrentForward.Value;

            //ughh.... for some reason its scaling from the side. so "center" it on the start point...
            //var center = LastStart.Average(LastEnd, initializedDistance/2);
            var center = LastStart;// LastStart.Average(LastEnd, initializedDistance/2);

            //get visible plane rotation right. only affects Z axis?
            //var planeRotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(LastEnd.y - LastStart.y, LastEnd.x - LastStart.x), Vector3.forward);//Quaternion.LookRotation(Vector3.forward, Vector3.Cross(LastEnd - LastStart, Vector3.back));//this other one is useful if we start using the camera to determine forward instead of normal forward.
            //get depth roation right. affects other axes.


            //i need the forward direction of the connector to always be facing forward to the camera.
            //this is complicated by the angle of the start and end points of the connection. 

            //determine angle of rotation on the actual forward axis(in this case, from start to the camera or just forward as we have been doing it).
            //determine the rotation of the object such that forward direction is the same as the forward axis
            //on the axis which is the cross product of the forward axis and the axis determined in the previous step find the rotation backwards to reach the endpoint. wow this is complicated.
            //:/


            var forwardDirection = LastForward;
            //else if (!ForwardCamera.orthographic)
            //    forwardDirection -= LastStart;

            var rotation = Quaternion.LookRotation(LastEnd - LastStart, forwardDirection);

            //////var rotation = Quaternion.LookRotation(Quaternion.FromToRotation(Vector3.forward, LastEnd- LastStart) * Vector3.forward, Vector3.forward);

            var scale = LastStart.Hypotenuse(LastEnd) * initializedDistance;

            MoveImpl(initializedDistance,transitiontime, center, scale, rotation);
        }
    }



    private AnimationInstanceQuat<GameObject> ConnectorRotator = null;
    private AnimationInstanceVec3<GameObject> ConnectorMover = null;
    private AnimationInstanceVec3<GameObject> EndpointMover = null;
    private AnimationInstanceVec3<GameObject> ScaledObjectMover = null;
    private AnimationInstanceVec3<GameObject> ScaledObjectScaler = null;
    protected virtual void MoveImpl(float initializedDistance, float transitiontime, Vector3 center, float scale, Quaternion rotation)
    {
        OldConnectionDistancePercentage = ConnectionDistancePercentage;
        ConnectionDistancePercentage = initializedDistance;

        ConnectorMover = gameObject.MoveTo(center, transitiontime, EasingCurve, EasingPatterns.In, ConnectorMover);

        ConnectorRotator = gameObject.RotateTo(rotation, transitiontime, EasingCurve, EasingPatterns.In, ConnectorRotator);

        if (ScaledTransform != null)
        {
            ScaledObjectScaler = ScaledTransform.ScaleTo(new Vector3(ScaledTransform.transform.localScale.x, ScaledTransform.transform.localScale.y, scale), transitiontime, EasingCurve, EasingPatterns.In, ScaledObjectScaler);
            ScaledObjectMover = ScaledTransform.MoveTo(new Vector3(0, 0, scale / 2), transitiontime, EasingCurve, EasingPatterns.In, ScaledObjectMover);
        }
        else
            ScaledObjectScaler = gameObject.ScaleTo(new Vector3(transform.localScale.x, transform.localScale.y, scale), transitiontime, EasingCurve, EasingPatterns.In, ScaledObjectScaler);
        if (initializedDistance == 0 || initializedDistance == 1)
            ScaledObjectScaler.Ended += MovementEnding;

        if (EndPoint != null)
        {
            EndpointMover = EndPoint.MoveTo(new Vector3(0, 0, scale), transitiontime, EasingCurve, EasingPatterns.In, EndpointMover);
        }
    }
    public void MovementEnding(IPlayable instance)
    {
        instance.Ended -= MovementEnding;
        if (!instance.IsCancelled)
        {
            EndpointsChanged = LastConnectedEnd != End || LastConnectedBegin != Begin;
            LastConnectedEnd = End;
            LastConnectedBegin = Begin;
            if (OldConnectionDistancePercentage != ConnectionDistancePercentage || EndpointsChanged)
            {
                if (ConnectionDistancePercentage == 1)
                {
                    Connected?.Invoke();
                }
                else if (ConnectionDistancePercentage == 0)
                {
                    ResetConnectionDistance?.Invoke();
                }
            }
        }
    }
}
