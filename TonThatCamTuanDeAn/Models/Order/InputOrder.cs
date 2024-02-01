﻿using System.ComponentModel.DataAnnotations;

namespace TonThatCamTuanDeAn.Models.Order
{
    public class InputOrder
    {
        public string? OrderId { get; set; }

        [StringLength(450)]
        public string? ProductId { get; set; }
        public string? ProductQuantity { get; set; }
        [StringLength(150)]
        public string UserName { get; set; }
        [StringLength(10)]
        public string Sodienthoai { get; set; }
        [StringLength(150)]
        public string Diachi { get; set; }
        [StringLength(100)]
        public string? Ngaytao { get; set; }


    }
}
