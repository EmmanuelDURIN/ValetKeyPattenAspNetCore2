using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PictureGalleryBlobStorageNetCore.Models;
using PictureGalleryBlobStorageNetCore.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PictureGalleryBlobStorageNetCore.BlobStorage
{
  public class BlobStorageManager
  {
    public static string StorageAccount => "valetkeypattern";
    public static string StorageAccountKey => "tmdiUndCfoZnGXhJq4FTUOZOILA1ZET1fXvwVhqtRxVgsJLv28t3yo+CDcmdSGnWFUV65DG04i5E0PApUBD/DQ==";
    private const string containerName = "imagesupload";

    public static string ContainerName => containerName;

    private static async Task<CloudBlobContainer> GetContainer()
    {
      // Make a connection string
      CloudStorageAccount storageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={StorageAccount};AccountKey={StorageAccountKey}");
      // Create the blob client object.
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
      // Get a reference to a container to use for the sample code, and create it if it does not exist.
      CloudBlobContainer container = blobClient.GetContainerReference(containerName);
#if DEBUG
      await container.CreateIfNotExistsAsync();
      await container.SetPermissionsAsync(new BlobContainerPermissions()
      {
        PublicAccess = BlobContainerPublicAccessType.Blob
      });
#endif
      return container;
    }
    public async Task<IEnumerable<Picture>> GetPicturesAsync()
    {
      List<Picture> pictures = new List<Picture>();
      // Retrouver la liste des images (Uri) dans le container
      CloudBlobContainer container = await GetContainer();
      BlobContinuationToken continuationToken = null;
      BlobResultSegment resultSegment = null;
      do
      {
        // Requête Http :
        resultSegment = await container.ListBlobsSegmentedAsync(currentToken: continuationToken);
        foreach (var pictureTask in resultSegment.Results.Select(async (bi) => new Picture
        {
          ImageUrl = bi.Uri.ToString(),
          // Requête Http pour chaque image, pas optimal, mais bon ok
          Date = await GetDateAsync(container, bi)
        }))
        {
          pictures.Add(await pictureTask);
        }
      } while (resultSegment.ContinuationToken != null);
      return pictures.ToList();
    }
    private async Task<DateTime> GetDateAsync(CloudBlobContainer container, IListBlobItem blobItem)
    {
      // Retrouver la date dans les métadonnées du blob
      string[] segments = blobItem?.Uri?.Segments;
      if (segments != null && segments.Length > 0)
      {
        String blobName = segments[segments.Length - 1];
        CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
        try
        {
          await blob.FetchAttributesAsync();
          string dateMetadata = blob.Metadata["Date"];
          DateTime date = DateTime.Parse(dateMetadata);
          return date;
        }
        catch (Exception)
        {
          System.Diagnostics.Debug.WriteLine("Error getting date attribute");
        }
      }
      return DateTime.Now;
    }
    public async Task SavePictureAsync(PictureUploadViewModel picture)
    {
      // Sauvegarder l'écriture en blob storage
      String filename = Path.GetFileName(picture.Image.FileName);
      CloudBlobContainer container = await GetContainer();
      CloudBlockBlob blob = container.GetBlockBlobReference(filename);
      // Ajout de métadonnées requêtables
      blob.Metadata.Add("Date", picture.Date.ToString());
      // Ajout d'un ContentType pour ajouter l'en-tête aux requêtes Http
      blob.Properties.ContentType = GetImageTypeFromExtension(filename);
      if (!await blob.ExistsAsync())
        // requête Http avec données + metadonnées
        await blob.UploadFromStreamAsync(picture.Image.OpenReadStream());
    }
    private string GetImageTypeFromExtension(string filename)
    {
      string ext = Path.GetExtension(filename);
      switch (ext)
      {
        case ".jpeg":
        case ".jpg":
          return "image/jpeg";
        case ".png":
          return "image/png";
        case ".tiff":
          return "image/tiff";
        case ".gif":
          return "image/gif";
      }
      return null;
    }
  }
}
