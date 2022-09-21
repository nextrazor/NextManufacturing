using Microsoft.AspNetCore.Mvc;
using NextBackend.DAL;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourcesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Resource> Read()
        {
            using var dbContext = new NmaContext();
            return dbContext.Resources.ToList();
        }

        [HttpPost]
        [Route("CreateResource/{name}")]
        public async Task<Resource> Create(string name)
        {
            name = name.Trim();
            if (name == string.Empty)
                throw new ArgumentException("Empty name", nameof(name));
            using var dbContext = new NmaContext();
            if (dbContext.Resources.Any(r => r.Name == name))
                throw new ArgumentException("Duplicate name", nameof(name));
            var resource = new Resource()
            {
                Guid = Guid.NewGuid(),
                Name = name
            };
            dbContext.Resources.Add(resource);
            await dbContext.SaveChangesAsync();
            return resource;
        }

        [HttpDelete]
        [Route("DeleteResource/{guid:guid}")]
        public async Task<bool> Delete(Guid guid)
        {
            using var dbContext = new NmaContext();
            var resource = dbContext.Resources.FirstOrDefault(r => r.Guid == guid);
            if (resource == null)
                return false;
            dbContext.Resources.Remove(resource);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}