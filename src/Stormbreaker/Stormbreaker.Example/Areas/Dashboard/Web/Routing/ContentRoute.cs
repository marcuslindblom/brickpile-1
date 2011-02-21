using System.Web.Mvc;
using System.Web.Routing;
using Stormbreaker.Extensions;
using Stormbreaker.Models;
using Stormbreaker.Web;

namespace Dashboard.Web.Routing {

    public class ContentRoute : RouteBase, IRouteWithArea {

        private readonly IPathResolver _pathResolver;
        private readonly IVirtualPathResolver _virtualPathResolver;
        private readonly IRouteHandler _routeHandler;
        private readonly Route _innerRoute;
        public const string ControllerKey = "controller";

        public string Area
        {
            get { return "Dashboard"; }
        }
        public static string ModelKey
        {
            get { return "model"; }
        }

        public static string ActionKey
        {
            get { return "action"; }
        }

        public static string DefaultAction
        {
            get { return "index"; }
        }

        public ContentRoute(IPathResolver pathResolver, IVirtualPathResolver virtualPathResolver, Route innerRoute)
        {
            _pathResolver = pathResolver;
            _virtualPathResolver = virtualPathResolver;
            _routeHandler = _routeHandler ?? new MvcRouteHandler();
            _innerRoute = innerRoute ?? new Route("dashboard/{controller}/{action}",
                                                        new RouteValueDictionary(new { controller = "content", action = "index" }),
                                                        new RouteValueDictionary(),
                                                        new RouteValueDictionary( new { area ="Dashboard" } ),
                                                        new MvcRouteHandler());
        }
        public override RouteData GetRouteData(System.Web.HttpContextBase httpContext)
        {
            // get the virtual path of the request
            var virtualPath = httpContext.Request.CurrentExecutionFilePath;
            
            // abort if the virtual path does not contain dashboard
            if (!IsDashboardRoute(virtualPath))
            {
                return null;
            }

            // try to resolve the current item
            var pathData = _pathResolver.ResolvePath(virtualPath.Replace("dashboard/content/", "").Trim(new[] { '/' }));

            var routeData = new RouteData(this, _routeHandler);

            foreach (var defaultPair in _innerRoute.Defaults)
                routeData.Values[defaultPair.Key] = defaultPair.Value;
            foreach (var tokenPair in _innerRoute.DataTokens)
                routeData.DataTokens[tokenPair.Key] = tokenPair.Value;

            // Abort and proceed to other routes in the route table
            if (pathData == null) {
                return null;
            }

            routeData.ApplyCurrentModel("content", pathData.Action, pathData.CurrentPageModel);

            return routeData;
        }
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {
            if(!IsDashboardRoute(requestContext.HttpContext.Request.CurrentExecutionFilePath)) {
                return null;
            }

            var item = values[ModelKey] as IPageModel;

            if (item == null)
                return null;

            var vpd = _innerRoute.GetVirtualPath(requestContext, values);

            if (vpd == null)
                return null;

            vpd.Route = this;



            vpd.VirtualPath = string.Format("dashboard/content/{0}", _virtualPathResolver.ResolveVirtualPath(item, values));

            return vpd;
        }
        private static bool IsDashboardRoute(string virtualPath)
        {
            return virtualPath.StartsWith("/dashboard/content");
        }
    }
}