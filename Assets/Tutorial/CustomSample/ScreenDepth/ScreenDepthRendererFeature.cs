using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenDepthRendererFeature : ScriptableRendererFeature
{
    private ScreenDepthRenderPass renderPass;


    public override void Create()
    {
        renderPass = new ScreenDepthRenderPass();
        renderPass.Init();
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        renderPass.Setup();
    }

    protected override void Dispose(bool disposing)
    {
        renderPass.Dispose();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(renderPass);
    }
}
