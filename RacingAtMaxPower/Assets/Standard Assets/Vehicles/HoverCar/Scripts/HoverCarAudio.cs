using UnityEngine;
using System.Collections;

public class HoverCarAudio : MonoBehaviour {
    public AudioSource jetSound;
    private float jetPitch;
    private const float LowPitch = .1f;
    private const float HighPitch = 2.0f;
    private const float SpeedToRevs = .001f;
    Vector3 myVelocity;
    Rigidbody carRigidbody;

    void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        myVelocity = carRigidbody.velocity;
        float forwardSpeed = transform.InverseTransformDirection(carRigidbody.velocity).z;
        float engineRevs = Mathf.Abs(forwardSpeed) * SpeedToRevs;
        jetSound.pitch = Mathf.Clamp(engineRevs, LowPitch, HighPitch);
    }

}