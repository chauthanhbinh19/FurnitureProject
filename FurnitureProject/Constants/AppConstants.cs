namespace FurnitureProject.Constants
{
    public static class AppConstants
    {
        public static class LogMessages
        {
            public const string CreateUserSuccess = "User created successfully!";
            public const string CreateUserError = "Create user failed!";
            public const string UpdateUserSuccess = "User updated successfully!";
            public const string UpdateUserError = "Update user failed!";
            public const string DeleteUserSuccess = "User deleted successfully!";
            public const string DeleteUserError = "Delete user failed!";

            public const string SignInUserError = "Sign in failed!";
            public const string SignUpUserError = "Sign up failed!";
            public const string UsernameAlreadyExists = "Username already exists";
            public const string UsernameIsNotExists = "Username is not exists";
            public const string EmailAlreadyExists = "Email already exists";
            public const string EmailDoesNotExists = "Email does not exists";
            public const string SignUpSuccess = "Sign up successfully";
            public const string WrongPassword = "Your password is not correct";
            public const string PasswordSameAsOld = "Your new password can not match with old password";
            public const string ChangePasswordSuccessful = "Change password successfully!";

            public const string CreateCategorySuccess = "Category created successfully!";
            public const string CreateCategoryError = "Create category failed!";
            public const string UpdateCategorySuccess = "Category updated successfully!";
            public const string UpdateCategoryError = "Update category failed!";
            public const string DeleteCategorySuccess = "Category deleted successfully!";
            public const string DeleteCategoryError = "Delete category failed!";
            public const string CategoryNameCannotBeEmpty = "Category name cannot be empty";

            public const string CreateTagSuccess = "Tag created successfully!";
            public const string CreateTagError = "Create tag failed!";
            public const string UpdateTagSuccess = "Tag updated successfully!";
            public const string UpdateTagError = "Update tag failed!";
            public const string DeleteTagSuccess = "Tag deleted successfully!";
            public const string DeleteTagError = "Delete tag failed!";
            public const string TagNameCannotBeEmpty = "Tag name cannot be empty";

            public const string CreateProductSuccess = "Product created successfully!";
            public const string CreateProductError = "Create product failed!";
            public const string UpdateProductSuccess = "Product updated successfully!";
            public const string UpdateProductError = "Update product failed!";
            public const string DeleteProductSuccess = "Product deleted successfully!";
            public const string DeleteProductError = "Delete product failed!";

            public const string CreatePromotionSuccess = "Promotion created successfully!";
            public const string CreatePromotionError = "Create promotion failed!";
            public const string UpdatePromotionSuccess = "Promotion updated successfully!";
            public const string UpdatePromotionError = "Update promotion failed!";
            public const string DeletePromotionSuccess = "Promotion deleted successfully!";
            public const string DeletePromotionError = "Delete promotion failed!";
            public const string PromotionTitleCannotBeEmpty = "Promotion title cannot be empty";
            public const string PromotionDescriptionCannotBeEmpty = "Promotion description cannot be empty";
            public const string PromotionDiscountPercentCannotBeEmpty = "Promotion discount percent cannot be empty";
            public const string PromotionStartDateCannotBeEmpty = "Promotion start date cannot be empty";
            public const string PromotionStartDateCannotBeInPast = "Promotion start date cannot be earlier than today.";
            public const string PromotionEndDateCannotBeEmpty = "Promotion end date cannot be empty";
            public const string PromotionEndDateCannotBeBeforeStart = "Promotion end date cannot be earlier than the start date.";

            public const string CreateVoucherSuccess = "Voucher created successfully!";
            public const string CreateVoucherError = "Create voucher failed!";
            public const string UpdateVoucherSuccess = "Voucher updated successfully!";
            public const string UpdateVoucherError = "Update voucher failed!";
            public const string DeleteVoucherSuccess = "Voucher deleted successfully!";
            public const string DeleteVoucherError = "Delete voucher failed!";
            public const string VoucherCodeCannotBeEmpty = "Voucher code cannot be empty";
            //public const string VoucherDiscountPerCannotBeEmpty = "Promotion description cannot be empty";
            //public const string PromotionDiscountPercentCannotBeEmpty = "Promotion discount percent cannot be empty";
            public const string VoucherExpiryDateCannotBeEmpty = "Voucher expiry date cannot be empty";
            public const string VoucherExpiryDateCannotBeInPast = "Voucher expiry date cannot be earlier than today.";
            public const string VoucherUsageLimitCannotBeEmpty = "Voucher usage limit cannot be empty";
            public const string VoucherTimeUsedCannotBeEmpty = "Voucher time used cannot be empty";
            //public const string PromotionEndDateCannotBeBeforeStart = "Promotion end date cannot be earlier than the start date.";

            public const string UsernameExists = "Username already exists.";
            public const string EmailExists = "Email already exists.";
            public const string PhoneNumberExists = "Phone number already exists.";

            public const string InvalidCode = "Code is invalid";
            public const string MustSignIn = "You must sign in first";
            
            public const string CartItemAdded = "Product added to cart successfully!";
            public const string CartItemRemoved = "Product removed from cart successfully!";
            public const string CartItemRemoveFailed = "Failed to remove the item from your cart.";
            public const string CartItemUpdated = "Product quantity updated successfully!";
            public const string CartItemUpdatedFailed = "Failed to update the item from your cart.";
            public const string CartItemNotFound = "Product not found in cart.";
            public const string CartItemNotEnoughStock = "Not enough stock for this product.";

            public const string OrderPaymentSuccessfully = "Order payment was successful.";
            public const string OrderPaymentFailed = "Order payment failed. Please try again.";
            public const string CreateOrderSuccess = "Order created successfully!";
            public const string CreateOrderError = "Create order failed!";
            public const string UpdateOrderSuccess = "Order updated successfully!";
            public const string UpdateOrderError = "Update order failed!";
            public const string DeleteOrderSuccess = "Order deleted successfully!";
            public const string DeleteOrderError = "Delete order failed!";

            public const string CreateAddressSuccess = "Address created successfully!";
            public const string CreateAddressError = "Create address failed!";

            public const string Admin = "Admin";
            public const string User = "User";
            public const string Active = "Active";
            public const string Inactive = "Inactive";
            public const string Newest = "Newest";
            public const string Oldest = "Oldest";
            public const string PriceAscending = "Price-asc";
            public const string PriceDescending = "Price-desc";
        }

        public static class ExceptionMessages
        {
            public const string UserIsNull = "User not found.";
            public const string ProductIsNull = "Product not found.";
            public const string CategoryIsNull = "Category not found.";
            public const string ImageIsNull = "Image not found.";
        }

        public static class Status
        {
            public const string Admin = "admin";
            public const string User = "user";
            public const string Success = "Success";
            public const string Error = "Error";
            public const string Active = "active";
            public const string Inactive = "inactive";
            public const string Newest = "newest";
            public const string Oldest = "oldest";
            public const string PriceAscending = "price-asc";
            public const string PriceDescending = "price-desc";
            public const string Male = "male";
            public const string Female = "female";
            public const string Other = "other";

            public const string Pending = "pending";
            public const string Confirmed = "confirmed"; 
            public const string Processing = "processing";
            public const string Shipping = "shipping";   
            public const string Completed = "completed"; 
            public const string Cancelled = "cancelled";

            public const string COD = "cod";
            public const string BankTransfer = "banktransfer";
            public const string CreditCard = "creditcard";
        }

        public static class Params
        {
            public const string UserIdPrefix = "USR";
        }

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }

        public static class ConfigKeys
        {
            public const string SomeApiKey = "MyApp:SomeApiKey";
        }

        public static class  Display
        {
            public const string CategoryName = "Tên danh mục";
            public const string CategoryDescription = "Mô tả danh mục";
            public const string CategoryStatus = "Trạng thái danh mục";

            public const string ProductName = "Tên sản phẩm";
            public const string ProductDescription = "Mô tả sản phẩm";
            public const string ProductStock = "Số lượng tồn kho";
            public const string ProductPrice = "Giá sản phẩm";
            public const string ProductCategory = "Danh mục sản phẩm";
            public const string ProductTag = "Nhãn sản phẩm";
            public const string ProductStatus = "Trạng thái sản phẩm";

            public const string PromotionName = "Tên khuyến mãi";
            public const string PromotionDescription = "Mô tả khuyến mãi";
            public const string PromotionDiscountPercent = "Phần trăm giảm giá";
            public const string PromotionStartDate = "Ngày bắt đầu";
            public const string PromotionEndDate = "Ngày kết thúc";
            public const string PromotionStatus = "Trạng thái khuyến mãi";

            public const string TagName = "Tên nhãn";
            public const string TagStatus = "Trạng thái nhãn";

            public const string UserUsername = "Tên đăng nhập";
            public const string UserEmail = "Email";
            public const string UserPassword = "Mật khẩu";
            public const string UserConfirmPassword = "Nhập lại mật khẩu";
            public const string UserFullname = "Họ tên đầy đủ";
            public const string UserPhoneNumber = "Số điện thoại";
            public const string UserDateOfBirth = "Ngày sinh";
            public const string UserGender = "Giới tính";
            public const string UserEmailConfirmed = "Xác nhận email";
            public const string UserPhoneNumberConfirmed = "Xác nhận số điện thoại";
            public const string UserAvatarUrl = "Ảnh đại diện";
            public const string UserRole = "Vai trò";
            public const string UserStatus = "Trạng thái người dùng";
            public const string PasswordTooShort = "Mật khẩu phải có ít nhất 8 ký tự.";
            public const string PasswordMissingUpper = "Mật khẩu phải chứa ít nhất 1 chữ hoa.";
            public const string PasswordMissingLower = "Mật khẩu phải chứa ít nhất 1 chữ thường.";
            public const string PasswordMissingNumber = "Mật khẩu phải chứa ít nhất 1 chữ số.";
            public const string PasswordMissingSpecial = "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt (@, $, !, %, *, ?, &).";
            public const string InvalidEmailFormat = "Email không đúng định dạng";

            public const string VoucherCode = "Mã voucher";
            public const string VoucherDiscountPercent = "Phần trăm giảm (%)";
            public const string VoucherDiscountAmount = "Số tiền giảm";
            public const string VoucherExpiryDate = "Ngày hết hạn";
            public const string VoucherUsageLimit = "Số lượt sử dụng tối đa";
            public const string VoucherTimeUsed = "Số lần đã sử dụng";
            public const string VoucherIsValid = "Còn hiệu lực";
            public const string VoucherStatus = "Trạng thái mã giảm giá";

            public const string OrderReceiverName = "Họ tên người nhận";
            public const string OrderReceiverEmail = "Email người nhận";
            public const string OrderReceiverPhone = "Số điện thoại người nhận";
            public const string OrderShippingAddress = "Địa chỉ giao hàng";
            public const string OrderPaymentMethod = "Phương thức thanh toán";
            public const string OrderShippingFee = "Phí giao hàng";
            public const string OrderOrderDate = "Ngày đặt hàng";
            public const string OrderStatus = "Trạng thái đơn hàng";
            public const string OrderTotalAmount = "Tổng tiền";

            public const string AddressStreet = "Địa chỉ";
            public const string AddressWard = "Phường/Xã";
            public const string AddressDistrict = "Quận/Huyện";
            public const string AddressCity = "Thành phố/Tỉnh";
            public const string AddressCountry = "Quốc gia";
            public const string AddressPostalCode = "Mã bưu điện";

            public const string COD = "Thanh toán khi nhận hàng";
            public const string BankTransfer = "Chuyển khoản ngân hàng";
            public const string CreditCard = "Thẻ tín dụng";

            public const string Status = "Trạng thái";
            public const string Male = "Nam";
            public const string Female = "Nữ";
            public const string Other = "Khác";
            public const string Admin = "Quản lý";
            public const string User = "Người dùng";
            public const string Active = "Hoạt động";
            public const string Inactive = "Không hoạt động";
            public const string Pending = "Chờ xác nhận";
            public const string Confirmed = "Đã xác nhận";
            public const string Processing = "Đang xử lý";
            public const string Shipping = "Đang giao hàng";
            public const string Completed = "Hoàn thành";
            public const string Cancelled = "Đã huỷ";
        }
    }
}
