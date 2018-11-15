using Microsoft.AspNetCore.Mvc;
using PictureGalleryBlobStorageNetCore.BlobStorage;
using PictureGalleryBlobStorageNetCore.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PictureGalleryBlobStorageNetCore.Controllers
{
  public class PictureController : Controller
  {
    public async Task<IActionResult> Index()
    {
      return View(model: await new BlobStorageManager().GetPicturesAsync());
    }
    // Action pour voir la page du formulaire
    public IActionResult Upload()
    {
      PictureUploadViewModel p = new PictureUploadViewModel
      {
        Date = DateTime.Now,
        StorageAccount = BlobStorageManager.StorageAccount,
        ContainerName = BlobStorageManager.ContainerName
      };
      return View(model: p);
    }
    // Action d'envoi d'image
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Upload(PictureUploadViewModel pictureViewModel)
    {
      if (!ModelState.IsValid)
        return View();
      string msg = null;
      // TODO : utiliser BlobStorageManager pour sauvegarder l'images
      try
      {
        await new BlobStorageManager().SavePictureAsync(pictureViewModel);
        msg = "Picture saved";
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        msg = "Error saving picture";
      }
      Debug.WriteLine(msg);
      return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> GetSasToken(string filename)
    {
      //string filename = this.Request["filename"];
      String token = await new BlobStorageManager().GetSASTokenForCreation(filename);
      return new JsonResult(token);
    }
  }
}