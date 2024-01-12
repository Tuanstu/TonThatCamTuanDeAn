using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QuanLySinhVien.Areas.Admin.Views.TaiKhoan;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TonThatCamTuanDeAn.Areas.Admin.Models.TaiKhoan;

namespace TonThatCamTuanDeAn.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("tai-khoan")]

    public class TaiKhoanController : Controller
    {
        private readonly HttpClient _httpClient;

        public TaiKhoanController(IHttpClientFactory httpClient) //De moi lan goi Api gan cai header nay vo cho no le ma muon su dung thang nay phai configure no trong program.cs
        {
            _httpClient = httpClient.CreateClient("Client");
        }

        [Route("dang-nhap")]
        public IActionResult DangNhap()
        {
            return View();
        }
        [Route("dang-nhap")]
        [HttpPost]
        public async Task<IActionResult> DangNhap(InputDangNhap input)
        {
            string url = "http://localhost:5098/api/Authentication/auth";
            if (ModelState.IsValid)
            {
                var data = new MultipartFormDataContent();
                data.Add(new StringContent(input.Email), "Email");
                //data.Add(new StringContent(input.Username), "Username");
                data.Add(new StringContent(EncryptPassword(input.Password)), "Password");
                //data.Add(new StringContent(input.Role), "Role");
                var res = await _httpClient.PostAsync(url, data);
                if (res.IsSuccessStatusCode)
                {
                    var token = await res.Content.ReadAsAsync<OutputToken>(); //doc du lieu
                                                                              //string test = token.Token;
                    /* Response.Cookies.Append("Username", input.Username); *///khai bao mot cai cookie de chua cai username
                                                                              //Response.Cookies.Append("JwtToken", token.Token);
                    return await AccessLogin(token.Token);
                }
            }
            return View();
        }

        private async Task<IActionResult> AccessLogin(string Token) //dang nhap bang token ko phai bang username,...
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(Token) as JwtSecurityToken; //lay cai thang Token cua API roi bien lai thanh dang giong decode cuar JWT

            var identity = new ClaimsIdentity(token.Claims, "Token"); // thay noi coi "Token" ko quan trong noi chung no lay het nhung thong tin nhu payload roi nhet vo cai identity

            var principal = new ClaimsPrincipal(identity); //principal de no su dung phuong thuc SignInAsync

            var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value; //lay ra cai role
            var username = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;


            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires =token.ValidTo; 
            Response.Cookies.Append("Expire", DateTime.Now.ToString(), cookieOptions);
            Response.Cookies.Append("Token", Token, cookieOptions); //luu cookie muon tao session time kieu nhu het h thi bat login lai thi coi trong slide a
            Response.Cookies.Append("Username", username, cookieOptions);
            AuthenticationProperties properties = new AuthenticationProperties();
            properties.ExpiresUtc = token.ValidTo;
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal); // dang nhap vao roi ghi lai token theo kieu mac dinh cua he thong
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            return await CheckRole(role);
            
        }

        private async Task<IActionResult> CheckRole(string? role) // khi dang nhap xong lay cai role roi chuyen toi cai trang tuy vao role  
        {
            //var identity = HttpContext.User.Identity as ClaimsIdentity; //ham nay phai SignInAsync roi moi xai duoc ma tai co chu await o truoc await HttpContext.SignInAsync nen ko dc
            //var role = identity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == "admin") return RedirectToAction("Index", "Home", new { Area = "Admin" });
            if (role == "user") return RedirectToAction("Index", "Home", new { Area = "User" });
            //if (role == "nhansu") return RedirectToAction("Index", "Home", new { Areas = "NhanSu" }); //Vi du co nhieu role thi cu them vo
            return Redirect("/");
        }


        [Route("dang-ky")]
        public IActionResult DangKy()
        {
            return View();
        }
        [Route("dang-ky")]
        [HttpPost]
        public async Task<IActionResult> DangKy(InputTaiKhoan input)
        {
            string url = "http://localhost:5098/api/TaiKhoan/dang-ky";
            if (ModelState.IsValid)
            {
                var data = new MultipartFormDataContent();
                data.Add(new StringContent(input.Email), "Email");
                data.Add(new StringContent(input.Username), "Username");
                data.Add(new StringContent(EncryptPassword(input.Password)), "Password");
                data.Add(new StringContent(input.Role), "Role");

                var res = await _httpClient.PostAsync(url, data);
                //var result = await res.Content.ReadAsAsync<OutputDangKy>(); //buoc nay thay noi la kieu ai muon dang ky roi vao luon chu ko phai dang ky roi dang nhap
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("DangNhap", "TaiKhoan", new { Areas = "Admin" });
                }
            }
            return View();
        }

        private string EncryptPassword(string password) //Ma hoa password
        {
            using (var sha256 = SHA256.Create()) //thuat toan ma hoa
            {
                var data = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(data); //convert 2 lan cho no lu
                return Convert.ToBase64String(hash);
            }
        }

        [Route("access-denied")]
        public IActionResult TuChoi()
        {
            return View();
        }

        [Route("logout")]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            Response.Cookies.Delete("Username"); //Delete cai cookie dc luu
            Response.Cookies.Delete("Token"); //delete cai cookie dc luu
            return RedirectToAction("DangNhap", "TaiKhoan", new { Area = "Admin" });
        }
    }
}
