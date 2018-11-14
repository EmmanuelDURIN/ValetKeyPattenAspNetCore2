using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using PictureGalleryBlobStorageNetCore.BlobStorage;
using PictureGalleryBlobStorageNetCore.Models;
using PictureGalleryBlobStorageNetCore.ViewModels;

namespace PictureGalleryBlobStorageNetCore.Controllers
{
  public class PictureController : Controller
  {
    public IActionResult Index()
    {
      return View(model: new List<Picture>());
    }
    // Action pour voir la page du formulaire
    public IActionResult Upload()
    {
      PictureUploadViewModel p = new PictureUploadViewModel
      {
        Date = DateTime.Now,
      };
      return View(model: p);
    }
    // Action d'envoi d'image
    [ValidateAntiForgeryToken]
    [HttpPost]
    public IActionResult Upload(PictureUploadViewModel pictureViewModel)
    {
      if (!ModelState.IsValid)
        return View();
      // TODO : utiliser BlobStorageManager pour sauvegarder la liste des images
      return RedirectToAction(nameof(Index));
    }
  }
}