using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts
{
    [Serializable]
    public class KawaseBlurSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material blurMaterial = null;
        public Material sortingLayerBlitMaterial = null;

        [Range(2, 15)]
        public int blurIterations = 2;

        [Range(0, 4)]
        public int downsample = 1;

        public bool increasing = false;
    }

    public class KawaseBlurPass : ScriptableRenderPass
    {
        public class PassData
        {
            public Material blurMaterial;
            public Material sortingLayerBlitMaterial;
            public int iterations;
            public int downsample;
        }

        public PassData passData;

        private RTHandle source;
        private RTHandle rt_blurTexture;
        private RTHandle tmpRT1;
        private RTHandle tmpRT2;

        private const string k_offsetName = "_kawaseBlurOffset";
        private static readonly int offsetID = Shader.PropertyToID(k_offsetName);

        private const string k_blurTextureName = "_blurTexture";
        private static readonly int globalTextureID = Shader.PropertyToID(k_blurTextureName);

        //private static readonly ProfilingSampler profilingSampler = new ProfilingSampler("KawaseBlurPass");

        public KawaseBlurPass(KawaseBlurSettings settings)
        {
            passData = new PassData
            {
                blurMaterial = settings.blurMaterial,
                sortingLayerBlitMaterial = settings.sortingLayerBlitMaterial,
                iterations = settings.blurIterations,
                downsample = settings.downsample,
            };
            renderPassEvent = settings.renderPassEvent;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType != CameraType.Game)
                return;

            RenderTextureDescriptor desc = GetBlurDescriptor(renderingData.cameraData.cameraTargetDescriptor);

            source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            RenderingUtils.ReAllocateIfNeeded(
                ref rt_blurTexture,
                desc,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: k_blurTextureName);

            RenderingUtils.ReAllocateIfNeeded(
                ref tmpRT1,
                desc,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "KawaseBlurTmp1");

            RenderingUtils.ReAllocateIfNeeded(
                ref tmpRT2,
                desc,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "KawaseBlurTmp2");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType != CameraType.Game)
                return;

            if (source == null || passData.blurMaterial == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("KawaseBlurPass");

            Blitter.BlitCameraTexture(cmd, source, tmpRT1, passData.sortingLayerBlitMaterial, 0);

            RTHandle currentSource = tmpRT1;
            RTHandle currentDestination = tmpRT2;

            for (int i = 0; i < passData.iterations; i++)
            {
                float offset = passData.iterations - i + 1;

                cmd.SetGlobalFloat(offsetID, offset);
                Blitter.BlitCameraTexture(cmd, currentSource, currentDestination, passData.blurMaterial, 0);

                (currentDestination, currentSource) = (currentSource, currentDestination);
            }

            if (passData.iterations % 2 == 0)
            {
                Blitter.BlitCameraTexture(cmd, tmpRT1, rt_blurTexture);
            }
            else
            {
                Blitter.BlitCameraTexture(cmd, tmpRT2, rt_blurTexture);
            }
                
            cmd.SetGlobalTexture(globalTextureID, rt_blurTexture.nameID);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            source = null;
        }

        private RenderTextureDescriptor GetBlurDescriptor(RenderTextureDescriptor baseDescriptor)
        {
            RenderTextureDescriptor desc = baseDescriptor;

            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;
            desc.width = Mathf.Max(1, desc.width >> passData.downsample);
            desc.height = Mathf.Max(1, desc.height >> passData.downsample);

            return desc;
        }

        public void Dispose()
        {
            rt_blurTexture?.Release();
            tmpRT1?.Release();
            tmpRT2?.Release();

            rt_blurTexture = null;
            tmpRT1 = null;
            tmpRT2 = null;
            source = null;
        }
    }
}
