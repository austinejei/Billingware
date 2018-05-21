using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Billingware.Common.Api
{
    /// <summary>
    /// </summary>
    public class SwaggerHideFilter : IDocumentFilter
    {
     

        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry,
            System.Web.Http.Description.IApiExplorer apiExplorer)
        {
            foreach (var apiDescription in apiExplorer.ApiDescriptions)
            {
                if (!apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<SwaggerHideAttribute>().Any() && !apiDescription.ActionDescriptor.GetCustomAttributes<SwaggerHideAttribute>().Any()) continue;
                var route = "/" + apiDescription.Route.RouteTemplate.TrimEnd('/');
                swaggerDoc.paths.Remove(route);
            }
        }
    }
}