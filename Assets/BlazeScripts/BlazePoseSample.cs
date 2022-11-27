using System.Threading;
using Cysharp.Threading.Tasks;
using TensorFlowLite;
using UnityEngine;

[RequireComponent(typeof(WebCamInput))]
public sealed class BlazePoseSample : MonoBehaviour
{
    [SerializeField] private BlazePose.Options options = default;

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private bool runBackground;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField, Range(0f, 1f)] private float visibilityThreshold = 0.5f;
    private BlazePose pose;
    private PoseLandmarkDetect.Result landmarkResult;
    private Camera camera;
    private UniTask<bool> task;
    private CancellationToken cancellationToken;
    private bool initialToggle = true;

    private void Start()
    {
        camera = Camera.main;
        pose = new BlazePose(options);

        cancellationToken = this.GetCancellationTokenOnDestroy();

        GetComponent<WebCamInput>().OnTextureUpdate.AddListener(OnTextureUpdate);
    }

    private void OnDestroy()
    {
        GetComponent<WebCamInput>().OnTextureUpdate.RemoveListener(OnTextureUpdate);
        pose?.Dispose();
    }

    private void OnTextureUpdate(Texture texture)
    {
        if (runBackground)
        {
            if (task.Status.IsCompleted())
            {
                task = InvokeAsync(texture);
            }
        }
        else
        {
            Invoke(texture);
        }
    }

    private void Update()
    {
        if (initialToggle)
        {
            GetComponent<WebCamInput>().ToggleCamera();
            initialToggle = false;
        }

        if (landmarkResult != null && landmarkResult.score > 0.2f)
        {
            float zoffset = canvas.planeDistance;
            Vector4[] landmarks = landmarkResult.viewportLandmarks;
            // Update world joints
            Vector3 p15 = camera.ViewportToWorldPoint(landmarks[15]);
            Vector3 p16 = camera.ViewportToWorldPoint(landmarks[16]);
            Vector3 p19 = camera.ViewportToWorldPoint(landmarks[19]);
            Vector3 p20 = camera.ViewportToWorldPoint(landmarks[20]);

            rightHand.transform.localPosition = new Vector3(p20.x, p20.y, p20.z+zoffset);
            rightHand.transform.localScale = new Vector3((p20.y-p16.y)*2.0f,(p20.y-p16.y)*2.0f,(p20.y-p16.y)*2.0f);
            leftHand.transform.localPosition = new Vector3(p19.x, p19.y, p19.z+zoffset);
            leftHand.transform.localScale = new Vector3((p19.y-p15.y)*2.0f,(p19.y-p15.y)*2.0f,(p19.y-p15.y)*2.0f);
            
        }
    }

    private void Invoke(Texture texture)
    {
        landmarkResult = pose.Invoke(texture);
    }

    private async UniTask<bool> InvokeAsync(Texture texture)
    {
        landmarkResult = await pose.InvokeAsync(texture, cancellationToken);

        return landmarkResult != null;
    }
}

