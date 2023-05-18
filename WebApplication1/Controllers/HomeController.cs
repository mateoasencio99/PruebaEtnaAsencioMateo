using DbService;
using Jint.Native.Json;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShoppingCartContext _shoppingCartContext;

        public HomeController(ILogger<HomeController> logger, ShoppingCartContext shoppingCartContext)
        {
            _logger = logger;
            _shoppingCartContext = shoppingCartContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductsCrud()
        {
            var products = _shoppingCartContext.Products.ToList();
            return View("Views/ShopViews/ProductsCrud.cshtml", products);
        }
        public IActionResult Purcharse()
        {
            var products = _shoppingCartContext.Products.ToList();
            return View("Views/PurcharseViews/Purcharse.cshtml", products);
        }

        public IActionResult Cart(IList<string> Name,IList<string> Quantity, IList<string> TotalProductPrice)
        {
            List<FinalCartGridViewModel> products = new List<FinalCartGridViewModel>();
            for(int i = 0; i < Name.Count(); i++)
            {
                if (Quantity[i] != "0") {
                    products.Add(new FinalCartGridViewModel
                    {
                        Name = Name[i],
                        Quantity = Convert.ToInt32(Quantity[i]),
                        UnitPrice = Convert.ToDecimal(TotalProductPrice[i].Replace(".", ",")) / Convert.ToInt32(Quantity[i]),
                        TotalPrice = Convert.ToDecimal(TotalProductPrice[i].Replace(".", ","))
                    });
                }
            }
                
            return View("Views/PurcharseViews/FinalCart.cshtml", products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public ActionResult Product(IFormCollection form)
        {
            try
            {
                bool isDelete = form.ContainsKey("Delete");
                if (isDelete)
                {
                    var product = _shoppingCartContext.Products.FirstOrDefault(x => x.Name == form["Name"].ToString());
                    if(product != null)
                        _shoppingCartContext.Products.Remove(product);
                }
                else
                { bool isEdit = form.ContainsKey("Edit");
                    if (isEdit)
                    {
                        var product = _shoppingCartContext.Products.FirstOrDefault(x => x.Name == form["Name"].ToString());
                        if (product != null)
                        {
                            product.Description = form["Description"].ToString();
                            product.Price = Convert.ToDecimal(form["Price"].ToString().Replace(".", ","));
                        }
                    }
                    else
                    {
                        _shoppingCartContext.Products.Add(new Product
                        {
                            Name = form["Name"].ToString(),
                            Description = form["Description"].ToString(),
                            Price = Convert.ToDecimal(form["Price"].ToString().Replace(".",","))
                        });
                    }
                }
                _shoppingCartContext.SaveChanges();
                var products = _shoppingCartContext.Products.ToList();
                return View("Views/ShopViews/ProductsCrud.cshtml", products);
            }
            catch(Exception e)
            {
                var products = _shoppingCartContext.Products.ToList();
                return View("Views/ShopViews/ProductsCrud.cshtml", products);
            }
        }

    }
}