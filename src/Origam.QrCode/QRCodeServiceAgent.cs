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
using ImageMagick;
using ImageMagick.Formats;
using Origam.Service.Core;
using QRCoder;
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
        BitmapByteQRCode qrCode = new(qrCodeData);
        return qrCode.GetGraphic(pixelsPerModule);
    }
    private static byte[] FixedSizeBytes(byte[] byteArrayImage, int width, int height)
    {
        var bmpReadDefines = new BmpReadDefines
        {
            IgnoreFileSize = true
        };

        // Write to stream
        var settings = new MagickReadSettings();
        settings.SetDefines(bmpReadDefines);
        MagickImage imageInfo = new MagickImage(byteArrayImage, settings);
        var sourceWidth = imageInfo.Width;
        var sourceHeight = imageInfo.Height;
        var destX = 0;
        var destY = 0;
        float percent;
        var percentWidth = width / (float)sourceWidth;
        var percentHeight = height / (float)sourceHeight;
        if (percentHeight < percentWidth)
        {
            percent = percentHeight;
            destX = Convert.ToInt16((width - sourceWidth * percent) / 2);
        }
        else
        {
            percent = percentWidth;
            destY = Convert.ToInt16(
                (height - sourceHeight * percent) / 2);
        }
        var destWidth = (int)(sourceWidth * percent);
        var destHeight = (int)(sourceHeight * percent);
        var backgroundImage = new MagickImage(MagickColors.Black, width, height);
        backgroundImage.Resize(width, height);
        var pictureBitmap = new MagickImage(byteArrayImage, settings);
        pictureBitmap.Resize(destWidth, destHeight);
        backgroundImage.Composite(pictureBitmap, destX, destY);
        return backgroundImage.ToByteArray(imageInfo.Format);
    }
}
