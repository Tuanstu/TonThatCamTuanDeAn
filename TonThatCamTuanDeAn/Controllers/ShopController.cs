using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using TonThatCamTuanDeAn.Models.Entity;
using TonThatCamTuanDeAn.Models.Product;

namespace TonThatCamTuanDeAn.Controllers
{
    public class ShopController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly TonThatCamTuanDeAnContext _context;
        public ShopController(TonThatCamTuanDeAnContext context, IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("Client");
            _context = context;
        }
        //public IActionResult Index(string timkiem)
        //{
        //    var items = _context.Products.Where(c => c.Filter.Contains((timkiem ?? "").ToLower())).ToList();
        //    return View(items);
        //}

        public async Task<IActionResult> Index(string timkiem)
        {
            var items = await getProduct(timkiem);
            return View(items);
        }
        [HttpPost]
        //[Route("/admin/product/Index")]

        public async Task<List<ViewModelProduct>> getProduct(string timkiem)
        {
            string url = "http://localhost:5098/api/Product/danh-sach-san-pham?timkiem=" + timkiem;


            /* Cach trc khi authorize
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(url); //call Api neu xai post thi xai PostAsync tuong tu voi may cai nhu Delete, Put, Post,...; phai co chu await moi hien IsSuccessStatusCode
                if (res.IsSuccessStatusCode) //ISSucessStatusCode la khi no ra 200 giong luc chay Api
                {
                    var viewproduct = new List<ViewModelProduct>();
                    var listitems = res.Content.ReadAsAsync<List<ResultApiProduct>>().Result; //ReadAsync la dc thong tin
                    foreach (var item in listitems)
                    {
                        ViewModelProduct product = new ViewModelProduct();
                        product.Id = item.Id;
                        product.ProductName = item.ProductName;
                        product.Detail = item.Detail;
                        product.Material = item.Material;
                        product.Size = item.Size;
                        product.Price = item.Price;
                        product.Images = JsonSerializer.Deserialize<List<ResultApiImage>>(item.UrlImages); //tai luc them no serialize la bien thanh mot cai string nen phai deserialize voi ep kieu no thanh mot cai list ma moi cai object trong list la mo cai class ResultApiImage
                        viewproduct.Add(product);

                    }
                    return viewproduct;
                }
                return null;
            
            }
            */

            //Sau khi authorize
            //Cach sau khi co authorize thay doi cho client a tai moi lan goi toi ham PostAsync, GetAsync, PutAsync,... thi se goi toi ham SendAsync o Authhandled de nhet cai token vo
            var res = await _httpClient.GetAsync(url); //call Api neu xai post thi xai PostAsync tuong tu voi may cai nhu Delete, Put, Post,...; phai co chu await moi hien IsSuccessStatusCode
            if (res.IsSuccessStatusCode) //ISSucessStatusCode la khi no ra 200 giong luc chay Api
            {
                var viewproduct = new List<ViewModelProduct>();
                var listitems = res.Content.ReadAsAsync<List<ResultApiProduct>>().Result; //ReadAsync la dc thong tin
                foreach (var item in listitems)
                {
                    ViewModelProduct product = new ViewModelProduct();
                    product.Id = item.Id;
                    product.ProductName = item.ProductName;
                    product.Detail = item.Detail;
                    product.Material = item.Material;
                    product.Size = item.Size;
                    product.Price = item.Price;
                    product.Images = JsonSerializer.Deserialize<List<ResultApiImage>>(item.UrlImages); //tai luc them no serialize la bien thanh mot cai string nen phai deserialize voi ep kieu no thanh mot cai list ma moi cai object trong list la mo cai class ResultApiImage
                    viewproduct.Add(product);

                }
                return viewproduct;
            }
            return null;
        }

        public async Task<IActionResult> ChiTietSanPhamAsync(string id)
        {
            var items = await GetProductById(id);
            return View(items);
        }
        public async Task<ViewModelProduct> GetProductById(string id)
        {
            string url = "http://localhost:5098/api/Product/data-product/" + id;
            var res = await _httpClient.GetAsync(url); //call Api neu xai post thi xai PostAsync tuong tu voi may cai nhu Delete, Put, Post,...; phai co chu await moi hien IsSuccessStatusCode

            if (res.IsSuccessStatusCode) //ISSucessStatusCode la khi no ra 200 giong luc chay Api
            {

                var item = res.Content.ReadAsAsync<ResultApiProduct>().Result; //ReadAsync la dc thong tin

                ViewModelProduct product = new ViewModelProduct();
                product.Id = item.Id;
                product.ProductName = item.ProductName;
                product.Detail = item.Detail;
                product.Material = item.Material;
                product.Size = item.Size;
                product.Price = item.Price;
                product.Images = JsonSerializer.Deserialize<List<ResultApiImage>>(item.UrlImages); //tai luc them no serialize la bien thanh mot cai string nen phai deserialize voi ep kieu no thanh mot cai list ma moi cai object trong list la mo cai class ResultApiImage

                return product;
            }
            return null;
        }
    }
}
