// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGet.PackageManagement.UI;
using NupkgDownloader.Web.Controllers;

namespace NupkgDownloader.Web.Models
{
	/// <summary>
	/// Helper class encapsulating common scenarios of source repository operations.
	/// </summary>
	internal static class SourceRepositoryExtensions
	{
		public static Task<SearchResult<IPackageSearchMetadata>> SearchAsync(this SourceRepository sourceRepository, string searchText, SearchFilter searchFilter, int pageSize, CancellationToken cancellationToken)
		{
			var searchToken = new FeedSearchContinuationToken
			{
				SearchString = searchText,
				SearchFilter = searchFilter,
				StartIndex = 0
			};

			return sourceRepository.SearchAsync(searchToken, pageSize, cancellationToken);
		}

		public static async Task<SearchResult<IPackageSearchMetadata>> SearchAsync(
			this SourceRepository sourceRepository, ContinuationToken continuationToken, int pageSize, CancellationToken cancellationToken)
		{
			var searchToken = continuationToken as FeedSearchContinuationToken;
			if (searchToken == null)
			{
				throw new InvalidOperationException("Invalid token");
			}

			var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>(cancellationToken);

			IEnumerable<IPackageSearchMetadata> searchResults = null;
			if (searchResource != null) {
				searchResults = await searchResource.SearchAsync(
				searchToken.SearchString,
				searchToken.SearchFilter,
				searchToken.StartIndex,
				pageSize + 1,
				Logging.NullLogger,
				cancellationToken);
			}

			var items = searchResults?.ToArray() ?? new IPackageSearchMetadata[] { };

			var hasMoreItems = items.Length > pageSize;
			if (hasMoreItems)
			{
				items = items.Take(items.Length - 1).ToArray();
			}

			var result = SearchResult.FromItems(items);

			var loadingStatus = hasMoreItems
				? LoadingStatus.Ready
			                   : items.Length == 0
			                   ? LoadingStatus.NoItemsFound
			                   : LoadingStatus.NoMoreItems;
			result.SourceSearchStatus = new Dictionary<string, LoadingStatus>
			{
				{ sourceRepository.PackageSource.Name, loadingStatus }
			};

			if (hasMoreItems)
			{
				result.NextToken = new FeedSearchContinuationToken
				{
					SearchString = searchToken.SearchString,
					SearchFilter = searchToken.SearchFilter,
					StartIndex = searchToken.StartIndex + items.Length
				};
			}

			return result;
		}

		public static async Task<IPackageSearchMetadata> GetPackageMetadataAsync(
			this SourceRepository sourceRepository, PackageIdentity identity, bool includePrerelease, CancellationToken cancellationToken)
		{
			var metadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
			IEnumerable<IPackageSearchMetadata> packages = null;
			if (metadataResource != null) {
				packages = await metadataResource.GetMetadataAsync(
					identity.Id,
					includePrerelease: true,
					includeUnlisted: false,
					log: Logging.NullLogger,
					token: cancellationToken);
			}

			if (packages?.FirstOrDefault() == null)
			{
				return null;
			}

			var packageMetadata = packages
				.FirstOrDefault(p => p.Identity.Version == identity.Version)
				?? PackageSearchMetadataBuilder.FromIdentity(identity).Build();

			return packageMetadata.WithVersions(ToVersionInfo(packages, includePrerelease));
		}

		public static async Task<IPackageSearchMetadata> GetPackageMetadataFromLocalSourceAsync(
			this SourceRepository localRepository, PackageIdentity identity, CancellationToken cancellationToken)
		{
			var localResource = await localRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
			IEnumerable<IPackageSearchMetadata> localPackages = null;
			if (localResource != null) {
				localPackages = await localResource.GetMetadataAsync(
					identity.Id,
					includePrerelease: true,
					includeUnlisted: true,
					log: Logging.NullLogger,
					token: cancellationToken);
			}

			var packageMetadata = localPackages?.FirstOrDefault(p => p.Identity.Version == identity.Version);

			var versions = new[]
			{
				new VersionInfo(identity.Version)
			};

			return packageMetadata?.WithVersions(versions);
		}

		public static async Task<IPackageSearchMetadata> GetLatestPackageMetadataAsync(
			this SourceRepository sourceRepository, string packageId, bool includePrerelease, CancellationToken cancellationToken)
		{
			var metadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
			IEnumerable<IPackageSearchMetadata> packages = null;
			if (metadataResource != null) {
				packages = await metadataResource.GetMetadataAsync(
					packageId,
					includePrerelease,
					false,
					Logging.NullLogger,
					cancellationToken);
			}

			var highest = packages?
				.OrderByDescending(e => e.Identity.Version, VersionComparer.VersionRelease)
				.FirstOrDefault();

			return highest?.WithVersions(ToVersionInfo(packages, includePrerelease));
		}

		public static async Task<IEnumerable<IPackageSearchMetadata>> GetPackageMetadataListAsync(
			this SourceRepository sourceRepository, string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken)
		{
			var metadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
			IEnumerable<IPackageSearchMetadata> packages = null;
			if (metadataResource != null) {
				packages = await metadataResource.GetMetadataAsync(
					packageId,
					includePrerelease,
					includeUnlisted,
					Logging.NullLogger,
					cancellationToken);
			}
			return packages;
		}

		private static IEnumerable<VersionInfo> ToVersionInfo(IEnumerable<IPackageSearchMetadata> packages, bool includePrerelease)
		{
			return packages?
				.Where(v => includePrerelease || !v.Identity.Version.IsPrerelease)
				.OrderByDescending(m => m.Identity.Version, VersionComparer.VersionRelease)
				.Select(m => new VersionInfo(m.Identity.Version, m.DownloadCount));
		}

		public static async Task<IEnumerable<string>> IdStartsWithAsync(
			this SourceRepository sourceRepository, string packageIdPrefix, bool includePrerelease, CancellationToken cancellationToken)
		{
			var autoCompleteResource = await sourceRepository.GetResourceAsync<AutoCompleteResource>(cancellationToken);
			IEnumerable<string> packageIds = null;
			if (autoCompleteResource != null) {
				packageIds = await autoCompleteResource.IdStartsWith(
					packageIdPrefix,
					includePrerelease: includePrerelease,
					log: Logging.NullLogger,
					token: cancellationToken);
			}

			return packageIds ?? Enumerable.Empty<string>();
		}

		public static async Task<IEnumerable<NuGetVersion>> VersionStartsWithAsync(
			this SourceRepository sourceRepository, string packageId, string versionPrefix, bool includePrerelease, CancellationToken cancellationToken)
		{
			var autoCompleteResource = await sourceRepository.GetResourceAsync<AutoCompleteResource>(cancellationToken);
			IEnumerable<NuGetVersion> versions = null;
			if (autoCompleteResource != null) {
				versions = await autoCompleteResource.VersionStartsWith(
					packageId,
					versionPrefix,
					includePrerelease: includePrerelease,
					log: Logging.NullLogger,
					token: cancellationToken);
			}

			return versions ?? Enumerable.Empty<NuGetVersion>();
		}
	}

    /// <summary>
    /// Most commonly used continuation token for plain package feeds.
    /// </summary>
    internal class FeedSearchContinuationToken : ContinuationToken
    {
        public int StartIndex { get; set; }
        public string SearchString { get; set; }
        public SearchFilter SearchFilter { get; set; }
    }

    public class Logging
    {
        public static Logger NullLogger { get; set; }
    }


    /// <summary>
    /// List of possible statuses of items loading operation (search).
    /// Utilized by item loader and UI for progress tracking.
    /// </summary>
    public enum LoadingStatus
    {
        Unknown, // not initialized
        Cancelled, // loading cancelled
        ErrorOccurred, // error occured
        Loading, // loading is running in background
        NoItemsFound, // loading complete, no items found
        NoMoreItems, // loading complete, no more items discovered beyond current page
        Ready // loading of current page is done, next page is available
    }

}
