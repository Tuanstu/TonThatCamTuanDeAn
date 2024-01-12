
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using TonThatCamTuanDeAn.Models.Entity;
using TonThatCamTuanDeAn.Common;
using TonThatCamTuanDeAn.Models.Product;
using Utility = Common.Utilities.Utility;
using System.Text.Json;
using System.Configuration;
using System;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;

namespace TonThatCamTuanDeAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/product")]
    [Authorize(Roles = "admin")] //role co phan biet viet hoa nha
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly TonThatCamTuanDeAnContext _context;
        public ProductController(TonThatCamTuanDeAnContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _httpClient = clientFactory.CreateClient("Client");
        }
        private static string wwwroot = Directory.GetCurrentDirectory() + "\\wwwroot";
        /*Cach kho de tim kiem:
        public IActionResult Index(string timkiem)
         {
             //Cach kho:
             var items = _context.Products.Where(c => c.Filter.Contains((timkiem ?? "").ToLower())).ToList();
             return View(items);
             //cai nay nghia la neu timkiem bi null thi tra ve "" ngc lai tra ve chinh no

             //Cach de:
             //if (String.IsNullOrWhiteSpace(timkiem))
             //{
             //    var items1 = _context.Products.ToList();
             //    return View(items1);
             //}
             //else
             //{
             //    var items2 = _context.Products.Where(c => c.Filter.Contains(timkiem)).ToList();
             //    return View(items2);
             //}
         }*/
        [Route("/admin/product/Index")]
        public async Task<IActionResult> Index(string timkiem)
        {
            var items = await getProduct(timkiem);
            return View(items);
        }
        [HttpPost]
        [Route("/admin/product/Index")]

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


        [Route("/admin/product/them")]
        public IActionResult Them()
        {
            //ViewData["Thembithieu"] = "";
            return View();
        }
        [Route("/admin/product/them")]
        [HttpPost]

        public async Task<IActionResult> Them(InputProduct input) //co await thi phai co async voi them chu Task
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            string url = "http://localhost:5098/api/Product/tao-product";
            /* Cach trc khi co authorize
            using (var client = new HttpClient())
            {
                var data = new MultipartFormDataContent(); //vo Api ngay cai cho dien form cua thang them co MultipartFormData
                data.Add(new StringContent(input.ProductName), "ProductName"); // cai "ProductName" la cai ten dien trong cai form cua Api
                //data.Add(new StringContent (input.Price, "Price");
                data.Add(new StringContent(input.Detail), "Detail");
                data.Add(new StringContent(input.Material), "Material");
                data.Add(new StringContent(input.Size), "Size");

                var listimg = new List<string>();
                foreach (var img in input.Images)
                {
                    var imgPath = UploadFiles.SaveImage(img); //de luu tam hinh vo MVC
                    listimg.Add(imgPath); //add vo cai list tai tra ve nhieu cai string de lat hoi xoa
                    var imgStream = new MemoryStream(System.IO.File.ReadAllBytes(wwwroot + imgPath)); //doc tam hinh
                    var imgContent = new ByteArrayContent(imgStream.ToArray()); //doc xong roi chuyen no ve dang byte phai dang nay moi duoc
                    data.Add(imgContent, "Images", img.FileName); //thay no giong cai o tren chua
                }
                var res = await client.PostAsync(url, data); //phai co await moi ra cai res.IsSuccessStatusCode
                if (res.IsSuccessStatusCode)
                {
                    foreach (var img in listimg)
                    {
                        UploadFiles.RemoveImage(img); //day du lieu xong thi phai xoa tam hinh tai ko muon ben MVC luu tam hinh

                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    //co the them thong bao loi
                    return View();
                }

            }
            */

            //Cach sau khi co authorize thay doi cho client a tai moi lan goi toi ham PostAsync, GetAsync, PutAsync,... thi se goi toi ham SendAsync o Authhandled de nhet cai token vo
            var data = new MultipartFormDataContent(); //vo Api ngay cai cho dien form cua thang them co MultipartFormData
            data.Add(new StringContent(input.ProductName), "ProductName"); // cai "ProductName" la cai ten dien trong cai form cua Api
            data.Add(new StringContent(input.Price), "Price");
            data.Add(new StringContent(input.Detail), "Detail");
            data.Add(new StringContent(input.Material), "Material");
            data.Add(new StringContent(input.Size), "Size");

            var listimg = new List<string>();
            foreach (var img in input.Images)
            {
                var imgPath = UploadFiles.SaveImage(img); //de luu tam hinh vo MVC
                listimg.Add(imgPath); //add vo cai list tai tra ve nhieu cai string de lat hoi xoa
                var imgStream = new MemoryStream(System.IO.File.ReadAllBytes(wwwroot + imgPath)); //doc tam hinh
                var imgContent = new ByteArrayContent(imgStream.ToArray()); //doc xong roi chuyen no ve dang byte phai dang nay moi duoc
                data.Add(imgContent, "Images", img.FileName); //thay no giong cai o tren chua
            }
            var res = await _httpClient.PostAsync(url, data); //phai co await moi ra cai res.IsSuccessStatusCode
            if (res.IsSuccessStatusCode)
            {
                foreach (var img in listimg)
                {
                    UploadFiles.RemoveImage(img); //day du lieu xong thi phai xoa tam hinh tai ko muon ben MVC luu tam hinh

                }
                return RedirectToAction("Index");
            }
            else
            {
                //co the them thong bao loi
                return View();


            }
        }
        /* //cai nay goi la Class Binding: truyen vao mot cai model
        
        public IActionResult Them(InputProduct input)
        {
            
            if (ModelState.IsValid)
            {

            Product product = new Product();
                product.Id = Guid.NewGuid().ToString();
                product.ProductName = input.ProductName;
                product.Price = input.Price;
                product.Detail = input.Detail;
                product.Image = UploadFiles.SaveImage(input.Images);
                product.Material = input.Material;
                product.Size = input.Size;
                product.Filter = input.Detail + " " + input.ProductName.ToLower() + " " + Utility.ConvertToUnsign(input.ProductName.ToLower()) + " " + input.Price;

                _context.Add(product);
                _context.SaveChanges();
                ViewData["Thembithieu"] = "";
                return RedirectToAction("Index");
                //return Redirect("/Khoa/Index");
            }
            //ViewData["Thembithieu"] = "Xin Đừng Để Trống Tên Sản Phẩm";
            return View();
        }

        */

        /* Simple Binding: khi truyen tham so
         
        public IActionResult Them(string productname, int price, IFormFile image, string detail, string material, string size)
        {
            
            //tạo Guid
            //Guid id = Guid.NewGuid();
            if (!string.IsNullOrEmpty(productname))
            {
                Product product = new Product();
                product.Id = Guid.NewGuid().ToString();
                product.ProductName = productname;
                product.Price = price;
                product.Detail = detail;
                product.Image = UploadFiles.SaveImage(image);
                product.Material = material;
                product.Size = size;
                product.Filter = detail + " " + productname.ToLower() + " " + Utility.ConvertToUnsign(productname.ToLower()) + " " + price;

                _context.Add(product);
                _context.SaveChanges();
                ViewData["Thembithieu"] = "";
                return RedirectToAction("Index");
                //return Redirect("/Khoa/Index");
            }
            ViewData["Thembithieu"] = "Xin Đừng Để Trống Tên Sản Phẩm";
            return View(ViewData["Thembithieu"]);
        }
        */
        [Route("/admin/product/capnhat")]
        public async Task<IActionResult> CapNhat(string id)
        {
            string url = "http://localhost:5098/api/Product/data-product/" + id;
            /* Cach trc khi co authorization
            using (var client = new HttpClient())
            {

                var res = await client.GetAsync(url); //call Api neu xai post thi xai PostAsync tuong tu voi may cai nhu Delete, Put, Post,...; phai co chu await moi hien IsSuccessStatusCode
                
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
                    
                    return View(product);
                }
                return null;
            }
            */

            //Sau khi co authorization
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

                return View(product);
            }
            return null;
        }
        [Route("/admin/product/capnhat")]
        [HttpPost]
        public async Task<IActionResult> CapNhat(string id, EditProduct edit) //co await thi phai co async voi them chu Task
        {
            if(!ModelState.IsValid)
            {
                var item = await GetProductById(id);
                return View(item);
            }

            string url = "http://localhost:5098/api/Product/cap-nhat-product/" + id;
            /* Cach trc khi co authorization
            using (var client = new HttpClient())
            {
                var data = new MultipartFormDataContent(); //vo Api ngay cai cho dien form cua thang them co MultipartFormData
                data.Add(new StringContent(input.ProductName), "ProductName"); // cai "ProductName" la cai ten dien trong cai form cua Api
                //data.Add(new StringContent (input.Price.ToString(), "Price");
                data.Add(new StringContent(input.Detail), "Detail");
                data.Add(new StringContent(input.Material), "Material");
                data.Add(new StringContent(input.Size), "Size");

                var listimg = new List<string>();
                foreach (var img in input.Images)
                {
                    var imgPath = UploadFiles.SaveImage(img); //de luu tam hinh vo MVC
                    listimg.Add(imgPath); //add vo cai list tai tra ve nhieu cai string de lat hoi xoa
                    var imgStream = new MemoryStream(System.IO.File.ReadAllBytes(wwwroot + imgPath)); //doc tam hinh
                    var imgContent = new ByteArrayContent(imgStream.ToArray()); //doc xong roi chuyen no ve dang byte phai dang nay moi duoc
                    data.Add(imgContent, "Images", img.FileName); //thay no giong cai o tren chua
                }
                var res = await client.PutAsync(url, data); //phai co await moi ra cai res.IsSuccessStatusCode
                if (res.IsSuccessStatusCode)
                {
                    foreach (var img in listimg)
                    {
                        UploadFiles.RemoveImage(img); //day du lieu xong thi phai xoa tam hinh tai ko muon ben MVC luu tam hinh

                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    //co the them thong bao loi
                    return View();
                }
            }
            */
            var data = new MultipartFormDataContent(); //vo Api ngay cai cho dien form cua thang them co MultipartFormData
            data.Add(new StringContent(edit.ProductName), "ProductName"); // cai "ProductName" la cai ten dien trong cai form cua Api
            data.Add(new StringContent(edit.Price), "Price");
            data.Add(new StringContent(edit.Detail), "Detail");
            data.Add(new StringContent(edit.Material), "Material");
            data.Add(new StringContent(edit.Size), "Size");
            if (edit.Images != null)
            {
                var listimg = new List<string>();
                foreach (var img in edit.Images)
                {
                    var imgPath = UploadFiles.SaveImage(img); //de luu tam hinh vo MVC
                    listimg.Add(imgPath); //add vo cai list tai tra ve nhieu cai string de lat hoi xoa
                    var imgStream = new MemoryStream(System.IO.File.ReadAllBytes(wwwroot + imgPath)); //doc tam hinh
                    var imgContent = new ByteArrayContent(imgStream.ToArray()); //doc xong roi chuyen no ve dang byte phai dang nay moi duoc
                    data.Add(imgContent, "Images", img.FileName); //thay no giong cai o tren chua
                }
                foreach (var img in listimg)
                {
                    UploadFiles.RemoveImage(img); //day du lieu xong thi phai xoa tam hinh tai ko muon ben MVC luu tam hinh

                }
            }
            
            var res = await _httpClient.PutAsync(url, data); //phai co await moi ra cai res.IsSuccessStatusCode
            if (res.IsSuccessStatusCode)
            {
                //foreach (var img in listimg)
                //{
                //    UploadFiles.RemoveImage(img); //day du lieu xong thi phai xoa tam hinh tai ko muon ben MVC luu tam hinh

                //}
                return RedirectToAction("Index");
            }
            else
            {
                //co the them thong bao loi
                return View();
            }

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
        //public async Task<IActionResult> CapNhat(string id)
        //{
        //    var items = await getProduct();
        //    return View(items);
        //}



        /*      [Route("/admin/product/capnhat")]
        [HttpPost]
         public IActionResult CapNhat(Guid id, InputProduct input) //neu upload nhieu hinh thi xai IFormFile collection
        {
            var item = _context.Products.FirstOrDefault(c => c.Id == id.ToString());
            item.Id = id.ToString();
            item.ProductName = input.ProductName;
            item.Price = input.Price;
            if (input.Images != null)
            {
                UploadFiles.RemoveImage(item.Image);
                item.Image = UploadFiles.SaveImage(input.Images);
            }
            else
            {
                item.Image = item.Image;
            }
            item.Detail = input.Detail;
            item.Material = input.Material;
            item.Size = input.Size;
            item.Filter = input.Detail + " " + input.ProductName.ToLower() + " " + Utility.ConvertToUnsign(input.ProductName.ToLower()) + " " + input.Price;
            _context.Update(item);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
        */


        /*
         Simple Binding:  truyen tham so
        public IActionResult CapNhat(string id, string productname, int price, IFormFile image, string detail, string material, string size, string filter) //neu upload nhieu hinh thi xai IFormFile collection
        {
            var item = _context.Products.FirstOrDefault(c => c.Id.Contains(id));
            item.Id = id;
            item.ProductName = productname;
            item.Price = price;
            if (image != null)
            {
                UploadFiles.RemoveImage(item.Image);
                item.Image = UploadFiles.SaveImage(image);
            }
            else
            {
                item.Image = item.Image;
            }
            item.Detail = detail;
            item.Material = material;
            item.Size = size;
            item.Filter = filter;
            _context.Update(item);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
        */

        [Route("admin/product/xoa")]
        public async Task<IActionResult> Xoa(string id)
        {
            string url = "http://localhost:5098/api/Product/xoa-product/" + id;
            /*Cach trc khi co authorization
            using (var client = new HttpClient())
            {
                var res = await client.DeleteAsync(url); //call Api neu xai post thi xai PostAsync tuong tu voi may cai nhu Delete, Put, Post,...; phai co chu await moi hien IsSuccessStatusCode

            }
            */

            //Sau khi co authorization
            var res = await _httpClient.DeleteAsync(url); //call Api neu xai post thi xai PostAsync tuong tu voi may cai nhu Delete, Put, Post,...; phai co chu await moi hien IsSuccessStatusCode

            return RedirectToAction("Index");
        }

        /* Xoa theo dang ket noi truc tiep voi server
        [Route("admin/prodcut/xoa")]
        public IActionResult Xoa(string id)
        {
            var item = _context.Products.FirstOrDefault(x => x.Id == id);
            UploadFiles.RemoveImage(item.Image);
            _context.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        } 
        */
    }
}
