#if UNITY_URP
using UnityEngine.Rendering.Universal;
using UnityEngine;

namespace AEG.DLSS
{
    public class DLSSScriptableRenderFeature : ScriptableRendererFeature
    {
        [HideInInspector]
        public bool IsEnabled = false;

        private DLSS_URP m_dlssURP;

        private DLSSBufferPass fsrBufferPass;
        private DLSSRenderPass fsrRenderPass;

        private CameraData cameraData;

        public void OnSetReference(DLSS_URP _fsrURP) {
            m_dlssURP = _fsrURP;
            fsrBufferPass.OnSetReference(m_dlssURP);
            fsrRenderPass.OnSetReference(m_dlssURP);
        }

        public override void Create() {
            name = "DLSSScriptableRenderFeature";

            // Pass the settings as a parameter to the constructor of the pass.
            fsrBufferPass = new DLSSBufferPass(m_dlssURP);
            fsrRenderPass = new DLSSRenderPass(m_dlssURP);

            fsrBufferPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
        }

        public void OnDispose() {
        }

#if UNITY_2022_1_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) {
            fsrBufferPass.Setup();
        }
#endif

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            if(!IsEnabled) {
                return;
            }
            if(!Application.isPlaying) {
                return;
            }
            if(m_dlssURP == null) {
                return;
            }

            cameraData = renderingData.cameraData;
            if(cameraData.cameraType != CameraType.Game) {
                return;
            }
            if(cameraData.camera.GetComponent<DLSS_URP>() == null) {
                return;
            }
            if(!cameraData.resolveFinalTarget) {
                return;
            }

            // Here you can queue up multiple passes after each other.
            renderer.EnqueuePass(fsrBufferPass);
            renderer.EnqueuePass(fsrRenderPass);
        }
    }
}
#endif