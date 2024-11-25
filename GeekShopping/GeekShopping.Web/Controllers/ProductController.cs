using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var products = await _service.FindAllProducts(token!);
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var response = await _service.CreateProduct(model, token!);
                if (response != null)
                    return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(long id)
        {
            if (id <= 0) return NotFound();
            var token = await HttpContext.GetTokenAsync("access_token");
            var product = await _service.FindProductById(id, token!);
            if (product == null || product.Id <= 0) return NotFound();
            return View(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var response = await _service.UpdateProduct(model, token!);
                if (response != null)
                    return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0) return NotFound();
            var token = await HttpContext.GetTokenAsync("access_token");
            var product = await _service.FindProductById(id, token!);
            if (product == null || product.Id <= 0) return NotFound();
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(ProductModel model)
        {
            if (model.Id > 0)
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var status = await _service.DeleteProductById(model.Id, token!);
                if (status)
                    return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), new { id = model.Id});
        }
    }
}
