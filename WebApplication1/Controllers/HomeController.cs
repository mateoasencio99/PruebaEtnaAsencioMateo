using DbService;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
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
            var products = _shoppingCartContext.Product.ToList();
            ProductsCrudViewModel model = new ProductsCrudViewModel();
            model.Products = products;
            model.Alert = "";
            return View("Views/ShopViews/ProductsCrud.cshtml", model);
        }
        public IActionResult Purcharse()
        {
            PurcharseViewModel model = new PurcharseViewModel();
            var products = _shoppingCartContext.Product.ToList();
            model.Products = products;
            model.Alert = "";
            return View("Views/PurcharseViews/Purcharse.cshtml", model);
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
            if (products.Count() == 0)
            {
                PurcharseViewModel model = new PurcharseViewModel();
                var returnProducts = _shoppingCartContext.Product.ToList();
                model.Products = returnProducts;
                model.TypeAlert = "error";
                model.Alert = "Debe seleccionar al menos un producto para terminar el pedido";
                return View("Views/PurcharseViews/Purcharse.cshtml", model);
            }
            else
            {
                FinalCartViewModel model = new FinalCartViewModel();
                model.Products = products;
                return View("Views/PurcharseViews/FinalCart.cshtml", model);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public ActionResult Product(IFormCollection form)
        {
            string msg = "";
            string typeAlert = "";
            try
            {
                bool isDelete = form.ContainsKey("Delete");
                if (isDelete)
                {
                    var product = _shoppingCartContext.Product.FirstOrDefault(x => x.Name == form["Name"].ToString());
                    if (product != null)
                    {
                        _shoppingCartContext.Product.Remove(product);
                        msg = "Se elimino el producto con exito";
                        typeAlert = "ok";
                    }
                    else
                    {
                        msg = "El producto que desea eliminar no existe";
                        typeAlert = "error";
                    }
                }
                else
                { bool isEdit = form.ContainsKey("Edit");
                    if (isEdit)
                    {
                        var product = _shoppingCartContext.Product.FirstOrDefault(x => x.Name == form["Name"].ToString());
                        if (product != null)
                        {
                            product.Description = form["Description"].ToString();
                            product.Price = Convert.ToDecimal(form["Price"].ToString().Replace(".", ","));
                            msg = "Se edito el producto con exito";
                            typeAlert = "ok";
                        }
                        else
                        {
                            msg = "El prducto que desea editar no existe";
                            typeAlert = "error";
                        }
                    }
                    else
                    {
                        var product = _shoppingCartContext.Product.FirstOrDefault(x => x.Name == form["Name"].ToString());
                        if (product == null)
                        {
                            _shoppingCartContext.Product.Add(new Product
                            {
                                Name = form["Name"].ToString(),
                                Description = form["Description"].ToString(),
                                Price = Convert.ToDecimal(form["Price"].ToString().Replace(".", ","))
                            });
                            msg = "Se agrego el producto con exito";
                            typeAlert = "ok";
                        }
                        else
                        {
                            msg = "El prducto que desea agregar ya existe";
                            typeAlert = "error";
                        }
                    }
                }
                _shoppingCartContext.SaveChanges();
                var products = _shoppingCartContext.Product.ToList();
                ProductsCrudViewModel model = new ProductsCrudViewModel();
                model.Products = products;
                model.Alert = msg;
                model.TypeAlert = typeAlert;
                return View("Views/ShopViews/ProductsCrud.cshtml", model);
            }
            catch(Exception e)
            {
                var products = _shoppingCartContext.Product.ToList();
                return View("Views/ShopViews/ProductsCrud.cshtml", products);
            }
        }

    }
}