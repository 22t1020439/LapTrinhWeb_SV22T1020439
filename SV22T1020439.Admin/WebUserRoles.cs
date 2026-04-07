namespace SV22T1020439.Admin
{
    /// <summary>
    /// Định nghĩa tên các quyền trong hệ thống
    /// </summary>
    public static class WebUserRoles
    {
        /// <summary>
        /// Quản trị hệ thống
        /// </summary>
        public const string ADMIN = "admin";
        /// <summary>
        /// Quản lý mặt hàng
        /// </summary>
        public const string PRODUCT = "product";
        /// <summary>
        /// Quản lý khách hàng
        /// </summary>
        public const string CUSTOMER = "customer";
        /// <summary>
        /// Lập đơn hàng
        /// </summary>
        public const string ORDER = "order";
    }
}
