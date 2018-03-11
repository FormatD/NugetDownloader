using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol;
using NuGet.Configuration;
using System.Threading;
using NupkgDownloader.Web.Models;
using NuGet.Versioning;
using MailUtility;
using System.IO;
using NuGet.Packaging.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NupkgDownloader.Web.NupkgControllers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace NupkgDownloader.Web.Areas.Nupkg.Controllers
{
    public class PackageController : Controller
    {
        LoggerWrapper<PackageController> _nugetLogger;
        private readonly ILogger<PackageController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _downloadFolder = "nupkg_cache";

        public PackageController(UserManager<ApplicationUser> userManager,
            ILogger<PackageController> logger,
            IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;

            _nugetLogger = new LoggerWrapper<PackageController>(_logger);
        }

        async public Task<IActionResult> Index(string keyword)
        {
            _logger.LogInformation("Query Package with:{0}", keyword);
            keyword = keyword?.Trim() ?? string.Empty;
            IEnumerable<IPackageSearchMetadata> searchMetadata = await SearchResult(keyword);

            var result = new SearchResultViewModel
            {
                Keyword = keyword,
                Result = searchMetadata.Select(x => new PackageViewModel(x)),
            };
            return View(result);
        }

        private async Task<IEnumerable<IPackageSearchMetadata>> SearchResult(string keyword)
        {
            IEnumerable<IPackageSearchMetadata> searchMetadata = Enumerable.Empty<IPackageSearchMetadata>();
            try
            {
                SourceRepository sourceRepository = CreateRepository();

                PackageMetadataResource packageMetadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();

                PackageSearchResource searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
                searchMetadata = await searchResource.SearchAsync(keyword, new SearchFilter(false), 0, 10, _nugetLogger, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Search packages fail,{0}", ex);
            }
            return searchMetadata;
        }

        async public Task<IActionResult> Detail(string id, string versionNo = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            SourceRepository sourceRepository = CreateRepository();
            var metadataResource = await sourceRepository.GetResourceAsync<MetadataResource>();
            var version = !string.IsNullOrWhiteSpace(versionNo) ? new NuGetVersion(versionNo) : await GetVersion(id, metadataResource);

            if (version == null)
                return NotFound();

            var identity = new PackageIdentity(id, version);

            var searchResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();
            var packageMetaInfo = await searchResource.GetMetadataAsync(identity, _nugetLogger, CancellationToken.None);

            var findPackageByIdResource = await sourceRepository.GetResourceAsync<FindPackageByIdResource>();
            var dependencyInfo = await findPackageByIdResource.GetDependencyInfoAsync(id, version, new SourceCacheContext(), _nugetLogger, CancellationToken.None);

            var dependenceResource = sourceRepository.GetResource<DependencyInfoResource>();
            // NuGetFramework.ParseFrameworkName("4.5", new DefaultFrameworkNameProvider())
            var reletedPackages = await dependenceResource.ResolvePackages(id, _nugetLogger, CancellationToken.None);

            var pacakgeViewModel = new PackageViewModel(packageMetaInfo)
            {
                ReleatedPackages = reletedPackages,
                DependencySets = dependencyInfo.DependencyGroups,
            };

            //var xxResource = sourceRepository.GetResource<DownloadResource>();
            //var downloadRsult = await xxResource.GetDownloadResourceResultAsync(identity, new PackageDownloadContext(new SourceCacheContext()), "test", _nugetLogger, CancellationToken.None);
            return View(pacakgeViewModel);
        }

        private async Task<NuGetVersion> GetVersion(string id, MetadataResource metadataResource)
        {
            return await metadataResource.GetLatestVersion(id, false, false, _nugetLogger, CancellationToken.None);
        }

        [Authorize]
        async public Task<IActionResult> Download(string id, string versionNo = null)
        {
            SourceRepository sourceRepository = CreateRepository();
            var metadataResource = await sourceRepository.GetResourceAsync<MetadataResource>();
            var version = !string.IsNullOrWhiteSpace(versionNo) ? new NuGetVersion(versionNo) : await GetVersion(id, metadataResource);

            var lowCaseId = id.ToLower();
            var fileName = Path.Combine(_downloadFolder, lowCaseId, version.ToString(), $"{lowCaseId}.{version}.nupkg");
            if (!System.IO.File.Exists(fileName))
            {
                var xxResource = sourceRepository.GetResource<DownloadResource>();
                var identity = new PackageIdentity(id, version);
                var downloadRsult = await xxResource.GetDownloadResourceResultAsync(identity, new PackageDownloadContext(new SourceCacheContext()), _downloadFolder, _nugetLogger, CancellationToken.None);

                if (downloadRsult.Status == DownloadResourceResultStatus.NotFound)
                    return NotFound();
                return File(downloadRsult.PackageStream, "application/x-nuget", $"{id}.{version}.nupkg");
            }
            else
            {
                return File(Path.Combine(_hostingEnvironment.WebRootPath, fileName), "application/x-nuget", $"{id}.{version}.nupkg");
            }
        }

        [Authorize]
        async public Task<IActionResult> Send(string id, string versionNo = null, string email = null)
        {
            _logger.LogInformation("Sending {0}.", id);
            SourceRepository sourceRepository = CreateRepository();
            var metadataResource = await sourceRepository.GetResourceAsync<MetadataResource>();
            var version = !string.IsNullOrWhiteSpace(versionNo) ? new NuGetVersion(versionNo) : await GetVersion(id, metadataResource);

            var lowCaseId = id.ToLower();
            var fileName = Path.Combine(_downloadFolder, lowCaseId, version.ToString(), $"{lowCaseId}.{version}.nupkg");
            if (!System.IO.File.Exists(fileName))
            {
                var downloadResource = sourceRepository.GetResource<DownloadResource>();
                var identity = new PackageIdentity(id, version);
                var downloadRsult = await downloadResource.GetDownloadResourceResultAsync(identity, new PackageDownloadContext(new SourceCacheContext()), _downloadFolder, _nugetLogger, CancellationToken.None);

                if (downloadRsult.Status == DownloadResourceResultStatus.NotFound)
                {
                    return Json(new
                    {
                        Id = id,
                        Version = versionNo,
                        IsSuccessed = false,
                        Message = "Not Found",
                    });
                }
            }

            var config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText("mail_config.json"));

            var user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);
            if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                config.SendTo = user.Email;

            _logger.LogInformation("Sending {0} to {1}.", id, email);
            //var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            new MailSender(config, _logger).SendEmail(fileName, $"[Nuget]{lowCaseId} {version}");
            _logger.LogInformation("package sended.");
            return Json(new SendViewModel
            {
                Id = id,
                Version = version.Version.ToString(),
                IsSuccessed = true,
                Message = "OK",
            });
        }

        private static SourceRepository CreateRepository()
        {
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
            //providers.AddRange(Repository.Provider.GetCoreV2());  // Add v2 API support
            PackageSource packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            SourceRepository sourceRepository = new SourceRepository(packageSource, providers);
            return sourceRepository;
        }
    }

    public static class XxExtension
    {
        public static void Dump(this object o)
        { }
    }
}