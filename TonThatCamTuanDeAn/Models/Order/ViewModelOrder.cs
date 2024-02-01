using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TonThatCamTuanDeAn.Models.Product;

namespace TonThatCamTuanDeAn.Models.Order
{
    public class ViewModelOrder
    {
        public string Id { get; set; }
        public string? ProductName { get; set; }

        [Column(TypeName = "money")]
        public long? Price { get; set; }
        public List<ResultApiImage> Images { get; set; }

        public string UserName { get; set; }
        public string DiaChi { get; set; }
        public string Sdt { get; set; }
    }
}
