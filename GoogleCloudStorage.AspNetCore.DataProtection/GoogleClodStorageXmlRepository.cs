using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace GoogleCloudStorage.AspNetCore.DataProtection
{
    /// <summary>
    /// Google Cloud Storage implementation for storing and retrieving XML elements.
    /// </summary>
    public class GoogleClodStorageXmlRepository : IXmlRepository
    {
        private readonly Bucket _bucket;
        private readonly StorageClient _client;
        private readonly string _bucketName;

        public GoogleClodStorageXmlRepository(GoogleCredential credential, string projectId, string bucketName)
        {
            _bucketName = bucketName;
            _client = StorageClient.Create(credential);
            _bucket = _client.GetBucket(_bucketName) ?? _client.CreateBucket(_bucketName, projectId);
        }

        /// <summary>Gets all top-level XML elements in the repository.</summary>
        /// <remarks>All top-level elements in the repository.</remarks>
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            ListObjectsOptions options = new ListObjectsOptions()
            {
                PageSize = Int32.MaxValue
            };

            List<XElement> elements = new List<XElement>();

            foreach (Object @object in _client.ListObjects(_bucketName, null, options))
            {
                using (var stream = new MemoryStream())
                {
                    _client.DownloadObject(@object, stream);
                    stream.Position = 0;

                    XElement element = XElement.Load(stream);
                    elements.Add(element);
                }
            }

            return elements.AsReadOnly();
        }

        /// <summary>Adds a top-level XML element to the repository.</summary>
        /// <param name="element">The element to add.</param>
        /// <param name="friendlyName">An optional name to be associated with the XML element.
        /// For instance, if this repository stores XML files on disk, the friendly name may
        /// be used as part of the file name. Repository implementations are not required to
        /// observe this parameter even if it has been provided by the caller.</param>
        /// <remarks>
        /// The 'friendlyName' parameter must be unique if specified. For instance, it could
        /// be the id of the key being stored.
        /// </remarks>
        public void StoreElement(XElement element, string friendlyName)
        {
            string content = element.ToString(SaveOptions.DisableFormatting);

            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(content)))
            {
                _client.UploadObject(_bucketName, Guid.NewGuid().ToString(), "text/plain", stream);
            }
        }
    }
}