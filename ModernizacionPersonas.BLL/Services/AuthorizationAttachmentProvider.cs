using Microsoft.AspNetCore.Http;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class AuthorizationAttachmentProvider
    {
        private readonly string rootPath;
        private readonly CotizacionTransactionsProvider transactionsProvider;

        public AuthorizationAttachmentProvider(string rootPath)
        {
            this.rootPath = rootPath;
            this.transactionsProvider = new CotizacionTransactionsProvider();
        }

        public async Task<UploadAuthorizationAttachmentResponse> UploadAsync(int codigoCotizacion, int version, string userName, IFormFile file)
        {
            try
            {
                var fileName = file.FileName;
                var transactionId = await this.CreateTransaction(codigoCotizacion, version, userName);
                var directoryPath = Path.Combine(this.rootPath, $"attachments/{codigoCotizacion}/{version}/{transactionId}");
                var currentDirectory = Directory.GetCurrentDirectory();
                var filePath = Path.Combine(this.rootPath, directoryPath, fileName);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var s = file.OpenReadStream())
                using (var f = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await s.CopyToAsync(f);
                    await this.CreateTransactionAttachmentAsync(userName, transactionId, fileName);
                    return new UploadAuthorizationAttachmentResponse
                    {
                        TransactionId = transactionId,
                        FileName = fileName
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AuthorizationAttachmentProvider :: UploadAsync", ex);
            }
        }

        public async Task<byte[]> CreateCompressedFile(int codigoCotizacion, int version, object transactionId)
        {
            try
            {
                List<InMemoryFile> files = new List<InMemoryFile>();
                var directoryPath = Path.Combine(this.rootPath, $"attachments/{codigoCotizacion}/{version}/{transactionId}");
                var fileNames = Directory.GetFiles(directoryPath);
                foreach (var filePath in fileNames)
                {
                    var fileName = Path.GetFileName(filePath);
                    var fileStream = await this.GetFileStreamAsync(filePath);
                    files.Add(new InMemoryFile
                    {
                        FileName = fileName,
                        Content = this.ReadFully(fileStream)
                    });
                }

                var zipResult = this.GetZipArchive(files);

                // var savePath = Path.Combine(directoryPath, "Foo.zip");
                // File.WriteAllBytes(savePath, zipResult);

                return zipResult;
            }
            catch (Exception ex)
            {
                throw new Exception("AuthorizationAttachmentProvider :: CreateCompressedFile", ex);
            }
        }

        private byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private async Task<Stream> GetFileStreamAsync(string filePath)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return memory;
        }

        private byte[] GetZipArchive(List<InMemoryFile> files)
        {
            byte[] archiveFile;
            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var zipArchiveEntry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                        using (var zipStream = zipArchiveEntry.Open())
                            zipStream.Write(file.Content, 0, file.Content.Length);
                    }
                }

                archiveFile = archiveStream.ToArray();
            }

            return archiveFile;
        }

        private async Task CreateTransactionAttachmentAsync(string userName, int transactionId, string fileName)
        {
            await this.transactionsProvider.CreateTransactionAttachment(userName, transactionId, fileName);
        }

        private async Task<int> CreateTransaction(int codigoCotizacion, int version, string userName)
        {
            var args = new CotizacionTransactionArgs
            {
                CodigoCotizacion = codigoCotizacion,
                UserName = userName,
                AuthorizationsCount = 0,
                Comments = new List<TransactionComment>()
            };

            return await this.transactionsProvider.CreateAuthorizationTransaction(args);
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }

    public class UploadAuthorizationAttachmentResponse : ActionResponseBase
    {
        public int TransactionId { get; set; }
        public string FileName { get; set; }
    }

    public class DownloadAttachmentResponse : ActionResponseBase
    {
        public byte[] Data { get; set; }
    }

    public class InMemoryFile
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }
}
