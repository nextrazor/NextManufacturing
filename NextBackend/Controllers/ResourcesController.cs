using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NextBackend.DAL;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly NmaContext _dbContext;
        private readonly IStringLocalizer<ResourcesController> _localizer;

        public ResourcesController(NmaContext dbContext, IStringLocalizer<ResourcesController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        [HttpGet]
        public IEnumerable<Resource> Read()
        {
            return _dbContext.Resources.ToList();
        }

        [HttpPost]
        [Route("CreateResource/{name}")]
        public async Task<Resource> Create(string name)
        {
            name = name.Trim();
            if (name == string.Empty)
                throw new ArgumentException(_localizer["Empty name"], nameof(name));
            if (_dbContext.Resources.Any(r => r.Name == name))
                throw new ArgumentException(_localizer["Duplicate name"], nameof(name));
            var resource = new Resource()
            {
                Guid = Guid.NewGuid(),
                Name = name
            };
            _dbContext.Resources.Add(resource);
            await _dbContext.SaveChangesAsync();
            return resource;
        }

        [HttpDelete]
        [Route("DeleteResource/{guid:guid}")]
        public async Task<bool> Delete(Guid guid)
        {
            var resource = _dbContext.Resources.FirstOrDefault(r => r.Guid == guid);
            if (resource == null)
                return false;
            _dbContext.Resources.Remove(resource);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}