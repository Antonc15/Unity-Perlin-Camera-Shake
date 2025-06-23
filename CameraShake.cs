using UnityEngine;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    // Fields \\
    [Header("Position")]
    [SerializeField] private bool posLockX = false;
    [SerializeField] private bool posLockY = false;
    [SerializeField] private bool posLockZ = false;

    [Header("Rotation")]
    [SerializeField] private bool rotLockX = false;
    [SerializeField] private bool rotLockY = false;
    [SerializeField] private bool rotLockZ = false;

    // static
    private static List<CameraShake> camShakes = new List<CameraShake>();

    // instance
    private float duration = 0f;
    private float timer = 0f;
    private float posMagnitude = 0f;
    private float rotMagnitude = 0f;

    private float noiseFrequency = 0f;
    private Vector2 noiseEdge = Vector2.zero;
    private Vector2 noiseCenter = Vector2.zero;
    
    private Vector3 moveDir;

    // Methods \\
    private void OnEnable()
    {
        camShakes.Add(this);
    }

    private void OnDisable()
    {
        camShakes.Remove(this);
    }

    private void LateUpdate()
    {
        if (timer <= 0) { return; }

        timer -= Time.deltaTime;
        timer = Mathf.Max(timer, 0f);

        float t = timer / duration;
        Vector2 noisePoint = Vector2.Lerp(noiseEdge, noiseCenter, t);
        Vector3 displacement = new Vector3();

        displacement.x = Mathf.PerlinNoise(noisePoint.x, noisePoint.y);
        displacement.y = Mathf.PerlinNoise(noisePoint.x + 1000f, noisePoint.y + 1000f);
        displacement.z = Mathf.PerlinNoise(noisePoint.x + 10000f, noisePoint.y + 10000f);

        displacement.x = ((displacement.x * 2f) - 1f);
        displacement.y = ((displacement.y * 2f) - 1f);
        displacement.z = ((displacement.z * 2f) - 1f);

        // Apply shakes
        Vector3 pos = displacement;
        pos.x = posLockX ? 0f : pos.x;
        pos.y = posLockY ? 0f : pos.y;
        pos.z = posLockZ ? 0f : pos.z;

        Vector3 rot = displacement;
        rot.x = rotLockX ? 0f : rot.x;
        rot.y = rotLockY ? 0f : rot.y;
        rot.z = rotLockZ ? 0f : rot.z;

        transform.localPosition = (pos * posMagnitude * t) + (moveDir * t);
        transform.localEulerAngles = (rot * rotMagnitude * t);
    }

    public void Shake(ShakeProperties _shake)
    {
        // If falloff is enabled, adjust magnitude and duration
        if (_shake.falloff)
        {
            float dist = Vector3.Distance(transform.position, _shake.point);
            float t = Mathf.InverseLerp(_shake.maxFalloffDist, _shake.minFalloffDist, dist);

            _shake.duration *= t;
            _shake.posMag *= t;
            _shake.dirMag *= t;
            _shake.rotMag *= t;
        }

        // Only play the new shake if it is more important than the current one
        float currentPriority = (posMagnitude + rotMagnitude) * (timer / duration);
        float newPriority = _shake.posMag + _shake.rotMag;

        if (currentPriority > newPriority)
        {
            return;
        }

        // Apply move direction
        moveDir = transform.position - _shake.point;
        moveDir.x = posLockX ? 0f : moveDir.x;
        moveDir.y = posLockY ? 0f : moveDir.y;
        moveDir.z = posLockZ ? 0f : moveDir.z;
        moveDir = moveDir.normalized * _shake.dirMag;

        // Apply the new shake properties
        duration = _shake.duration;
        timer = duration;
        posMagnitude = _shake.posMag;
        rotMagnitude = _shake.rotMag;
        noiseFrequency = _shake.frequency;

        // Generate new noise values
        noiseEdge = new Vector2(Random.value * 10f, Random.value * 10f);
        noiseCenter = noiseEdge + Vector2.one * noiseFrequency;
    }

    // Static Methods \\
    public static void ShakeAll(ShakeProperties _shake)
    {
        // Walk backwards to enable element removal during iteration
        for (int i = camShakes.Count - 1; i >= 0; i--)
        {
            camShakes[i].Shake(_shake);
        }
    }

    // Classes \\
    public struct ShakeProperties
    {
        // shake
        public float duration;
        public float posMag;
        public float dirMag;
        public float rotMag;

        // noise
        public float frequency;

        // world
        public Vector3 point;
        public bool falloff;
        public float minFalloffDist;
        public float maxFalloffDist;


        public ShakeProperties(
            float _duration, 
            float _posMag, 
            float _dirMag, 
            float _rotMag, 
            float _frequency, 
            Vector3 _point,
            bool _falloff,
            float _minFalloffDist,
            float _maxFalloffDist) 
        {
            duration = _duration;
            posMag = _posMag;
            dirMag = _dirMag;
            rotMag = _rotMag;

            frequency = _frequency;
            point = _point;

            falloff = _falloff;
            minFalloffDist = _minFalloffDist;
            maxFalloffDist = _maxFalloffDist;
        }
    }
}
