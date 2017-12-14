using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Http;

namespace Multitenancy.Routing
{
    public class DomainTemplateRoute
        : INamedRouter
    {
        private readonly Route _innerRoute;
        private readonly IRouter _target;
        private readonly string _domainTemplate;
        private readonly TemplateMatcher _matcher;
        private ILogger _logger;

        public DomainTemplateRoute(IRouter target, string domainTemplate, string routeTemplate, bool ignorePort, IInlineConstraintResolver inlineConstraintResolver)
            : this(target, domainTemplate, routeTemplate, null, null, null, ignorePort, inlineConstraintResolver)
        {
        }

        public DomainTemplateRoute(IRouter target, string domainTemplate, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, bool ignorePort, IInlineConstraintResolver inlineConstraintResolver)
            : this(target, null, domainTemplate, routeTemplate, defaults, constraints, dataTokens, ignorePort, inlineConstraintResolver)
        {
        }

        public DomainTemplateRoute(IRouter target, string routeName, string domainTemplate, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, bool ignorePort, IInlineConstraintResolver inlineConstraintResolver)
        {
            _innerRoute = new Route(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver);

            _target = target;
            _domainTemplate = domainTemplate;

            _matcher = new TemplateMatcher(
                TemplateParser.Parse(DomainTemplate), Defaults);

            Name = routeName;
            IgnorePort = ignorePort;
        }

        public string Name { get; private set; }

        public RouteValueDictionary Defaults
        {
            get
            {
                return _innerRoute.Defaults;
            }
        }

        public RouteValueDictionary DataTokens
        {
            get
            {
                return _innerRoute.DataTokens;
            }
        }

        public string RouteTemplate
        {
            get
            {
                return _innerRoute.RouteTemplate;
            }
        }

        public IDictionary<string, IRouteConstraint> Constraints
        {
            get
            {
                return _innerRoute.Constraints;
            }
        }

        public string DomainTemplate
        {
            get
            {
                return _domainTemplate;
            }
        }

        public bool IgnorePort { get; set; }

        public async Task RouteAsync(RouteContext context)
        {
            EnsureLoggers(context.HttpContext);
            using (_logger.BeginScope("DomainTemplateRoute.RouteAsync"))
            {
                var requestHost = context.HttpContext.Request.Host.Value;
                if (IgnorePort && requestHost.Contains(":"))
                {
                    requestHost = requestHost.Substring(0, requestHost.IndexOf(":"));
                }

                var values = new RouteValueDictionary();
                //if (_matcher.TryMatch(requestHost, values))
                //{
                //    if (_logger.IsEnabled(LogLevel.Trace))
                //    {
                //        _logger.LogTrace("DomainTemplateRoute " + Name + " - Host \"" + context.HttpContext.Request.Host + "\" did not match.");
                //    }

                //    // If we got back a null value set, that means the URI did not match
                //    return;
                //}

                var oldRouteData = context.RouteData;

                var newRouteData = new RouteData(oldRouteData);
                MergeValues(newRouteData.DataTokens, DataTokens);
                newRouteData.Routers.Add(_target);
                MergeValues(newRouteData.Values, values.ToImmutableDictionary());

                try
                {
                    context.RouteData = newRouteData;

                    // delegate further processing to inner route
                    await _innerRoute.RouteAsync(context);
                }
                finally
                {
                    // Restore the original values to prevent polluting the route data.
                    //TODO if (!context.IsHandled)
                    //{
                        context.RouteData = oldRouteData;
                    //}
                }
            }
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            foreach (var matcherParameter in _matcher.Template.Parameters)
            {
                context.Values.Remove(matcherParameter.Name); // make sure none of the domain-placeholders are appended as query string parameters
            }

            return _innerRoute.GetVirtualPath(context);
        }

        private static void MergeValues(IDictionary<string, object> destination, IReadOnlyDictionary<string, object> values)
        {
            if (values == null)
                return;

            foreach (var kvp in values)
            {
                // This will replace the original value for the specified key.
                // Values from the matched route will take preference over previous
                // data in the route context.
                destination[kvp.Key] = kvp.Value;
            }
        }

        private void EnsureLoggers(HttpContext context)
        {
            if (_logger == null)
            {
                var factory = (ILoggerFactory)context.RequestServices.GetService(typeof(ILoggerFactory));
                _logger = factory.CreateLogger<Route>();
            }
        }

        public override string ToString()
        {
            return _domainTemplate + "/" + RouteTemplate;
        }
    }
}
