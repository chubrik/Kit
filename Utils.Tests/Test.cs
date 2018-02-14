using System;
using System.Threading;
using System.Threading.Tasks;
using Utils.Services;

namespace Utils.Tests {
    public class Test {

        private BlobService BlobService => BlobService.Instance;
        private ExceptionService ExceptionService => ExceptionService.Instance;
        private FileService FileService => FileService.Instance;

        public async Task Run(CancellationToken cancellationToken) {
            Console.WriteLine("Hello World!");

            using (var stream = FileService.OpenRead("../Test.cs"))
                await BlobService.UploadAsync("file.ext", stream, cancellationToken);

            using (var stream = FileService.OpenWrite("Test2.cs"))
                await BlobService.DownloadStreamAsync("file.ext", stream, cancellationToken);
        }
    }
}
