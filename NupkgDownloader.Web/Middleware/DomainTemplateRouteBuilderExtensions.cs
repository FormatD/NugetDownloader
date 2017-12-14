using System;
using Microsoft.AspNetCore.Routing;

namespace Multitenancy.Routing
{
    public static class DomainTemplateRouteBuilderExtensions
    {
        public static IRouteBuilder MapDomainRoute(this IRouteBuilder routeCollectionBuilder, string name, string domainTemplate, string routeTemplate)
        {
            MapDomainRoute(routeCollectionBuilder, name, domainTemplate, routeTemplate, (object)null);
            return routeCollectionBuilder;
        }

        public static IRouteBuilder MapDomainRoute(this IRouteBuilder routeCollectionBuilder, string name, string domainTemplate, string routeTemplate, object defaults, bool ignorePort = true)
        {
            return MapDomainRoute(routeCollectionBuilder, name, domainTemplate, routeTemplate, defaults, null, ignorePort);
        }

        public static IRouteBuilder MapDomainRoute(this IRouteBuilder routeCollectionBuilder, string name, string domainTemplate, string routeTemplate, object defaults, object constraints, bool ignorePort = true)
        {
            return MapDomainRoute(routeCollectionBuilder, name, domainTemplate, routeTemplate, defaults, constraints, null, ignorePort);
        }

        public static IRouteBuilder MapDomainRoute(this IRouteBuilder routeCollectionBuilder, string name, string domainTemplate, string routeTemplate, object defaults, object constraints, object dataTokens, bool ignorePort = true)
        {
            if (routeCollectionBuilder.DefaultHandler == null)
                throw new InvalidOperationException("Default handler must be set.");
            var inlineConstraintResolver = (IInlineConstraintResolver)routeCollectionBuilder.ServiceProvider.GetService(typeof(IInlineConstraintResolver));
            routeCollectionBuilder.Routes.Add(new DomainTemplateRoute(routeCollectionBuilder.DefaultHandler, name, domainTemplate, routeTemplate, ObjectToDictionary(defaults), ObjectToDictionary(constraints), ObjectToDictionary(dataTokens), ignorePort, inlineConstraintResolver));
            return routeCollectionBuilder;
        }

        private static RouteValueDictionary ObjectToDictionary(object value)
        {
            return value as RouteValueDictionary ?? new RouteValueDictionary(value);
        }
    }
}