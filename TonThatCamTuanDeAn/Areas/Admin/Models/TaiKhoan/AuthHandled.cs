using System.Net.Http.Headers;

namespace TonThatCamTuanDeAn.Areas.Admin.Models.TaiKhoan
{
    public class AuthHandled : DelegatingHandler //phai ke thua moi co ham SendAsync
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthHandled(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        //khi su dung phuong thuc lq toi httpclient nhu PutAsync, GetAsync, PatchAsync, PostAsync,... no se deu gui thong tin vo ham SendAsync. Thang nay la thang di ra cuoi cung
        //Mac dinh la thang nay gui request cai gi la mac dinh gui cai do di nen phai ovveride de them cai token vo (bam "Ctrl + ." roi chon gennerate ovveride)
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) 
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var token = httpContext.Request.Cookies["Token"]; //Moi len Send la se lay ra dc chuoi token luu trong cookie
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); //no se tu dong gan Bearer voi token vo cai authorize ma nho trong API co khung Authorize ko. LUU Y: hinh nhu header co hai cai mot la header de nhet vo thang authorize, cai header kia chi header trong JWT
            }
            return await base.SendAsync(request, cancellationToken);
        }


    }
}
