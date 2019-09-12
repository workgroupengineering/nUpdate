﻿// Copyright © Dominic Beger 2019

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate
{
    public class FileSystemUpdateProvider : UpdateProvider
    {
        public FileSystemUpdateProvider(string publicKey, IVersion applicationVersion, bool includePreRelease) : base(
            publicKey, applicationVersion, includePreRelease)
        {
        }

        public Uri PackageDataFile { get; set; }

        public override async Task<UpdateCheckResult> CheckForUpdates(CancellationToken cancellationToken)
        {
            var packageData = File.ReadAllText(PackageDataFile.ToString());
            var packages = JsonSerializer.Deserialize<IEnumerable<UpdatePackage>>(packageData);

            var result = new UpdateCheckResult();
            await result.Initialize(packages, ApplicationVersion, IncludePreRelease, cancellationToken);
            return result;
        }

        public Task DownloadPackage(UpdatePackage package, IProgress<UpdateProgressData> progress,
            CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var packageUri = package.PackageUri;
                var localPackagePath = Path.Combine(Globals.ApplicationUpdateDirectory, package.Guid.ToString(),
                    Globals.PackageExtension);
                File.Copy(packageUri.ToString(), localPackagePath, true);
            }, cancellationToken);
        }

        public override Task DownloadUpdates(UpdateCheckResult checkResult, CancellationToken cancellationToken,
            IProgress<UpdateProgressData> progress)
        {
            long downloadedBytes = 0;
            long totalBytes = checkResult.Packages.Sum(p => p.Size);
            return checkResult.Packages.ForEachAsync(async p =>
            {
                var packageProgress = new Progress<UpdateProgressData>();
                long currentlyDownloadedBytes = 0;
                packageProgress.ProgressChanged += (sender, data) =>
                {
                    downloadedBytes += data.BytesReceived - currentlyDownloadedBytes;
                    currentlyDownloadedBytes += data.BytesReceived;
                    progress.Report(new UpdateProgressData(downloadedBytes,
                        // ReSharper disable once PossibleLossOfFraction
                        totalBytes, (float) (downloadedBytes / totalBytes) * 100));
                };
                await DownloadPackage(p, packageProgress, cancellationToken);
            });
        }
    }
}