using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TonThatCamTuanDeAn.Models.Product
{
    public class ResultApiProduct //lop hung ket qua tu Api
    {
        public string Id { get; set; }
        public string? ProductName { get; set; }

        [Column(TypeName = "money")]
        public long? Price { get; set; }
        public string UrlImages { get; set; }
        [StringLength(150)]
        public string? Detail { get; set; }

        public string? Material { get; set; }
        public string? Size { get; set; }
    }
    public class ResultApiImage //lop xu ly hinh anh
    {
        public string UrlImage { get; set; }
        public int Position { get; set; }
    }
}
