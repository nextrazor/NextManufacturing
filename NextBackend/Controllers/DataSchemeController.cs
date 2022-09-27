using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataSchemeController : Controller
    {
        private readonly IStringLocalizer<DataSchemeController> _localizer;

        public DataSchemeController(IStringLocalizer<DataSchemeController> localizer)
        {
            _localizer = localizer;
        }

        [HttpGet]
        [Route("{entityName:required}")]
        public EntityDesc Read(string entityName)
        {
            Type? type = Type.GetType($"NextBackend.DAL.{entityName}");
            if (type == null)
                throw new Exception(_localizer["Entity not found"]);
            EntityDesc desc = new()
            {
                Name = new(type.Name, _localizer[type.Name])
            };

            var x = type.GetProperties();

            foreach (var prop in type.GetProperties())
            {
                desc.Fields.Add(prop.Name, _localizer[$"{entityName}.{prop.Name}"]);
            }
            return desc;
        }
    }

    public class EntityDesc
    {
        public KeyValuePair<string, string> Name { get; set; }
        public Dictionary<string, string> Fields { get; set; } = new();
    }
}
