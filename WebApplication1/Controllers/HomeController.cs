﻿using DbService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var products = _shoppingCartContext.Product.Include(p => p.Category).ToList();
            ProductsCrudViewModel model = new ProductsCrudViewModel();
            model.Products = products;
            model.Categories = _shoppingCartContext.Category.Select(c => new KeyValueViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
            model.Alert = "";
            return View("Views/ShopViews/ProductsCrud.cshtml", model);
        }

        public IActionResult MyPurchases()
        {
            MyPurchasesViewModel model = new MyPurchasesViewModel();
            var purchases = _shoppingCartContext.Purchase.Include(p => p.Details).ToList();
            model.Purchases= purchases;
            return View("Views/PurcharseViews/MyPurchases.cshtml", model);
        }

        public IActionResult ViewPurchase(long id)
        {
            var purchase = _shoppingCartContext.Purchase.Include(p => p.Details).First(p => p.Id == id);
            FinalCartViewModel model = new FinalCartViewModel();
            model.Id = purchase.Id;
            model.Date = purchase.Date.ToShortDateString();
            model.IsView = true;
            model.Observations = purchase.Observation;
            model.Products = purchase.Details.Select(d => new FinalCartGridViewModel
            {
                Name = d.ProductName,
                Quantity = d.Quantity,
                UnitPrice = d.Price,
                TotalPrice = d.Quantity * d.Price
            }).ToList();
            return View("Views/PurcharseViews/FinalCart.cshtml", model);
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
                var returnProducts = _shoppingCartContext.Product.Include(p => p.Category).ToList();
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

        [HttpPost]
        public ActionResult NewPurchase(FinalCartViewModel data)
        {
            _shoppingCartContext.Purchase.Add(new Purchase
            {
                Date = DateTime.Now,
                Observation =  !string.IsNullOrWhiteSpace(data.Observations) ? data.Observations : string.Empty,
                Details = data.Products.Select(p => new PurchaseDetail
                {
                    ProductName = p.Name,
                    Price = p.UnitPrice,
                    Quantity = p.Quantity
                }).ToList()
            });
            _shoppingCartContext.SaveChanges();
            return RedirectToAction("MyPurchases");
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
                            long? category = form["Categoria"] == "" ? null : Convert.ToInt64(form["Categoria"]);
                            product.CategoryId = category;
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
                            long? category = form["Categoria"] == "" ? null : Convert.ToInt64(form["Categoria"]);
                            _shoppingCartContext.Product.Add(new Product
                            {
                                Name = form["Name"].ToString(),
                                Description = form["Description"].ToString(),
                                Price = Convert.ToDecimal(form["Price"].ToString().Replace(".", ",")),
                                CategoryId = category
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
                var products = _shoppingCartContext.Product.Include(p => p.Category).ToList();
                ProductsCrudViewModel model = new ProductsCrudViewModel();
                model.Products = products;
                model.Categories = _shoppingCartContext.Category.Select(c => new KeyValueViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();
                model.Alert = msg;
                model.TypeAlert = typeAlert;
                return View("Views/ShopViews/ProductsCrud.cshtml", model);
            }
            catch(Exception e)
            {
                var products = _shoppingCartContext.Product.Include(p => p.Category).ToList();
                ProductsCrudViewModel model = new ProductsCrudViewModel();
                model.Products = products;
                model.Categories = _shoppingCartContext.Category.Select(c => new KeyValueViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();
                return View("Views/ShopViews/ProductsCrud.cshtml", model);
            }
        }

    }
}