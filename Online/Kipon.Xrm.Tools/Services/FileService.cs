using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Extensions.Generics;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.IFileService))]
    public class FileService : ServiceAPI.IFileService
    {
        private readonly Entities.IUnitOfWork uow;

        private const int BLOCK_SIZE = 4194304;

        [ImportingConstructor]
        public FileService(Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public void Upload(string filename, byte[] data, EntityReference targetId, string attribLogicalName)
        {
            var req = new InitializeFileBlocksUploadRequest
            {
                Target = targetId,
                FileName = filename,
                FileAttributeName = attribLogicalName.ToLower()
            };
            var response = uow.ExecuteRequest<InitializeFileBlocksUploadResponse>(req);


            var pages = data.Pages(BLOCK_SIZE);

            var blocks = new List<string>();
            foreach (var page in pages)
            {
                var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                var uReq = new UploadBlockRequest
                {
                    BlockId = blockId,
                    BlockData = page,
                    FileContinuationToken = response.FileContinuationToken
                };
                var uploadResponse = uow.ExecuteRequest<UploadBlockResponse>(uReq);
                blocks.Add(blockId);
            }

            var cReq = new CommitFileBlocksUploadRequest
            {
                FileContinuationToken = response.FileContinuationToken,
                FileName = filename,
                MimeType = System.Web.MimeMapping.GetMimeMapping(filename),
                BlockList = blocks.ToArray()
            };

            var commitResponse = uow.ExecuteRequest<CommitFileBlocksUploadResponse>(cReq);
        }
    }
}
