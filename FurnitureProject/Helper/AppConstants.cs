namespace FurnitureProject.Helper
{
    public static class AppConstants
    {
        public static class LogMessages
        {
            public const string CreateUserSuccess = "User created successfully!";
            public const string CreateUserError = "Create failed!";
            public const string UpdateUserSuccess = "User updated successfully!";
            public const string UpdateUserError = "Update failed!";
            public const string DeleteUserSuccess = "User deleted successfully!";
            public const string DeleteUserError = "Delete failed!";
            public const string SignInUserError = "Sign in failed!";
            public const string SignUpUserError = "Sign up failed!";
            public const string UsernameAlreadyExists = "Username already exists";
            public const string UsernameIsNotExists = "Username is not exists";
            public const string EmailAlreadyExists = "Email already exists";
            public const string SignUpSuccess = "Sign up successfully";
            public const string WrongPassword = "Your password is not correct";

            public const string CreateCategorySuccess = "Category created successfully!";
            public const string CreateCategoryError = "Create failed!";
            public const string UpdateCategorySuccess = "Category updated successfully!";
            public const string UpdateCategoryError = "Update failed!";
            public const string DeleteCategorySuccess = "Category deleted successfully!";
            public const string DeleteCategoryError = "Delete failed!";

            public const string CreateProductSuccess = "Product created successfully!";
            public const string CreateProductError = "Create failed!";
            public const string UpdateProductSuccess = "Product updated successfully!";
            public const string UpdateProductError = "Update failed!";
            public const string DeleteProductSuccess = "Product deleted successfully!";
            public const string DeleteProductError = "Delete failed!";

            public const string UsernameExists = "Username already exists.";
            public const string EmailExists = "Email already exists.";
            public const string PhoneNumberExists = "Phone number already exists.";

            public const string InvalidCode = "Code is invalid";
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
            public const string Success = "Success";
            public const string Error = "Error";
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
