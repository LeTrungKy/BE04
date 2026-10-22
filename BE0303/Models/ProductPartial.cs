using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace BE0303.Models
{
    public partial class Product
    {

        [NotMapped]
        public HttpPostedFileBase UploadImg { get; set; }
    }
}