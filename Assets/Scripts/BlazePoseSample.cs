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
    [SerializeField, Range(0f, 1f)] private float visibilityThreshold = 0.5f;

    private BlazePose pose;
    private PoseDetect.Result poseResult;
    private PoseLandmarkDetect.Result landmarkResult;
    private BlazePoseDrawer drawer;

    private UniTask<bool> task;
    private CancellationToken cancellationToken;
    private bool initialToggle = true;

    private void Start()
    {
        pose = new BlazePose(options);

        drawer = new BlazePoseDrawer(Camera.main, gameObject.layer);

        cancellationToken = this.GetCancellationTokenOnDestroy();

        GetComponent<WebCamInput>().OnTextureUpdate.AddListener(OnTextureUpdate);
    }

    private void OnDestroy()
    {
        GetComponent<WebCamInput>().OnTextureUpdate.RemoveListener(OnTextureUpdate);
        pose?.Dispose();
        drawer?.Dispose();
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
            drawer.DrawLandmarkResult(landmarkResult, visibilityThreshold, canvas.planeDistance);
        }
    }

    private void Invoke(Texture texture)
    {
        landmarkResult = pose.Invoke(texture);
        poseResult = pose.PoseResult;
    }

    private async UniTask<bool> InvokeAsync(Texture texture)
    {
        landmarkResult = await pose.InvokeAsync(texture, cancellationToken);
        poseResult = pose.PoseResult;

        return landmarkResult != null;
    }
}

