using DocumentFormat.OpenXml.Drawing.Diagrams;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace GestorDeClientes.Core.Services.MinIo
{
    public class MinioService : IMinio
    {
        private readonly IMinioClient _minio;

        public MinioService(IMinioClient minio)
        {
            _minio = minio;
        }

        public async Task UploadAsync(
            string bucket,
            string objectName,
            Stream fileStream,
            string contentType)
        {
            var existsArgs = new BucketExistsArgs()
                .WithBucket(bucket);

            bool exists = await _minio.BucketExistsAsync(existsArgs);

            if (!exists)
            {
                var makeArgs = new MakeBucketArgs()
                    .WithBucket(bucket);

                await _minio.MakeBucketAsync(makeArgs);
            }

            var putArgs = new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minio.PutObjectAsync(putArgs);
        }

        public async Task<Stream> ObterArquivoAsync(string bucket, string nomeArquivo)
        {
            var memoryStream = new MemoryStream();

            await _minio.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(nomeArquivo)
                    .WithCallbackStream(stream =>
                    {
                        stream.CopyTo(memoryStream);
                    })
            );

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<MemoryStream> DownloadAsync(string bucket, string objectName)
        {
            var ms = new MemoryStream();

            var getArgs = new GetObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(ms);
                });

            await _minio.GetObjectAsync(getArgs);

            ms.Position = 0;
            return ms;
        }

        public async Task UploadAsync1(
            string bucket,
            string objectName,
            Stream fileStream,
            string contentType)
        {
            var exists = await _minio.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucket)
            );

            if (!exists)
            {
                await _minio.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(bucket)
                );
            }

            await _minio.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(objectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType)
            );
        }


    }
}
