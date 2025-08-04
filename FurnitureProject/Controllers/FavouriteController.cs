using CloudinaryDotNet;
using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("favourite")]
    public class FavouriteController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        private readonly IPromotionService _promotionService;
        private readonly IFavouriteService _favouriteService;
        public FavouriteController(IFavouriteService favouriteService, ICartService cartService, IProductService productService,
            ICategoryService categoryService, IPromotionService promotionService)
        {
            _favouriteService = favouriteService;
            _cartService = cartService;
            _productService = productService;
            _categoryService = categoryService;
            _promotionService = promotionService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index(string search = "", int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            int pageSize = 50;
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var promotions = await _promotionService.GetAllAsync();
            var today = DateTime.UtcNow;
            var userId = HttpContext.Session.GetString("UserID");
            List<Favourite> favourites = new();

            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userIds))
            {
                favourites = await _favouriteService.GetFavouritesByUserAsync(Guid.Parse(userId));
            }

            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var productDtos = products
                .Select(product =>
                {
                    var activePromotion = promotions.FirstOrDefault(promo =>
                        promo.ProductPromotions.Any(pp => pp.ProductId == product.Id) &&
                        promo.EndDate >= today
                    );

                    bool isFavourited = favourites
                        .Any(f => f.userId == Guid.Parse(userId) && f.productId == product.Id);

                    decimal discountPrice = 0;
                    if (activePromotion != null)
                    {
                        var discount = activePromotion.DiscountPercent;
                        discountPrice = product.Price * (1 - discount / 100m);
                    }
                    return new ProductDTO
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Stock = product.Stock,
                        Status = product.Status,
                        Category = categories.FirstOrDefault(c => c.Id == product.CategoryId),
                        CreatedAt = product.CreatedAt,
                        ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                        TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new(),
                        DiscountPrice = discountPrice,
                        IsFavourited = isFavourited,
                    };
                }).ToList();

            productDtos = productDtos
                .Where(p => p.IsFavourited == true)
                .ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(search))
            {
                productDtos = productDtos
                    .Where(u =>
                        (!string.IsNullOrEmpty(u.Name) && u.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.Description) && u.Description.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            int totalProducts = productDtos.Count();
            var pagedProducts = productDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productViewModel = new ProductViewModel
            {
                Products = pagedProducts,
                //Filter = filter
            };

            //ViewBag.Status = "active";
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.TotalProducts = totalProducts;
            return View(productViewModel);
        }
        [HttpPost("toggle-favourite")]
        public async Task<IActionResult> ToggleFavourite(Guid productId)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng đăng nhập để sử dụng chức năng yêu thích.",
                    redirectUrl = Url.Action("Signin", "User") // --> /user/signin
                });
            }

            try
            {
                bool isFavourited = await _favouriteService.IsFavouritedAsync(Guid.Parse(userId), productId);
                if (isFavourited)
                {
                    await _favouriteService.RemoveFavouriteAsync(Guid.Parse(userId), productId);
                    return Json(new { success = true, status = "removed" });
                }
                else
                {
                    await _favouriteService.AddFavouriteAsync(Guid.Parse(userId), productId);
                    return Json(new { success = true, status = "added" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, status = "added" });
            }
        }
    }
}
