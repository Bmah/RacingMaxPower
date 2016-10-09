using UnityEngine;
using System.Collections;

public class HoverCarController : MonoBehaviour {
    internal enum SpeedType
    {
        MPH,
        KPH
    }
    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;
    public float turnSpeed = 5f;
    private float originalDrag;
    private float originalRadDrag;
    private bool Braking;
    public float Revs { get; private set; }
    public float AccelInput { get; private set; }
    public float BrakeInput { get; private set; }
    public float AppliedBrakeTorque { get; set; }
    public float AppliedMotorTorque { get; set; }
    private float m_GearFactor;
    [SerializeField] private SpeedType m_SpeedType;
    [SerializeField] private float m_Topspeed = 200;
    [SerializeField] private float m_ReverseTorque;
    [SerializeField] private float m_BrakeTorque;
    [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
    [SerializeField] private float m_MaxHandbrakeTorque;
    [SerializeField] private static int NoOfGears = 5;
    [SerializeField] private float m_RevRangeBoundary = 1f;
    [SerializeField] private float m_SlipLimit;
    [SerializeField] private float m_FullTorqueOverAllWheels;
    public float MaxSpeed { get { return m_Topspeed; } }
    private int m_GearNum;


    [Range(0, 1)][SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
    private Rigidbody m_Rigidbody;
    public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }

    private float m_OldRotation;
    // Use this for initialization
    void Start () {
        m_Rigidbody = GetComponent<Rigidbody>();
        originalDrag = m_Rigidbody.drag;
        originalRadDrag = m_Rigidbody.angularDrag;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void FixedUpdate ()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight))
        {
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            m_Rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }
    }

    public void Move(float steering, float accel, float footbrake, float handbrake)
    {
        

        //clamp input values
        
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        
        //SteerHelper();
        ApplyDrive(accel, footbrake, steering);
        CapSpeed();

        if (m_Rigidbody.angularDrag != originalRadDrag && handbrake <= 0f)
        {
            m_Rigidbody.angularDrag = originalRadDrag;
            
            Braking = false;

        }
        //Set the handbrake.
        //Assuming that wheels 2 and 3 are the rear wheels.
        if (handbrake > 0f && !Braking)
        {
            m_Rigidbody.angularDrag = m_Rigidbody.angularDrag * .25f;
            accel = accel * .75f;
            Braking = true;
        }


        CalculateRevs();
        GearChanging();

        //TractionControl();
    }

    private void SteerHelper()
    {
        

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }
    private void ApplyDrive(float accel, float footbrake, float steer)
    {
        
        m_Rigidbody.AddRelativeForce(0f, 0f, accel*m_Topspeed);
        //m_Rigidbody.AddRelativeForce(0f, 0f, footbrake * m_Topspeed);
        m_Rigidbody.AddRelativeTorque(0f, steer * turnSpeed, 0f);
        if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
        {
           AppliedBrakeTorque = m_BrakeTorque * footbrake;
        }
        else if (footbrake > 0)
        {
            AppliedBrakeTorque = 0f;
            AppliedMotorTorque = -m_ReverseTorque * footbrake;
        }
    }

    private void CapSpeed()
    {
        float speed = m_Rigidbody.velocity.magnitude;
        switch (m_SpeedType)
        {
            case SpeedType.MPH:

                speed *= 2.23693629f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
                break;

            case SpeedType.KPH:
                speed *= 3.6f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
                break;
        }
    }

    private void GearChanging()
    {
        float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
        float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
        float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

        if (m_GearNum > 0 && f < downgearlimit)
        {
            m_GearNum--;
        }

        if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
        {
            m_GearNum++;
        }
    }

    private void CalculateRevs()
    {
        // calculate engine revs (for display / sound)
        // (this is done in retrospect - revs are not used in force/power calculations)
        CalculateGearFactor();
        var gearNumFactor = m_GearNum / (float)NoOfGears;
        var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
        
        Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
    }
    private void CalculateGearFactor()
    {
        float f = (1 / (float)NoOfGears);
        // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
        // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
        var targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
        m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
    }

    // simple function to add a curved bias towards 1 for a value in the 0-1 range
    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }


    // unclamped version of Lerp, to allow value to exceed the from-to range
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }
}
