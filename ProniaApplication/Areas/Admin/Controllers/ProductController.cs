using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaApplication.Areas.ViewModels;
using ProniaApplication.DAL;

namespace ProniaApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        public AppDBContext _context { get; }
        public IWebHostEnvironment _env { get; }
        public ProductController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }



        public async Task<IActionResult> Index()
        {
            List<GetProductAdminVM> productsVMs = await _context.Products
                .Include(p => p.category)
                .Include(p => p.productsImages.Where(pi => pi.IsPrimary == true))
                .Select(p => new GetProductAdminVM
                {
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.category.Name,
                    Image = p.productsImages[0].ImageURL,
                    Id = p.Id
                }
                )
                .ToListAsync();



            return View(productsVMs);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync()
            };
            return View(productVM);
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            bool result = productVM.Categories.Any(c=>c.Id == productVM.CategoryId);

            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM), "Category does not exist");
                return View(productVM);
            }
            //CreateProductVM productVM = new CreateProductVM
            //{
            //    Categories = await _context.Categories.ToListAsync()
            //};
            //return View(productVM);

            return RedirectToAction(nameof(Index));
        }
    }
}
