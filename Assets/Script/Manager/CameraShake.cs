using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private CinemachineCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private Coroutine shakeCoroutine;

    // 싱글톤
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.Log("Virtual Camera가 할당되지 않았습니다.");
            return;
        }

        noise = virtualCamera.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

        if (noise == null)
        {
            Debug.Log("Perlin Noise Component가 Virtual Camera에 없습니다!");
        }
    }

    // 카메라 셰이크 (강도, 시간)
    public void Shake(float intensity, float duration)
    {
        if (noise == null) return;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        noise.AmplitudeGain = intensity;

        yield return new WaitForSeconds(duration);

        StopShake();
    }

    // 스탑 셰이킹
    public void StopShake()
    {
        if (noise != null)
        {
            noise.AmplitudeGain = 0f;
        }

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
    }

    [ContextMenu("Shake")]
    public void Test()
    {
        Shake(5, 0.5f);
    }
}
