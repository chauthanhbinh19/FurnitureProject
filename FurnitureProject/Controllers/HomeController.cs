using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FurnitureProject.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly ICartService _cartService;
        private readonly IPromotionService _promotionService;
        private readonly IFavouriteService _favouriteService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICategoryService categoryService,
            ITagService tagService, ICartService cartService, IPromotionService promotionService, IFavouriteService favouriteService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _tagService = tagService;
            _cartService = cartService;
            _promotionService = promotionService;
            _favouriteService = favouriteService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            int pageSize = 10;
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

            var productDtos = products.Select(product =>
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
            //ViewBag.CurrentPage = page;
            //ViewBag.PageSize = pageSize;
            //ViewBag.Search = filter.SearchKeyWord;
            //ViewBag.TotalProducts = totalProducts;
            return View(productViewModel);
        }
        [HttpGet("/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
