using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TonThatCamTuanDeAn.Models.Product
{
    public class EditProduct
    {
        [Required(ErrorMessage = "Xin hay nhap day du thong tin")]
        public string? ProductName { get; set; }
        [Required(ErrorMessage = "Xin hay nhap day du thong tin")]
        [Column(TypeName = "money")]
        public string? Price { get; set; }

        [StringLength(150)]
        public string? Detail { get; set; }

        public string? Material { get; set; }
        public string? Size { get; set; }
        //[Required(ErrorMessage = "Xin hay nhap day du thong tin")]
        public IFormFileCollection? Images { get; set; }
    }
}
