using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPostProc : MonoBehaviour
{
    private Camera cam;
    private const int DownPass = 0;
    private const int UpPass = 1;
    private const int ApplyScenePass = 2;
    private const int ApplyBloomPass = 3;
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

        DualFilterIterations(ref currentSource, ref textures, ref width, ref height, ref format);

        ppBloomMat.SetTexture("_SourceTex", rt1);// original rt

        Graphics.Blit(currentSource, null as RenderTexture, ppBloomMat, 3);
        currentSource.DiscardContents();
        rt1.DiscardContents();
        RenderTexture.ReleaseTemporary(currentSource);
        RenderTexture.ReleaseTemporary(rt1);
        //RenderTexture.ReleaseTemporary(rt2); currentSource and rt2 refer to the same block of memory, don't release it twice 

    }

    // dual filtering, progressively scale down rt2 step-by-step, and scale it up back to normal resolution
    // do filtering / sampling every time it scales
    private void DualFilterIterations(ref RenderTexture currentSource, ref RenderTexture[] textures, ref int width, ref int height, ref RenderTextureFormat format)
    {
        RenderTexture currentDestination;
        int i;
        for (i = 0; i < 4; i++)
        {
            if (i > 0) // first, down-scale once without dual-fitering, and only do dual-filtering on smaller resolutions, which is too save performance
            {
                ppBloomMat.SetFloat("_HalfPixelX", 1f / (float)width);
                ppBloomMat.SetFloat("_HalfPixelY", 1f / (float)height);
            }
            width /= 2;
            height /= 2;
            // 
            if (height < 2 || width < 2)
            {
                break;
            }
            // in textures[], save a reference to the generated temporary render-texture, avoiding to generate RTs again in the up-scaling phase
            currentDestination = textures[i + 1] = RenderTexture.GetTemporary(width, height, 0, format);
            // DiscardContents to avoid read-back operations on mobile platforms, which costs performance
            currentDestination.DiscardContents();
            if (i > 0) // first, down-scale once without dual-fitering, and only do dual-filtering on smaller resolutions, which is too save performance
                Graphics.Blit(currentSource, currentDestination, ppBloomMat, DownPass);
            else
                Graphics.Blit(currentSource, currentDestination);
            currentSource.DiscardContents();
            // ping-pong operation between currentSource and currentDestination
            currentSource = currentDestination;
        }

        textures[i] = null;

        for (i -= 1; i >= 0; i--)
        {
            // in textures[], references to the generated temporary render-textures are saved, avoiding to generate RTs again in the up-scaling phase
            currentDestination = textures[i];
            textures[i] = null;
            width *= 2;
            height *= 2;
            if (i > 0) // last, up-scale once without dual-fitering, and by now dual-filtering has been applied on smaller resolutions, which is too save performance
            {
                ppBloomMat.SetFloat("_HalfPixelX", 1f / (float)width);
                ppBloomMat.SetFloat("_HalfPixelY", 1f / (float)height);
            }
            // DiscardContents to avoid read-back operations on mobile platforms, which costs performance
            currentDestination.DiscardContents();
            if (i > 0) // last, up-scale once without dual-fitering, and by now dual-filtering has been applied on smaller resolutions, which is too save performance
                Graphics.Blit(currentSource, currentDestination, ppBloomMat, UpPass);
            else
                Graphics.Blit(currentSource, currentDestination);
            currentSource.DiscardContents();
            RenderTexture.ReleaseTemporary(currentSource);
            // ping-pong operation between currentSource and currentDestination
            currentSource = currentDestination;
        }
    }


}