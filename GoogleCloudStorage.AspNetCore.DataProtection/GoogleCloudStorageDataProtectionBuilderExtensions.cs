using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;

namespace GoogleCloudStorage.AspNetCore.DataProtection
{
    public static class GoogleCloudStorageDataProtectionBuilderExtensions
    {
        /// <summary>
        /// Configures the data protection system to persist keys to specified key in Google Cloud Storage Bucket
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="serviceAccountKeyFilePath">The JSON key file path that holds credential data for a service account.</param>
        /// <param name="projectId">The Google Cloud Project Id.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToGoogleCloudStorage(this IDataProtectionBuilder builder, string serviceAccountKeyFilePath, string projectId)
        {
            GoogleCredential credential = GoogleCredential.FromFile(serviceAccountKeyFilePath);

            builder.Services.Configure<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new GoogleClodStorageXmlRepository(credential, projectId);
            });

            return builder;
        }
    }
}
