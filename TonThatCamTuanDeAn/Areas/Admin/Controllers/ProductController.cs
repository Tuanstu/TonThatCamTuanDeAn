
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using TonThatCamTuanDeAn.Common;
using TonThatCamTuanDeAn.Models;
using Utility = Common.Utilities.Utility;

namespace TonThatCamTuanDeAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/product")]
    public class ProductController : Controller
    {
        private readonly TonThatCamTuanDeAnContext _context;
        public ProductController(TonThatCamTuanDeAnContext context)
        {
            _context = context;
        }
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
        }
        [Route("/admin/product/them")]
        public IActionResult Them()
        {
            ViewData["Thembithieu"] = "";
            return View();
        }
        [Route("/admin/product/them")]
        [HttpPost]
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
        [Route("/admin/product/capnhat")]
        public IActionResult CapNhat()
        {
            return View();
        }
        [HttpPost]
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
            return Redirect("product");

        }
        [Route("admin/prodcut/xoa")]
        public IActionResult Xoa(string id)
        {
            var item = _context.Products.FirstOrDefault(x => x.Id == id);
            UploadFiles.RemoveImage(item.Image);
            _context.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
