namespace Catalog.API.Controllers
{
    using System.Net;
    using Catalog.API.Infrastructure;
    using Catalog.API.Model;
    using Catalog.API.ViewModel;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext catalogContext;

        public CatalogController(CatalogContext context)
        {
            this.catalogContext = context;
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{pageSize:int}/{pageIndex:int}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<Product>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Products(int pageSize = 10, int pageIndex = 0)
        {
            var totalItem = await catalogContext.Product.LongCountAsync().ConfigureAwait(false);

            var itemsOnPage = await catalogContext.Product.
                OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            var model = new PaginatedItemsViewModel<Product>(pageIndex, pageSize, totalItem, itemsOnPage);
            return Ok(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> AddBrand()
        {
            var item = new Brand
            {
                BrandName = "Samsung",
                Description = "Samsung android phone"
            };

            catalogContext.Brand.Add(item);
            await catalogContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> AddProduct()
        {
            var item = new Product
            {
                Name = "Samsung A72",
                Description = "Samsung A72 android phone",
                Price = 20000,
                BrandId = 1
            };

            await catalogContext.AddAsync(item);
            await catalogContext.SaveChangesAsync();

            catalogContext.Product.Add(item);
            await catalogContext.SaveChangesAsync();
            return Ok();
        }
     
    }
}