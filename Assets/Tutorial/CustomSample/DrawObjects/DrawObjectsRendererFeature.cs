using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DrawObjectsRendererFeature : ScriptableRendererFeature
{
    private DrawObjectsRenderPass renderPass;

    public Material overrideMaterial;
    public LayerMask filterLayerMask;

    public override void Create()
    {
        renderPass = new DrawObjectsRenderPass();
        renderPass.Init(overrideMaterial, filterLayerMask);
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
