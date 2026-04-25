using ImageMagick;
using Xunit;

namespace Origam.QRCode.Tests
{
    public class QRCodeServiceAgentTests
    {
        public QRCodeServiceAgentTests()
        {
        }

        [Fact]
        void RunTest()
        {
            var qrCodeServiceAgent = new QRCodeServiceAgent
            {
                MethodName = "Create"
            };
            qrCodeServiceAgent.Parameters.Add("InputText", "Hello, ORIGAM!");

            qrCodeServiceAgent.Run();

            Assert.NotNull(qrCodeServiceAgent.Result);
            var bytes = Assert.IsType<byte[]>(qrCodeServiceAgent.Result);
            Assert.NotEmpty(bytes);

            // TODO: temporary — dump generated QR for visual inspection
            var outPath = Path.Combine(AppContext.BaseDirectory, "qr-output.png");
            File.WriteAllBytes(outPath, bytes);
        }

        private static byte[] CreateQrBytes(string text = "Hello, ORIGAM!")
        {
            var agent = new QRCodeServiceAgent { MethodName = "Create" };
            agent.Parameters.Add("InputText", text);
            agent.Run();
            return (byte[])agent.Result!;
        }

        [Fact]
        void ConvertTest_ReturnsNonEmptyBytes()
        {
            var source = CreateQrBytes();

            var agent = new QRCodeServiceAgent { MethodName = "Convert" };
            agent.Parameters.Add("Data", source);
            agent.Parameters.Add("Width", 300);
            agent.Parameters.Add("Height", 300);

            agent.Run();

            Assert.NotNull(agent.Result);
            var bytes = Assert.IsType<byte[]>(agent.Result);
            Assert.NotEmpty(bytes);
        }

        [Theory]
        [InlineData(100, 100)]
        [InlineData(300, 300)]
        [InlineData(512, 256)]
        [InlineData(256, 512)]
        void ConvertTest_ProducesImageWithRequestedDimensions(int width, int height)
        {
            var source = CreateQrBytes();

            var agent = new QRCodeServiceAgent { MethodName = "Convert" };
            agent.Parameters.Add("Data", source);
            agent.Parameters.Add("Width", width);
            agent.Parameters.Add("Height", height);

            agent.Run();

            var bytes = Assert.IsType<byte[]>(agent.Result);
            using var image = new MagickImage(bytes);
            Assert.Equal(width, (int)image.Width);
            Assert.Equal(height, (int)image.Height);
        }

        [Fact]
        void ConvertTest_PreservesPngFormat()
        {
            var source = CreateQrBytes();

            var agent = new QRCodeServiceAgent { MethodName = "Convert" };
            agent.Parameters.Add("Data", source);
            agent.Parameters.Add("Width", 256);
            agent.Parameters.Add("Height", 256);

            agent.Run();

            var bytes = Assert.IsType<byte[]>(agent.Result);
            using var image = new MagickImage(bytes);
            Assert.Equal(MagickFormat.Png, image.Format);
        }

        [Fact]
        void ConvertTest_DumpsResizedQrForInspection()
        {
            var source = CreateQrBytes();

            var agent = new QRCodeServiceAgent { MethodName = "Convert" };
            agent.Parameters.Add("Data", source);
            agent.Parameters.Add("Width", 400);
            agent.Parameters.Add("Height", 200);

            agent.Run();

            var bytes = Assert.IsType<byte[]>(agent.Result);
            var outPath = Path.Combine(AppContext.BaseDirectory, "qr-converted.png");
            File.WriteAllBytes(outPath, bytes);
        }

        [Fact]
        void ConvertTest_InvalidDataThrows()
        {
            var agent = new QRCodeServiceAgent { MethodName = "Convert" };
            agent.Parameters.Add("Data", new byte[] { 0x00, 0x01, 0x02, 0x03 });
            agent.Parameters.Add("Width", 100);
            agent.Parameters.Add("Height", 100);

            Assert.ThrowsAny<Exception>(() => agent.Run());
        }
    }
}
