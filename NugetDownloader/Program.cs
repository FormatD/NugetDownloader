using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace NugetDownloader
{
    class Program
    {

        static void Main(string[] args)
        {
            const string packageName = "microsoft.aspnet.mvc";
            args = new string[] { packageName };

            var downloader = new Downloader();
            var package = downloader.GetPackage(packageName, "");

            var urls = downloader.AnalysisAllDownloadAddress(package).Distinct().ToList();

            var files = downloader.Download(urls);
            var zipFile = $"{packageName}.zip";
            downloader.Pack(files, zipFile);


            Console.ReadKey();
        }

    }

    public class Downloader
    {
        private static string _packageIndexTemplate = "https://nuget.cnblogs.com/v3/registration3/{0}/index.json";
        private static WebClient _client = new WebClient();
        private string _downloadFolder = "packages";

        public Package GetPackage(string packageName, string versionRange)
        {
            packageName = packageName?.ToLower();
            var jsonString = _client.DownloadString(string.Format(_packageIndexTemplate, packageName));
            var json = (JObject)JsonConvert.DeserializeObject(jsonString);

            var root = new Creator().Create(json) as RootCatalog;

            var package = GetPackage(root);
            // xx.DownloadFile(content, "xx.zip");
            return package;
        }

        public IEnumerable<string> AnalysisAllDownloadAddress(Package package)
        {
            yield return package.packageContent;
            foreach (var group in package.catalogEntry?.dependencyGroups?.SelectMany(x => x.dependencies) ?? new List<PackageDependency>())
            {
                if (group == null)
                    continue;

                var dependencePackage = GetPackage(group.packageName, group.range);
                foreach (var packageUrl in AnalysisAllDownloadAddress(dependencePackage))
                {
                    yield return packageUrl;
                }
            }
        }

        public IEnumerable<String> Download(IEnumerable<String> nugetPackageUrls)
        {
            if (!Directory.Exists(_downloadFolder))
                Directory.CreateDirectory(_downloadFolder);

            foreach (var url in nugetPackageUrls)
            {
                var savePath = Path.Combine(_downloadFolder, Path.GetFileName(url));
                Console.WriteLine($"Downloading {url} to {savePath}");
                _client.DownloadFile(url, savePath);
                yield return savePath;
            }
        }

        public void Pack(IEnumerable<String> files, string zipFile)
        {
            var folder = GetTempPath();
            Directory.CreateDirectory(folder);
            foreach (var file in files)
            {
                File.Copy(file, Path.Combine(folder, Path.GetFileName(file)));
            }

            var zip = new FastZip();

            zip.CreateZip(zipFile, folder, true, "");
        }

        private static string GetTempPath()
        {
            var tmpdir = Path.GetTempFileName();
            File.Delete(tmpdir);

            return tmpdir;
        }

        public static Package GetPackage(Node node)
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

            RetrieveCatalogAttribute(token, node);
            RetriveNodeAttribute(token, node);
            RetrivePackageAttribute(token, node);
            RetriveCatalogEntity(token, node);

            return node;
        }

        private void RetriveCatalogEntity(JToken token, Package node)
        {
            node.catalogEntry = CreateCatalogEntity(token["catalogEntry"]);
        }

        private CatalogEntry CreateCatalogEntity(JToken token)
        {
            var catalogEntry = new CatalogEntry();
            RetriveNodeAttribute(token, catalogEntry);
            catalogEntry.authors = token["authors"].Value<String>();
            catalogEntry.dependencyGroups = CreateDependenceGroup(token["dependencyGroups"]);

            return catalogEntry;
        }

        private List<PackageDependencyGroup> CreateDependenceGroup(JToken token)
        {
            var groups = new List<PackageDependencyGroup>();

            var jArray = token as JArray;
            if (jArray == null || jArray.Count == 0)
            {
                return groups;
            }

            foreach (var item in jArray)
            {
                var dependencyGroup = new PackageDependencyGroup();
                var dendenceGroup = jArray.First;
                RetriveNodeAttribute(dendenceGroup, dependencyGroup);
                dependencyGroup.dependencies = CreateDependences(dendenceGroup["dependencies"] as JArray);
                groups.Add(dependencyGroup);
            }

            return groups;
        }

        private ICollection<PackageDependency> CreateDependences(JArray array)
        {
            if (array == null)
                return null;
            var dependencies = new List<PackageDependency>();
            foreach (var token in array)
            {
                dependencies.Add(CreateDependence(token));
            }

            return dependencies;
        }

        private PackageDependency CreateDependence(JToken token)
        {
            var dependency = new PackageDependency();
            RetriveNodeAttribute(token, dependency);


            dependency.packageName = token["id"].Value<String>();
            dependency.range = token["range"].Value<String>();
            dependency.registration = token["registration"].Value<String>();

            return dependency;
        }

        private Node CreateCatalogPage(JToken token)
        {
            var node = new PageCatalog();

            RetriveNodeAttribute(token, node);
            RetrivePageCatalogAttribute(token, node);
            RetrieveItems(token, node);

            return node;
        }

        private RootCatalog CreateRoot(JToken token)
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

        private void RetriveNodeAttribute(JToken token, Node node)
        {
            node.id = token["@id"].Value<String>();
            //node.type = token["@type"].Value<String>();
        }

        private static void RetrieveCatalogAttribute(JToken token, Catalog node)
        {
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

        public override string ToString()
        {
            return $"{type}:{id}";
        }

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
        public CatalogEntry catalogEntry { get; set; }

        public string packageContent { get; set; }

        public string registration { get; set; }

    }

    public class CatalogEntry : Node
    {
        public string authors { get; set; }

        public List<PackageDependencyGroup> dependencyGroups { get; set; }

    }

    public class PackageDependencyGroup : Node
    {
        public ICollection<PackageDependency> dependencies { get; set; }

    }

    public class PackageDependency : Node
    {
        internal string packageName { get; set; }

        public string range { get; set; }

        public string registration { get; set; }

        public override string ToString()
        {
            return $"{packageName}({range})";
        }
    }
}
