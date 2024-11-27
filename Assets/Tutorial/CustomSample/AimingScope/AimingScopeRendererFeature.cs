using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AimingScopeRendererFeature : ScriptableRendererFeature
{
    private AimingScopeRenderPass renderPass;

    public Texture2D crosshair;
    public RenderTexture aimingCamerra, aimingScope;

    public override void Create()
    {
        renderPass = new AimingScopeRenderPass();
        renderPass.Init(crosshair, aimingCamerra, aimingScope);
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
        SetupRenderPasses(renderer, renderingData);
        renderer.EnqueuePass(renderPass);
    }
}
