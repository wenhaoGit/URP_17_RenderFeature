using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenDepthManager : MonoBehaviour
{
    private ScreenDepthRenderPass renderPass;

    private void OnEnable()
    {
        Create();
    }

    private void OnDisable()
    {
        Dispose();
    }

    public void Create()
    {
        renderPass = new ScreenDepthRenderPass();
        renderPass.Init();

        RenderPipelineManager.beginCameraRendering += SetupRenderPass;
    }

    public void Dispose()
    {
        renderPass.Dispose();

        RenderPipelineManager.beginCameraRendering -= SetupRenderPass;
    }

    public void SetupRenderPass(ScriptableRenderContext context, Camera cam)
    {
        renderPass.Setup();
        cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(renderPass);
    }
}
