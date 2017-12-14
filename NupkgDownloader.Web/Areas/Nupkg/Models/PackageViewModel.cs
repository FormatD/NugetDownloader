using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Versioning;

namespace NupkgDownloader.Web.Models
{
    public class PackageViewModel
    {
        public PackageViewModel(IPackageSearchMetadata packageSearchMetadata)
        {
            ProjectUrl = packageSearchMetadata.ProjectUrl;
            Tags = packageSearchMetadata.Tags;
            Summary = packageSearchMetadata.Summary;
            RequireLicenseAcceptance = packageSearchMetadata.RequireLicenseAcceptance;
            Owners = packageSearchMetadata.Owners;
            Published = packageSearchMetadata.Published;
            Title = packageSearchMetadata.Title;
            IsListed = packageSearchMetadata.IsListed;
            IconUrl = packageSearchMetadata.IconUrl;
            DownloadCount = packageSearchMetadata.DownloadCount;
            Description = packageSearchMetadata.Description;
            DependencySets = packageSearchMetadata.DependencySets;
            Authors = packageSearchMetadata.Authors;

            Id = packageSearchMetadata.Identity.Id;
            CurrentVersion = packageSearchMetadata.Identity?.Version;
        }

        public Uri ProjectUrl { get; }
        public string Tags { get; }
        public string Summary { get; }
        public bool RequireLicenseAcceptance { get; }
        public string Owners { get; }
        public DateTimeOffset? Published { get; }
        public Uri ReportAbuseUrl { get; }
        public string Title { get; }
        public bool IsListed { get; }
        //public PackageIdentity Identity { get; }
        public Uri IconUrl { get; }
        public long? DownloadCount { get; }
        public string Description { get; }
        public IEnumerable<PackageDependencyGroup> DependencySets { get; }
        public string Authors { get; }
        public string Id { get; }
        public Uri LicenseUrl { get; }
        public NuGetVersion CurrentVersion { get; }
        public IEnumerable<RemoteSourceDependencyInfo> ReleatedPackages { get; internal set; }
    }
}
