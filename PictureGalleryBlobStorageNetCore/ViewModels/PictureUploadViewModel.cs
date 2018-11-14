using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PictureGalleryBlobStorageNetCore.ViewModels
{
  public class PictureUploadViewModel
  {
    [Required]
    public String Title { get; set; }
    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    public IFormFile Image { set; get; }
  }
}