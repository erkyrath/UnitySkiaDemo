// Demo file for SkiaSharp drawing in a Unity project.
// Written by Andrew Plotkin <erkyrath@eblong.com>
// https://github.com/erkyrath/UnitySkiaDemo
// This file is in the public domain.

using System;
using UnityEngine;
using UnityEngine.UI;

using SkiaSharp;

public class RawImageDraw : MonoBehaviour
{
    void Start()
    {
        Debug.Log("RawImageDraw starting...");

        RawImage rawImage = GetComponent<RawImage>();
        Debug.Log("Found image: " + rawImage);

        // Prepare to create an image, 256x256 pixels.
        var info = new SKImageInfo(256, 256);
        Debug.Log("SKInfo: " + info);
        // If you get past here without a DLL loading error, it's smooth sailing.

        // Create the Skia drawing surface and canvas.
        var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;

        // Create a paint object. This stores all your drawing attributes (color, line width, whether you're stroking or filling, etc).
        var paint = new SKPaint();
        paint.IsStroke = false;

        // Draw a red circle.
        paint.Color = new SKColor(255, 0, 0, 255); // red
        canvas.DrawOval(128, 128, 128, 128, paint);

        // Draw a small green square.
        paint.Color = new SKColor(0, 255, 0, 255); // green
        canvas.DrawRect(new SKRect(0, 118, 20, 138), paint);

        // Draw a small blue square.
        paint.Color = new SKColor(0, 0, 255, 255); // blue
        canvas.DrawRect(new SKRect(236, 118, 256, 138), paint);

        // Switch to yellow stroking.
        paint.IsStroke = true;
        paint.Color = new SKColor(255, 255, 0, 192); // translucent yellow
        paint.StrokeWidth = 20;

        // Draw a triangle.
        var path = new SKPath();
        path.MoveTo(128, 20);
        path.LineTo(236, 236);
        path.LineTo(20, 236);
        path.Close();
        canvas.DrawPath(path, paint);

        // Draw a closed curve with rounded corners.
        path = new SKPath();
        path.MoveTo(20, 20);
        path.CubicTo(20, 128,  236, 128,  236, 20);
        path.Close();
        paint.StrokeJoin = SKStrokeJoin.Round;
        paint.StrokeWidth = 30;
        canvas.DrawPath(path, paint);

        // Okay, we're finished drawing. Now we create a Unity texture.
        TextureFormat format = (info.ColorType == SKColorType.Rgba8888) ? TextureFormat.RGBA32 : TextureFormat.BGRA32;
        var texture = new Texture2D(info.Width, info.Height, format, false, true);
        texture.wrapMode = TextureWrapMode.Clamp;

        // Pull a Skia image object out of the canvas...
        var pixmap = surface.PeekPixels();
        // Copy it to the Unity texture...
        texture.LoadRawTextureData(pixmap.GetPixels(), pixmap.RowBytes * pixmap.Height);
        texture.Apply(false, true);
        // And drop it into the RawImage object.
        rawImage.texture = texture;

        // Dispose all our (disposable) Skia objects.
        pixmap.Dispose();
        canvas.Dispose();
        surface.Dispose();

        Debug.Log("Draw complete.");

    }
}
