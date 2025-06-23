using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    // Fields \\
    [Tooltip("If a CameraShake is not assigned, all cameras with a CameraShake component are targeted.")]
    public CameraShake camShake;
    public bool shakeOnStart = true;

    [Header("Shake Settings")]
    public float duration = 0.5f;
    public float posMagnitude = 0.25f;
    public float dirMagnitude = 0.25f;
    public float rotMagnitude = 0.5f;
    public float noiseFrequency = 5f;

    [Header("3D Settings")]
    public bool falloff = true;
    public float minFalloffDist = 20f;
    public float maxFalloffDist = 70f;

    // Methods \\
    private void Start()
    {
        if (shakeOnStart)
        {
            Shake();
        }
    }

    public void Shake()
    {
        CameraShake.ShakeProperties shake = new CameraShake.ShakeProperties(
            duration, 
            posMagnitude, 
            dirMagnitude, 
            rotMagnitude, 
            noiseFrequency, 
            transform.position,
            falloff,
            minFalloffDist,
            maxFalloffDist);

        if (camShake)
        {
            camShake.Shake(shake);
        }
        else
        {
            CameraShake.ShakeAll(shake);
        }
    }
}
