using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(WebCamInput))]
public sealed class BeforeGameController : MonoBehaviour
{
    [SerializeField] private BlazePose.Options options = default;
    [SerializeField] private bool runBackground;
    [SerializeField, Range(0f, 1f)] private float visibilityThreshold = 0.5f;
    private BlazePose pose;
    private PoseLandmarkDetect.Result landmarkResult;
    private UniTask<bool> task;
    private CancellationToken cancellationToken;
    private bool initialToggle = true;
    private int isStartForOkay = 0;
    private int maxSliderValue = 0;
    public Slider slider;

    private void Start()
    {
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

        int points = 0;
        if (landmarkResult != null && landmarkResult.score > 0.2f)
        {
            Vector4[] landmarks = landmarkResult.viewportLandmarks;

            for (int i = 0; i < 27; i++)
            {
                if (landmarks[i].w >= visibilityThreshold)
                {
                    points += 1;
                }
            }

            if (points == 27)
            {
                isStartForOkay += 1;
                if (isStartForOkay > maxSliderValue)
                {
                    slider.value = isStartForOkay * 1.0f;
                    maxSliderValue = isStartForOkay;
                }
            }
            else if (isStartForOkay > 0)
                isStartForOkay--;
            
            if (isStartForOkay > 15)
            {
                StartCoroutine(waitOneSec());
            }
        }
    }

    private IEnumerator waitOneSec()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("PoseGame");
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

