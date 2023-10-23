using Microsoft.AspNetCore.Mvc;
using TonThatCamTuanDeAn.Models;

namespace TonThatCamTuanDeAn.Controllers
{
    public class ShopController : Controller
    {
        private readonly TonThatCamTuanDeAnContext _context;
        public ShopController(TonThatCamTuanDeAnContext context)
        {
            _context = context;
        }
        public IActionResult Index(string timkiem)
        {
            var items = _context.Products.Where(c => c.Filter.Contains((timkiem ?? "").ToLower())).ToList();
            return View(items);
        }
        public IActionResult ChiTietSanPham(string id)
        {
            var items = _context.Products.FirstOrDefault(c => c.Id == id);
            return View(items);
        }
    }
}
