using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using TonThatCamTuanDeAn.Common;
using TonThatCamTuanDeAn.Models.Entity;
using TonThatCamTuanDeAn.Models.Order;
using TonThatCamTuanDeAn.Models.Product;

namespace TonThatCamTuanDeAn.Controllers
{
    public class GioHangController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly TonThatCamTuanDeAnContext _context;
        public GioHangController(TonThatCamTuanDeAnContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _httpClient = clientFactory.CreateClient("Client");
        }
        public IActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(InputOrder input)
        {
            if (ModelState.IsValid)
            {

                var productids = System.Text.Json.JsonSerializer.Deserialize<List<string>>(input.ProductId);
                var orderquantity = System.Text.Json.JsonSerializer.Deserialize<List<string>>(input.ProductQuantity);
                string orderid = Guid.NewGuid().ToString();

                string url = "http://localhost:5098/api/Order/themvaogiohang";

                for (int i = 0; i < productids.Count(); i++)
                {
                    string id = Guid.NewGuid().ToString();
                    var data = new MultipartFormDataContent();
                    data.Add(new StringContent(id), "Id");
                    data.Add(new StringContent(orderid), "OrderId");
                    data.Add(new StringContent(productids[i]), "ProductId");
                    data.Add(new StringContent(orderquantity[i]), "ProductQuantity");
                    data.Add(new StringContent(input.UserName), "UserName");
                    data.Add(new StringContent(input.Sodienthoai), "Sodienthoai");
                    data.Add(new StringContent(input.Diachi), "DiaChi");
                    data.Add(new StringContent(input.Ngaytao), "Ngaytao");
                    var res = await _httpClient.PostAsync(url, data);
                }

                return RedirectToAction("Index", "ThankYou");
            }
            return View();

        }


    }
}
