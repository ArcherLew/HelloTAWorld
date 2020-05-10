using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPostProc : MonoBehaviour
{
    private Camera cam;
    private RenderTexture rt1;
    private RenderTexture rt2;
    private RenderBuffer[] _mrt;
    private Material ppBloomMat;
    public Color bloomColor = Color.yellow;

    void OnEnable()
    {
        _mrt = new RenderBuffer[2];
        cam = transform.GetComponent<Camera>();
        ppBloomMat = new Material(Shader.Find("MyWorld/PostProc Bloom"));
        ppBloomMat.hideFlags = HideFlags.DontSave;
        ppBloomMat.SetColor("_BloomColor", bloomColor);
    }
    void OnDisable()
    {
        if (ppBloomMat)
        {
            DestroyImmediate(ppBloomMat);
            ppBloomMat = null;
        }
    }

    void OnPreRender()
    {
        int resWidth = Screen.currentResolution.width; // width of resolution
        int resHeight = Screen.currentResolution.height; // height of resolution

        // render-texture to store other normal result except the bloom, 
        // later to be blended with bloom
        rt1 = RenderTexture.GetTemporary(resWidth, resHeight, 16);
        // render-texture to store the bloom
        rt2 = RenderTexture.GetTemporary(resWidth, resHeight, 0, RenderTextureFormat.R8);

        _mrt[0] = rt1.colorBuffer;
        _mrt[1] = rt2.colorBuffer;

        cam.backgroundColor = Color.clear;
        // let camera to render into 2 targets, 
        // rendering normal result and bloom simultaneously using multiple-render-targets (MRT)
        cam.SetTargetBuffers(_mrt, rt1.depthBuffer);

    }

    // for every frame, after the camera renders, 
    void OnPostRender()
    {

        cam.targetTexture = null; //null means framebuffer

        int width = rt2.width;
        int height = rt2.height;
        RenderTextureFormat format = rt2.format;

        RenderTexture[] textures = new RenderTexture[16];

        RenderTexture currentSource = textures[0] = rt2;// threshold rt

        // DualFilterIterations(ref currentSource, ref textures, ref width, ref height, ref format);

        ppBloomMat.SetTexture("_SourceTex", rt1);// original rt

        Graphics.Blit(currentSource, null as RenderTexture, ppBloomMat, 3);
        currentSource.DiscardContents();
        rt1.DiscardContents();
        RenderTexture.ReleaseTemporary(currentSource);
        RenderTexture.ReleaseTemporary(rt1);
        //RenderTexture.ReleaseTemporary(rt2); currentSource and rt2 refer to the same block of memory, don't release it twice 

    }


}