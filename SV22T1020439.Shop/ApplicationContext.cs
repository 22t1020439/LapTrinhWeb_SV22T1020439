using Newtonsoft.Json;

namespace SV22T1020439.Shop
{
    public static class ApplicationContext
    {
        private static IHttpContextAccessor? _httpContextAccessor;
        private static IWebHostEnvironment? _webHostEnvironment;
        private static IConfiguration? _configuration;

        public static void Configure(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public static HttpContext? HttpContext => _httpContextAccessor?.HttpContext;
        public static string WWWRootPath => _webHostEnvironment?.WebRootPath ?? string.Empty;

        public static void SetSessionData(string key, object value)
        {
            try
            {
                string sValue = JsonConvert.SerializeObject(value);
                _httpContextAccessor?.HttpContext?.Session.SetString(key, sValue);
            }
            catch { }
        }

        public static T? GetSessionData<T>(string key) where T : class
        {
            try
            {
                string sValue = _httpContextAccessor?.HttpContext?.Session.GetString(key) ?? "";
                if (!string.IsNullOrEmpty(sValue))
                    return JsonConvert.DeserializeObject<T>(sValue);
            }
            catch { }
            return null;
        }
    }
}
