using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private CinemachineCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private Coroutine shakeCoroutine;

    // �̱���
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
            Debug.Log("Virtual Camera�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        noise = virtualCamera.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

        if (noise == null)
        {
            Debug.Log("Perlin Noise Component�� Virtual Camera�� �����ϴ�!");
        }
    }

    // ī�޶� ����ũ (����, �ð�)
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

    // ��ž ����ŷ
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
