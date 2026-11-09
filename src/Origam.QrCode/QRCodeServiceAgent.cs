#region license
/*
Copyright 2005 - 2022 Advantage Solutions, s. r. o.

This file is part of ORIGAM (http://www.origam.org).

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with ORIGAM. If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
using Origam.Service.Core;
using QRCoder;
using SkiaSharp;
using System;
using System.Collections;

namespace Origam.QRCode;

public class QRCodeServiceAgent : IExternalServiceAgent
{
    public object Result { get; private set; }
    public virtual Hashtable Parameters { get; } = new();
    public string MethodName { get; set; } = "";
    public string TransactionId { get; set; } = null;
    public void Run()
    {
        switch (MethodName)
        {
            case "Create":
                string text = Parameters.Get<string>("InputText");
                int pixelsPerModule = 20;
                if(Parameters.ContainsKey("PixelPerModule"))
                {
                    pixelsPerModule = Parameters.Get<int>("PixelPerModule");
                }
                Result = Create(text, pixelsPerModule);
                break;
            case "Convert":
                byte[] data = Parameters.Get<byte[]>("Data");
                int width = Parameters.Get<int>("Width");
                int height = Parameters.Get<int>("Height");
                Result = FixedSizeBytes(data, width, height);
                break;
        }
    }
    private static object Create(string inputText, int pixelsPerModule)
    {
        QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qrCode = new(qrCodeData);
        return qrCode.GetGraphic(pixelsPerModule);
    }
    private static byte[] FixedSizeBytes(byte[] byteArrayImage, int width, int height)
    {
        using var codec = SKCodec.Create(new SKMemoryStream(byteArrayImage))
            ?? throw new ArgumentException(message: null, nameof(byteArrayImage));
        var sourceInfo = codec.Info
            .WithColorType(SKImageInfo.PlatformColorType)
            .WithAlphaType(SKAlphaType.Premul);
        using var sourceBitmap = new SKBitmap(sourceInfo);
        if (codec.GetPixels(sourceInfo, sourceBitmap.GetPixels()) != SKCodecResult.Success)
        {
            throw new ArgumentException(message: null, nameof(byteArrayImage));
        }
        sourceBitmap.SetImmutable();
        using var sourceImage = SKImage.FromBitmap(sourceBitmap);
        var sourceWidth = sourceImage.Width;
        var sourceHeight = sourceImage.Height;
        var destX = 0;
        var destY = 0;
        float percent;
        var percentWidth = width / (float)sourceWidth;
        var percentHeight = height / (float)sourceHeight;
        if (percentHeight < percentWidth)
        {
            percent = percentHeight;
            destX = (int)((width - sourceWidth * percent) / 2);
        }
        else
        {
            percent = percentWidth;
            destY = (int)((height - sourceHeight * percent) / 2);
        }
        var destWidth = Math.Max(1, (int)(sourceWidth * percent));
        var destHeight = Math.Max(1, (int)(sourceHeight * percent));

        using var backgroundImage = new SKBitmap(width, height);
        using var canvas = new SKCanvas(backgroundImage);
        canvas.Clear(SKColors.Black);
        canvas.DrawImage(
            sourceImage,
            SKRect.Create(destX, destY, destWidth, destHeight),
            new SKSamplingOptions(SKCubicResampler.CatmullRom));

        using var pngData = backgroundImage.Encode(SKEncodedImageFormat.Png, 100);
        return pngData.ToArray();
    }
}
