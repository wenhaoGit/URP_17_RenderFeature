using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class AimingScopeRenderPass : ScriptableRenderPass
{
    private Material mixerMaterial;

    private RTHandle crosshairRTH, aimingCameraRTH, aimingScopeRTH;
    private RenderTexture aimingCameraRT;

    private class CustomPassData
    {
        internal TextureHandle crosshairTH, aimingCameraTH, aimingScopeTH;
        internal Material material;
    }

    public void Init(Texture2D crosshair, RenderTexture aimingCamera, RenderTexture aimingScope)
    {
        profilingSampler = new ProfilingSampler("AimingScopePass");
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        mixerMaterial = new Material(Shader.Find("Shader Graphs/SG_Mixer"));

        crosshairRTH = RTHandles.Alloc(crosshair);
        aimingScopeRTH = RTHandles.Alloc(aimingScope);

        aimingCameraRT = aimingCamera;
    }

    public void Setup()
    {
        Texture2D aimingCameraTexture = new Texture2D(aimingCameraRT.width, aimingCameraRT.height);
        Graphics.ConvertTexture(aimingCameraRT, aimingCameraTexture);
        aimingCameraRTH = RTHandles.Alloc(aimingCameraTexture);
    }

    public void Dispose()
    {
        if (crosshairRTH != null)
        {
            crosshairRTH.Release();
        }
        if (aimingCameraRTH != null)
        {
            aimingCameraRTH.Release();
        }
        if (aimingScopeRTH != null)
        {
            aimingScopeRTH.Release();
        }

        if (Application.isPlaying)
        {
            Object.Destroy(mixerMaterial);
        }
        else
        {
            Object.DestroyImmediate(mixerMaterial);
        }
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        using (var builder = renderGraph.AddRasterRenderPass<CustomPassData>(passName, out CustomPassData data))
        {
            //passdata
            data.material = mixerMaterial;

            data.crosshairTH = renderGraph.ImportTexture(crosshairRTH);
            data.aimingCameraTH = renderGraph.ImportTexture(aimingCameraRTH);
            data.aimingScopeTH = renderGraph.ImportTexture(aimingScopeRTH);

            ////read
            builder.UseTexture(data.crosshairTH);
            builder.UseTexture(data.aimingCameraTH);

            //write
            builder.SetRenderAttachment(data.aimingScopeTH, 0, AccessFlags.ReadWrite);

            //process
            builder.SetRenderFunc<CustomPassData>((CustomPassData data, RasterGraphContext context) => ExecutePass(data, context));
        }
    }

    private void ExecutePass(CustomPassData data, RasterGraphContext context)
    {
        //process
        data.material.SetTexture("_A", data.crosshairTH);
        data.material.SetTexture("_B", data.aimingCameraTH);
        Blitter.BlitTexture(context.cmd, data.aimingScopeTH, new Vector4(1, 1, 0, 0), data.material, 0);
    }
}
