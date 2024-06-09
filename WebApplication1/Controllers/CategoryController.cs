using DbService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;

namespace ShoppingCart.Controllers
{

    public class CategoryController : Controller
    {

        private readonly ILogger<CategoryController> _logger;
        private readonly ShoppingCartContext _shoppingCartContext;

        public CategoryController(ILogger<CategoryController> logger, ShoppingCartContext shoppingCartContext)
        {
            _logger = logger;
            _shoppingCartContext = shoppingCartContext;
        }
        // GET: CategoryController
        public ActionResult Index()
        {
            var categories = _shoppingCartContext.Category.ToList();
            CategoryCrudViewModel model = new CategoryCrudViewModel();
            model.Categories = categories;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(IFormCollection form)
        {
            string msg = "";
            string typeAlert = "";
            try
            {
                bool isDelete = form.ContainsKey("Delete");
                if (isDelete)
                {
                    var category = _shoppingCartContext.Category.Include(i => i.Products).FirstOrDefault(x => x.Name == form["Name"].ToString());
                    if (category != null)
                    {
                        if (category.Products == null || !category.Products.Any())
                        {
                            _shoppingCartContext.Category.Remove(category);
                            msg = "Se elimino la categoría con exito";
                            typeAlert = "ok";
                        }
                        else
                        {
                            msg = "La categoría que desea eliminar tiene productos asociados";
                            typeAlert = "error";
                        }
                    }
                    else
                    {
                        msg = "La categoría que desea eliminar no existe";
                        typeAlert = "error";
                    }
                }
                else
                {
                    bool isEdit = form.ContainsKey("Edit");
                    if (isEdit)
                    {
                        var category = _shoppingCartContext.Category.FirstOrDefault(x => x.Name == form["Name"].ToString());
                        if (category != null)
                        {
                            category.Description = form["Description"].ToString();
                            msg = "Se edito la categoría con exito";
                            typeAlert = "ok";
                        }
                        else
                        {
                            msg = "La categoría que desea editar no existe";
                            typeAlert = "error";
                        }
                    }
                    else
                    {
                        var category = _shoppingCartContext.Category.FirstOrDefault(x => x.Name == form["Name"].ToString());
                        if (category == null)
                        {
                            _shoppingCartContext.Category.Add(new Category
                            {
                                Name = form["Name"].ToString(),
                                Description = form["Description"].ToString()
                            });
                            msg = "Se agrego la categoría con exito";
                            typeAlert = "ok";
                        }
                        else
                        {
                            msg = "La categoría que desea agregar ya existe";
                            typeAlert = "error";
                        }
                    }
                }
                _shoppingCartContext.SaveChanges();
                var categories = _shoppingCartContext.Category.ToList();
                CategoryCrudViewModel model = new CategoryCrudViewModel();
                model.Categories = categories;
                model.Alert = msg;
                model.TypeAlert = typeAlert;
                return View("Index", model);
            }
            catch (Exception e)
            {
                var categories = _shoppingCartContext.Category.ToList();
                CategoryCrudViewModel model = new CategoryCrudViewModel();
                model.Categories = categories;
                model.Alert = msg;
                model.TypeAlert = typeAlert;
                return View("Index", model);
            }
        }

    }
}
