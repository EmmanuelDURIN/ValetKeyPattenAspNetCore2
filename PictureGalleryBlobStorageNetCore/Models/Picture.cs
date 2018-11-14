using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PictureGalleryBlobStorageNetCore.Models
{
  public class Picture
  {
    // Pour la sauvegarde Cosmos DB
    [JsonProperty(PropertyName = "id")]
    public String Id { get; set; }
    [Required]
    public String Title { get; set; }
    [Required]
    public String ImageUrl { get; set; }
    [Required]
    [DisplayFormat(DataFormatString = "{0:D}")]
    public DateTime Date { get; set; }
    public String BrowserFile { get; set; }
  }
}