//using System.Threading.Tasks;
//using Microsoft.AspNet.Builder;
//using Microsoft.AspNet.Http;
//using Microsoft.Framework.Logging;

//namespace Multitenancy.Features
//{
//    public class TenantResolverMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger _logger;

//        public TenantResolverMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
//        {
//            _next = next;
//            _logger = loggerFactory.Create<TenantResolverMiddleware>();
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            using (_logger.BeginScope("TenantResolverMiddleware"))
//            {
//                var tenant = new Tenant
//                {
//                    Id = "Sample" // todo: determine this based on HttpContext etc.
//                };

//                _logger.WriteInformation(string.Format("Resolved tenant. Current tenant: {0}", tenant.Id));

//                var tenantFeature = new TenantFeature(tenant);
//                context.SetFeature<ITenantFeature>(tenantFeature);

//                await _next(context);
//            }
//        }
//    }
//}