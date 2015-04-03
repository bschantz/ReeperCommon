﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReeperCommon.Extensions
{
    public static class Texture2DExtensions
    {
        public static Texture2D CreateReadable(this UnityEngine.Texture2D original)
        {
            if (original.width == 0 || original.height == 0)
                throw new Exception("CreateReadable: Original has zero width or height or both");

            Texture2D finalTexture = new Texture2D(original.width, original.height);

            // nbTexture isn't read or writeable ... we'll have to get tricksy
            var rt = RenderTexture.GetTemporary(original.width, original.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB, 1);
            Graphics.Blit(original, rt);

            RenderTexture.active = rt;

            finalTexture.ReadPixels(new Rect(0, 0, finalTexture.width, finalTexture.height), 0, 0);

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            return finalTexture;
        }


        public static void GenerateRandom(this UnityEngine.Texture2D tex)
        {
            var pixels = tex.GetPixels32();

            for (int y = 0; y < tex.height; ++y)
                for (int x = 0; x < tex.width; ++x)
                    pixels[y * tex.width + x] = new Color(UnityEngine.Random.Range(0f, 1f),
                                                            UnityEngine.Random.Range(0f, 1f),
                                                            UnityEngine.Random.Range(0f, 1f),
                                                            UnityEngine.Random.Range(0f, 1f));

            tex.SetPixels32(pixels);
            tex.Apply();
        }


        public static Texture2D GenerateRandom(int w, int h)
        {
            var tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            tex.GenerateRandom();
            return tex;
        }


        public static void SetOpacity(this UnityEngine.Texture2D original, int opacity, int mipLevel = 0)
        {
            opacity = Mathf.Clamp(opacity, 0, 255);

            try
            {
                var pixels = original.GetPixels32(mipLevel);

                for (int i = 0; i < pixels.Length; ++i)
                    pixels[i].a = (byte) opacity;

                original.SetPixels32(pixels);
                original.Apply();
            }
            catch (UnityException)
            {
                // texture isn't readable
                try
                {
                    original.CreateReadable().SetOpacity(opacity, mipLevel);
                }
                catch
                {
                    throw new Exception("Original texture not readable and failed to create readable version");
                }
            }
        }



        /// <summary>
        /// Saves texture into plugin dir with supplied name.
        /// Precondition: texture must be readable
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="name"></param>
        public static bool SaveToDisk(this UnityEngine.Texture2D texture, string pathInGameData)
        {
            // texture format - needs to be ARGB32, RGBA32, RGB24 or Alpha8
            var validFormats = new List<TextureFormat>{ TextureFormat.Alpha8, 
                                                        TextureFormat.RGB24,
                                                        TextureFormat.RGBA32,
                                                        TextureFormat.ARGB32};

            if (!validFormats.Contains(texture.format))
                return CreateReadable(texture).SaveToDisk(pathInGameData);
            

            if (pathInGameData.StartsWith("/"))
                pathInGameData = pathInGameData.Substring(1);

            pathInGameData = "/GameData/" + pathInGameData;

            if (!pathInGameData.EndsWith(".png"))
                pathInGameData += ".png";

            try
            {
                var file = new System.IO.FileStream(KSPUtil.ApplicationRootPath + pathInGameData, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                var writer = new System.IO.BinaryWriter(file);
                writer.Write(texture.EncodeToPNG());

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static Texture2D As2D(this UnityEngine.Texture tex)
        {
            return tex as Texture2D;
        }
    }
}
