using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class ScreenDepthRenderPass : ScriptableRenderPass
{
    private Material material;

    private class CustomPassData
    {
        internal TextureHandle screenColor;
        internal Material overrideMaterial;
    }

    public void Init()
    {
        profilingSampler = new ProfilingSampler("ScreenDepthRenderPass");
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        material = new Material(Shader.Find("Shader Graphs/SG_ScreenDepth"));
    }

    public void Setup()
    {
    }

    public void Dispose()
    {
        if (Application.isPlaying)
        {
            Object.Destroy(material);
        }
        else
        {
            Object.DestroyImmediate(material);
        }
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        using (var builder = renderGraph.AddRasterRenderPass<CustomPassData>(passName, out CustomPassData data))
        {
            //passdata
            data.screenColor = frameData.Get<UniversalResourceData>().activeColorTexture;
            data.overrideMaterial = material;

            //read
            //builder.UseTexture(data.screenColor);

            //write
            builder.SetRenderAttachment(data.screenColor, 0, AccessFlags.ReadWrite);

            //process
            builder.SetRenderFunc<CustomPassData>((CustomPassData data, RasterGraphContext context) => ExecutePass(data, context));
        }
    }

    private void ExecutePass(CustomPassData data, RasterGraphContext context)
    {
        //process
        Blitter.BlitTexture(context.cmd, data.screenColor, new Vector4(1, 1, 0, 0), data.overrideMaterial, 0);
    }
}
