using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TonThatCamTuanDeAn.Models.Product
{
    public class ViewModelProduct
    {
        public string Id { get; set; }
        public string? ProductName { get; set; }

        [Column(TypeName = "money")]
        public long? Price { get; set; }
        public List<ResultApiImage> Images { get; set; }
        [StringLength(150)]
        public string? Detail { get; set; }

        public string? Material { get; set; }
        public string? Size { get; set; }
    }
}
