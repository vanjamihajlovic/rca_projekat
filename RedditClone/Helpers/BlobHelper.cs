using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Helpers
{
    public class BlobHelper
    {
        // read account configuration settings
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));

        // create blob container for images
        CloudBlobClient blobStorage;

        public BlobHelper()
        {
            blobStorage = storageAccount.CreateCloudBlobClient();
        }

        public Image DownloadImage(String containerName, String blobName)
        {
            CloudBlobContainer container = blobStorage.GetContainerReference(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            using (MemoryStream ms = new MemoryStream())
            {
                blob.DownloadToStream(ms);
                return new Bitmap(ms);
            }
        }

        public string UploadImage(Image image, string containerName, string blobName)
        {
            try
            {
                CloudBlobContainer container = blobStorage.GetContainerReference(containerName);
                container.CreateIfNotExists(); // Optionally create the container if it doesn't exist

                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Save the image to the memory stream
                    image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                    // Reset the position to the beginning of the stream before uploading
                    memoryStream.Position = 0;

                    // Set the content type
                    blob.Properties.ContentType = "image/png";

                    // Upload the image from the memory stream
                    blob.UploadFromStream(memoryStream);

                    // Return the URI of the uploaded blob
                    return blob.Uri.ToString();
                }
            }
            catch (StorageException ex)
            {
                Trace.WriteLine($"StorageException: {ex.Message}");
                throw; // Optional: Rethrow the exception to propagate it further
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Exception: {ex.Message}");
                throw; // Optional: Rethrow the exception to propagate it further
            }
        }
    }
}
