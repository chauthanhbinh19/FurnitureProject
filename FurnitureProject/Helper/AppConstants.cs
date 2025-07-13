namespace FurnitureProject.Helper
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

            public const string CreateTagSuccess = "Tag created successfully!";
            public const string CreateTagError = "Create tag failed!";
            public const string UpdateTagSuccess = "Tag updated successfully!";
            public const string UpdateTagError = "Update tag failed!";
            public const string DeleteTagSuccess = "Tag deleted successfully!";
            public const string DeleteTagError = "Delete tag failed!";

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

            public const string CreateVoucherSuccess = "Voucher created successfully!";
            public const string CreateVoucherError = "Create voucher failed!";
            public const string UpdateVoucherSuccess = "Voucher updated successfully!";
            public const string UpdateVoucherError = "Update voucher failed!";
            public const string DeleteVoucherSuccess = "Voucher deleted successfully!";
            public const string DeleteVoucherError = "Delete voucher failed!";

            public const string UsernameExists = "Username already exists.";
            public const string EmailExists = "Email already exists.";
            public const string PhoneNumberExists = "Phone number already exists.";

            public const string InvalidCode = "Code is invalid";

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
    }
}
