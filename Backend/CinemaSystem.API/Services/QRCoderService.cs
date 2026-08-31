#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Application.Common.Interfaces;
using CinemaSystem.Application.Interfaces;
using QRCoder;

namespace CinemaSystem.API.Services;

public class QRCoderService : IQRGeneratorService
{
    public Task<string> GenerateQRCodeAsync(string payload, CancellationToken ct)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);
        
        var base64 = Convert.ToBase64String(qrCodeImage);
        return Task.FromResult($"data:image/png;base64,{base64}");
    }
}
