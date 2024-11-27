using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
public class DrawObjectsRenderPass : ScriptableRenderPass
{
    private Material initOverrideMaterial;
    private LayerMask initFilterLayerMask;
    private Material overrideMaterial = null;
    private LayerMask filterLayerMask = 0;

    private class CustomPassData
    {
        internal RendererListHandle rendererList;
    }

    public void Init(Material material, LayerMask layermask)
    {
        profilingSampler = new ProfilingSampler("DrawObjects");
        renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;

        initOverrideMaterial = material;
        initFilterLayerMask = layermask;
    }

    public void Setup()
    {
        DrawObjectsVolume volume = VolumeManager.instance.stack.GetComponent<DrawObjectsVolume>();

        overrideMaterial = (volume != null && volume.overrideMaterial.overrideState) ? volume.overrideMaterial.value : initOverrideMaterial;
        filterLayerMask = (volume != null && volume.filterLayerMask.overrideState) ? volume.filterLayerMask.value : initFilterLayerMask;
    }

    public void Dispose()
    {
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        using (var builder = renderGraph.AddRasterRenderPass<CustomPassData>(passName, out CustomPassData data))
        {
            //passdata
            UniversalRenderingData rendereringData = frameData.Get<UniversalRenderingData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            CullingResults cullingResults = rendereringData.cullResults;
            ShaderTagId shaderTagdId = new ShaderTagId("UniversalForward");
            SortingCriteria sortingCriteria = cameraData.defaultOpaqueSortFlags;
            DrawingSettings drawingSettings = RenderingUtils.CreateDrawingSettings(shaderTagdId, rendereringData, cameraData, lightData, sortingCriteria);
            drawingSettings.overrideMaterial = overrideMaterial;
            RenderQueueRange renderQueueRange = RenderQueueRange.all;
            FilteringSettings filteringSettings = new FilteringSettings(renderQueueRange, filterLayerMask);
            RendererListParams rendererListParams = new RendererListParams(cullingResults, drawingSettings, filteringSettings);
            data.rendererList = renderGraph.CreateRendererList(rendererListParams);

            //read
            builder.UseRendererList(data.rendererList);

            //write
            builder.SetRenderAttachment(frameData.Get<UniversalResourceData>().activeColorTexture, 0);
            builder.SetRenderAttachmentDepth(frameData.Get<UniversalResourceData>().activeDepthTexture);

            //process
            builder.SetRenderFunc<CustomPassData>((CustomPassData data, RasterGraphContext context) => ExecutePass(data, context));
        }
    }

    private void ExecutePass(CustomPassData data, RasterGraphContext context)
    {
        //process
        context.cmd.DrawRendererList(data.rendererList);
    }
}
