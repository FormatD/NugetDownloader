using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NugetDownloader
{
    class Program
    {

        private string rootUrl = "https://nuget.cnblogs.com/v3";
        private static string _packageIndexTemplate = "https://nuget.cnblogs.com/v3/registration3/{0}/index.json";

        static void Main(string[] args)
        {
            args = new string[] { "MICROSOFT.ASPNET.MVC".ToLower() };

            var packageName = args.FirstOrDefault();
            var xx = new WebClient();
            var jsonString = xx.DownloadString(string.Format(_packageIndexTemplate, packageName));
            var json = (JObject)JsonConvert.DeserializeObject(jsonString);

            var node = new Creator().Create(json);

            var package = GetPackage(node);
            var content = package.packageContent;
            xx.DownloadFile(content, "xx.zip");

        }

        private static Package GetPackage(Node node)
        {
            if (node is Package)
                return (Package)node;
            else
                foreach (var item in (node as Catalog)?.items)
                {
                    var package = GetPackage(item);
                    if (package != null)
                        return package;
                }

            return null;
        }
    }

    public class Creator
    {
        public Node Create(JToken token)
        {
            var type = token["@type"];
            if (type.Type == JTokenType.Array)
                return CreateRoot(token);

            switch (type.Value<String>())
            {
                case "catalog:CatalogPage":
                    return CreateCatalogPage(token);
                case "Package":
                    return CreatePackage(token);

                default:
                    break;
            }

            return null;
        }

        private Node CreatePackage(JToken token)
        {
            var node = new Package();

            RetriveNodeAttribute(token, node);
            RetrivePackageAttribute(token, node);

            return node;
        }

        private Node CreateCatalogPage(JToken token)
        {
            var node = new PageCatalog();

            RetriveNodeAttribute(token, node);
            RetrivePageCatalogAttribute(token, node);
            RetrieveItems(token, node);

            return node;
        }

        private Node CreateRoot(JToken token)
        {
            var node = new RootCatalog();
            RetriveNodeAttribute(token, node);
            RetrieveItems(token, node);

            return node;
        }

        private void RetrieveItems(JToken token, Catalog node)
        {
            var items = token["items"] as JArray;

            node.items = items.Select(x => Create(x)).ToList();
        }

        private void RetriveNodeAttribute(JToken token, Catalog node)
        {
            node.id = token["@id"].Value<String>();
            //node.type = token["@type"].Value<String>();
            node.commitId = token["commitId"].Value<String>();
            node.commitTimeStamp = token["commitTimeStamp"].Value<String>();
        }

        private void RetrivePageCatalogAttribute(JToken token, PageCatalog node)
        {
            node.lower = token["lower"].Value<String>();
            node.upper = token["upper"].Value<String>();
        }



        private void RetrivePackageAttribute(JToken token, Package node)
        {
            node.packageContent = token["packageContent"].Value<String>();
            node.registration = token["registration"].Value<String>();
        }
    }

    public class Node
    {
        public string id { get; set; }

        public string type { get; set; }

    }

    public class Catalog : Node
    {

        public string commitId { get; set; }

        public string commitTimeStamp { get; set; }

        public ICollection<Node> items { get; set; }

    }

    public class RootCatalog : Catalog
    {

        public string count { get; set; }

    }

    public class PageCatalog : RootCatalog
    {

        public string lower { get; set; }

        public string upper { get; set; }

        public string parent { get; set; }
    }

    public class Package : Catalog
    {
        public string catalogEntry { get; set; }

        public string packageContent { get; set; }

        public string registration { get; set; }

    }
}
