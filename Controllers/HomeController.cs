using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvc_products.Helpers;
using mvc_products.Models;
using mvc_products.Services;
using System.Diagnostics;
using System.Linq;

namespace mvc_products.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProductService _productService { get; }

        public HomeController(ILogger<HomeController> logger, ProductService productService)
        {
            _productService = productService;
            _logger = logger;

        }

        public IActionResult Index(string filterBrand)
        {
            List<Product> products = _productService.GetProducts().ToList();

            if (HttpContext.Session.GetInt32("cartCount") == null)
            {
                HttpContext.Session.SetInt32("cartCount", 0);
                HttpContext.Session.SetObjectAsJson("cartItems", new List<Cart>());
            }

            string selectedBrand = string.IsNullOrEmpty(filterBrand) ? "ALL" : filterBrand;
            List<SelectListItem> selectList = new List<SelectListItem>
            {
                new SelectListItem { Text = "ALL", Value = "ALL" }
            };

            selectList.AddRange(products.DistinctBy(b => b.Brand).Select(x =>
            new SelectListItem
            {
                Value = x.Brand,
                Text = x.Brand,
                Selected = x.Brand == selectedBrand
            }).OrderByDescending(x => x.Text).ToList());



            ViewBag.Brands = selectList;
            ViewBag.SelectedBrand = selectedBrand;



            if (!String.IsNullOrEmpty(filterBrand) && filterBrand != "ALL")
            {
                products = products.Where(products => products.Brand == filterBrand).ToList();
            }


            ViewBag.productCount = products.Count;


            return View(products);
        }


        public ActionResult Cart()
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<Cart>>("cartItems");

            return View();
        }

        public JsonResult AddToCart(int itemId)
        {

            var product = _productService.GetProduct(itemId);

            var currentCartItems = HttpContext.Session.GetObjectFromJson<List<Cart>>("cartItems");

            Cart cartItem = currentCartItems.FirstOrDefault(c => c.ItemId == product.Id);


            if (cartItem != null)
            {
                currentCartItems.FirstOrDefault(x => x.ItemId == product.Id).Quantity += 1;
            }
            else
            {
                var rnd = new Random(DateTime.Now.Millisecond);
                int randomId = rnd.Next();

                currentCartItems.Add(new Cart()
                {
                    Id = randomId,
                    ItemName = product.Title,
                    ItemId = product.Id,
                    Quantity = 1,
                    UnitPrice = product.Price,
                    Thumbnail = product.Thumbnail,
                });
            }

            HttpContext.Session.SetObjectAsJson("cartItems", currentCartItems);
            HttpContext.Session.SetInt32("cartCount", currentCartItems.Count);
            return Json(new { status = 200, total = currentCartItems.Count });
        }




        public IActionResult ProductDetail(int id)
        {
            Product product = _productService.GetProduct(id);

            return View(product);
        }


        public ActionResult ChangeCartQuantity(int itemCartId,  int quantity)
        {
            var currentCartItems = HttpContext.Session.GetObjectFromJson<List<Cart>>("cartItems");

            Cart cartItem = currentCartItems.FirstOrDefault(c => c.Id == itemCartId);


            if (cartItem != null)
            {
                currentCartItems.FirstOrDefault(x => x.Id == itemCartId).Quantity = quantity;
            }

            HttpContext.Session.SetObjectAsJson("cartItems", currentCartItems);

            return Json(new { status = 200 });
         
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CartItem()
        {

            var cartItems = HttpContext.Session.GetObjectFromJson<List<Cart>>("cartItems");

            return PartialView("partial/_CartItems", cartItems);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}