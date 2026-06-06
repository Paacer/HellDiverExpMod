using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts
{
    public class EnqueueBlurPassR : MonoBehaviour
    {
        [SerializeField]
        private KawaseBlurSettings settings;
        private KawaseBlurPass blurPass;

        private void OnEnable()
        {
            blurPass = new KawaseBlurPass(settings);
            RenderPipelineManager.beginCameraRendering += OnBeginCamera;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
            blurPass.Dispose();
        }

        private void OnBeginCamera(ScriptableRenderContext context, Camera cam)
        {
            cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(blurPass);
        }
    }
}
