using System;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;

namespace MailUtility
{

    /// <summary>
    /// 使用SharpZipLib来完成打包解包
    /// </summary>
    public class ZipHelper
    {
        private readonly ILogger _logger;

        public ZipHelper(ILogger<ZipHelper> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="zipFileName">输出压缩文件名称</param>
        /// <param name="sourceFolderName">需要压缩的文件夹名称</param>
        /// <returns>成功true,失败false</returns>
        public bool Pack(string zipFileName, string sourceFolderName)
        {
            try
            {
                var fastZip = CreateZipComponent();

                fastZip.CreateZip(zipFileName, sourceFolderName, true, null);
                return true;
            }
            catch (Exception ex)
            {
                // 记录一个未处理异常的日志
                _logger.LogError(ex.ToString());
            }

            return false;
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="zipFileName">压缩文件名称</param>
        /// <param name="targetFolderName">解压缩的目标文件夹名称</param>
        /// <returns>成功true,失败false</returns>
        public bool Unpack(string zipFileName, string targetFolderName)
        {
            try
            {
                var fastZip = CreateZipComponent();
                fastZip.ExtractZip(zipFileName, targetFolderName, FastZip.Overwrite.Always, null, null, null, true);
                return true;
            }
            catch (Exception ex)
            {
                // 记录一个未处理异常的日志
                _logger.LogError(ex.ToString());
            }

            return false;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        private FastZip CreateZipComponent()
        {
            var fastZip = new FastZip
            {
                CreateEmptyDirectories = true,
                RestoreAttributesOnExtract = true,
                RestoreDateTimeOnExtract = true
            };
            return fastZip;
        }

    }
}
