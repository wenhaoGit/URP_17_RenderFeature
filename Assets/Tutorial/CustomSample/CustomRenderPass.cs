using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class CustomRenderPass : ScriptableRenderPass
{
    private class CustomPassData
    {

    }

    public void Init()
    {
        profilingSampler = new ProfilingSampler("CustomRenderPass");
        renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
    }

    public void Setup()
    {
    }

    public void Dispose()
    {
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        base.RecordRenderGraph(renderGraph, frameData);

        using (var builder = renderGraph.AddRasterRenderPass<CustomPassData>(passName, out CustomPassData data))
        {
            //passdata
            //data.xx = 

            //read
            //builder.UseTexture  builder.UseRendererList

            //write
            //builder.SetRenderAttachment 

            //process
            builder.SetRenderFunc<CustomPassData>((CustomPassData data, RasterGraphContext context) => ExecutePass(data, context));
        }
    }

    private void ExecutePass(CustomPassData data, RasterGraphContext context)
    {
        //process
        //Blitter.blit  context.cmd.DrawMesh 
    }
}
