using Maple.Core.ComponentModel;
using Maple.Core.Configuration;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;


//Contributor: Umbraco (http://www.umbraco.com). Thanks a lot! 
//SEE THIS POST for full details of what this does - http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx

namespace Maple.Core.Plugins
{
    /// <summary>
    /// 应用程序插件管理器
    /// </summary>
    public class PluginManager
    {
        #region 属性字段

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static DirectoryInfo _shadowCopyFolder;
        private static readonly List<string> BaseAppLibraries;

        #endregion

        #region 构造函数

        static PluginManager()
        {
            //get all libraries from /bin/{version}/ directory
            BaseAppLibraries = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
                .GetFiles("*.dll", SearchOption.TopDirectoryOnly).Select(fi => fi.Name).ToList();

            //get all libraries from base site directory
            if (!AppDomain.CurrentDomain.BaseDirectory.Equals(Environment.CurrentDirectory, StringComparison.InvariantCultureIgnoreCase))
                BaseAppLibraries.AddRange(new DirectoryInfo(Environment.CurrentDirectory).GetFiles("*.dll", SearchOption.TopDirectoryOnly).Select(fi => fi.Name));

            //get all libraries from refs directory
            var refsPathName = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, RefsPathName));
            if (refsPathName.Exists)
                BaseAppLibraries.AddRange(refsPathName.GetFiles("*.dll", SearchOption.TopDirectoryOnly).Select(fi => fi.Name));
        }

        #endregion

        #region 非共有方法

        /// <summary>
        /// 获取插件的描述性文件
        /// </summary>
        /// <param name="pluginFolder"> /Plugins/bin 目录 </param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<FileInfo, PluginDescriptor>> GetDescriptionFilesAndDescriptors(DirectoryInfo pluginFolder)
        {
            if (pluginFolder == null)
                throw new ArgumentNullException(nameof(pluginFolder));

            var result = new List<KeyValuePair<FileInfo, PluginDescriptor>>();

            //遍历 /Plugins/bin 目录中名称是plugin.json的文件
            foreach (var descriptionFile in pluginFolder.GetFiles(PluginDescriptionFileName, SearchOption.AllDirectories))
            {
                // 确保当前目录是 Plugs 的子目录
                if (!IsPackagePluginFolder(descriptionFile.Directory))
                    continue;
                //读取文件获得插件的描述性信息
                var pluginDescriptor = GetPluginDescriptorFromFile(descriptionFile.FullName);
                result.Add(new KeyValuePair<FileInfo, PluginDescriptor>(descriptionFile, pluginDescriptor));
            }

            //对插件执行排序
            result.Sort((firstPair, nextPair) => firstPair.Value.DisplayOrder.CompareTo(nextPair.Value.DisplayOrder));
            return result;
        }

        /// <summary>
        /// 获取已安装的插件名称
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static IList<string> GetInstalledPluginNames(string filePath)
        {
            //检查文件是否存在
            if (!File.Exists(filePath))
                throw new Exception("文件 " + filePath + " 不存在.");
            var text = File.ReadAllText(filePath);

            IList<string> result = null;

            if (string.IsNullOrEmpty(text))
                result = new List<string>();
            else
                result = JsonConvert.DeserializeObject<IList<string>>(text);
            //该组件是否默认必须要加载的
            result.Add("Maple.Foundation.Setup");
            return result;
        }

        /// <summary>
        /// 保存已安装的插件清单
        /// </summary>
        /// <param name="pluginSystemNames"></param>
        /// <param name="filePath"></param>
        private static void SaveInstalledPluginNames(IList<string> pluginSystemNames, string filePath)
        {
            var text = JsonConvert.SerializeObject(pluginSystemNames, Formatting.Indented);
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// 检查程序集是否已经被应用程序域加载
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns>Result</returns>
        private static bool IsAlreadyLoaded(FileInfo fileInfo)
        {
            //在基目录中搜索
            if (BaseAppLibraries.Any(sli => sli.Equals(fileInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            //compare full assembly name
            //var fileAssemblyName = AssemblyName.GetAssemblyName(fileInfo.FullName);
            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (a.FullName.Equals(fileAssemblyName.FullName, StringComparison.InvariantCultureIgnoreCase))
            //        return true;
            //}
            //return false;

            //do not compare the full assembly name, just filename
            try
            {
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                if (string.IsNullOrEmpty(fileNameWithoutExt))
                    throw new Exception($"无法获取文件的后缀名 -- {fileInfo.Name}");

                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var assemblyName = a.FullName.Split(',').FirstOrDefault();
                    if (fileNameWithoutExt.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("无法验证程序集是否已经被加载. " + exc);
            }
            return false;
        }

        /// <summary>
        /// 执行插件部署
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="applicationPartManager"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static Assembly PerformFileDeploy(FileInfo plug, ApplicationPartManager applicationPartManager, MapleConfig config)
        {
            if (plug.Directory == null || plug.Directory.Parent == null)
                throw new InvalidOperationException("插件 " + plug.Name + " 所在文件夹必须在Plugins 目录下");

            //在指定路径中创建所有目录和子目录，除非它们已经存在 ~/Plugins/bin/ 并返回目录对象
            var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
            var shadowCopiedPlug = ShadowCopyFile(plug, shadowCopyPlugFolder);
            
            //注册插件
            //修改Dll加载函数 Assembly.Load To Assembly.LoadFrom
            //var assemblyName = AssemblyName.GetAssemblyName(shadowCopiedPlug.FullName);
            Assembly shadowCopiedAssembly;
            try
            {

                //shadowCopiedAssembly = Assembly.Load(assemblyName);
                //shadowCopiedAssembly = Assembly.LoadFrom(shadowCopiedPlug.FullName);
                shadowCopiedAssembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(shadowCopiedPlug.FullName);
                
            }
            catch (FileLoadException)
            {
                if (config.UseUnsafeLoadAssembly)
                {
                    //if an application has been copied from the web, it is flagged by Windows as being a web application,
                    //even if it resides on the local computer.You can change that designation by changing the file properties,
                    //or you can use the<loadFromRemoteSources> element to grant the assembly full trust.As an alternative,
                    //you can use the UnsafeLoadFrom method to load a local assembly that the operating system has flagged as
                    //having been loaded from the web.
                    //see http://go.microsoft.com/fwlink/?LinkId=155569 for more information.
                    shadowCopiedAssembly = Assembly.UnsafeLoadFrom(shadowCopiedPlug.FullName);
                }
                else
                {
                    throw;
                }
            }

            Debug.WriteLine("添加 ApplicationParts: '{0}'", shadowCopiedAssembly.FullName);
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(shadowCopiedAssembly));

            return shadowCopiedAssembly;
        }

        /// <summary>
        /// 拷贝插件文件至Plugins/Bin目录下
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="shadowCopyPlugFolder"></param>
        /// <returns></returns>
        private static FileInfo ShadowCopyFile(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));

            //确保文件存在
            if (shadowCopiedPlug.Exists)
            {
                //暂时通过文件的更新时间来判断文件的是否为最新的，是否需要拷贝更新
                //但是实际上最好的办法还是通过读取文件的Hash值来判断
                var areFilesIdentical = shadowCopiedPlug.CreationTimeUtc.Ticks >= plug.CreationTimeUtc.Ticks;
                if (areFilesIdentical)
                {
                    Debug.WriteLine("文件不需要更新; 存在相同的文件: '{0}'", shadowCopiedPlug.Name);
                    shouldCopy = false;
                }
                else
                {
                    //delete an existing file
                    Debug.WriteLine("文件需要更新; 先删除原有的老文件: '{0}'", shadowCopiedPlug.Name);
                    File.Delete(shadowCopiedPlug.FullName);
                }
            }

            if (!shouldCopy)
                return shadowCopiedPlug;

            try
            {
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            catch (IOException)
            {
                Debug.WriteLine(shadowCopiedPlug.FullName + " 被锁定, 开始执行重命名");
                //this occurs when the files are locked,
                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                try
                {
                    var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException exc)
                {
                    throw new IOException(shadowCopiedPlug.FullName + " 重命名失败, 无法初始化插件", exc);
                }
                //OK, we've made it this far, now retry the shadow copy
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }

            return shadowCopiedPlug;
        }

        /// <summary>
        /// 确定目录是 Plugs 的子目录
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static bool IsPackagePluginFolder(DirectoryInfo folder)
        {
            if (folder?.Parent == null) return false;
            if (!folder.Parent.Name.Equals(PluginsPathName, StringComparison.InvariantCultureIgnoreCase)) return false;

            return true;
        }

        #endregion

        #region 共有方法

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="applicationPartManager">Application part manager</param>
        /// <param name="config">Config</param>
        public static void Initialize(ApplicationPartManager applicationPartManager, MapleConfig config)
        {
            // *********************************************************************************************************************************
            //
            // ApplicationPartManager 用途：剥离 Controller 到其他程序集中
            //
            //MVC 框架启动的时候，首先会把 Assembly 程序集转换为 ApplicationPart 添加到 ApplicationPartManager 对象列表中，才能执行后续的任务，
            //因为要从这些程序集中查找 Controller，那么从这个特性我们可以延伸到， 利用此功能，我们可以从 Web 层剥离 Controller 到其他程序集中
            //
            // *********************************************************************************************************************************
            if (applicationPartManager == null)
                throw new ArgumentNullException(nameof(applicationPartManager));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            using (new WriteLockDisposable(Locker))
            {
                // TODO: Add verbose exception handling / raising here since this is happening on app startup and could
                // prevent app from starting altogether
                var pluginFolder = new DirectoryInfo(CommonHelper.MapPath(PluginsPath));        //获取插件集合所在目录
                _shadowCopyFolder = new DirectoryInfo(CommonHelper.MapPath(ShadowCopyPath));    //获取插件集合中Bin所在目录

                //已加载的插件集合
                var referencedPlugins = new List<PluginDescriptor>();
                //不兼容或无法加载的插件集合
                var incompatiblePlugins = new List<string>();

                try
                {
                    //从读取installedPlugins.json中读取已加载的插件清单
                    var installedPluginSystemNames = GetInstalledPluginNames(CommonHelper.MapPath(InstalledPluginsFilePath));

                    Debug.WriteLine("查询现有的插件，并将插件对应的DLL移至Bin目录中");
                    //确保 /Plugins 已经存在
                    Directory.CreateDirectory(pluginFolder.FullName);
                    //确保 /Plugins/bin 已经存在
                    Directory.CreateDirectory(_shadowCopyFolder.FullName);

                    //获取 /Plugins/bin 目录中所有的文件清单（包含子目录）
                    var binFiles = _shadowCopyFolder.GetFiles("*", SearchOption.AllDirectories);
                    if (config.ClearPluginShadowDirectoryOnStartup)
                    {
                        //否清理 /Plugins/bin 目录
                        foreach (var f in binFiles)
                        {
                            if (f.Name.Equals("placeholder.txt", StringComparison.InvariantCultureIgnoreCase))
                                continue;

                            Debug.WriteLine("移除 " + f.Name);
                            try
                            {
                                //ignore index.htm
                                var fileName = Path.GetFileName(f.FullName);
                                if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                                    continue;

                                File.Delete(f.FullName);
                            }
                            catch (Exception exc)
                            {
                                Debug.WriteLine("移除文件 " + f.Name + " 失败. 错误描述: " + exc);
                            }
                        }
                    }

                    //加载查询的描述性信息
                    foreach (var dfd in GetDescriptionFilesAndDescriptors(pluginFolder))
                    {
                        var descriptionFile = dfd.Key;      //插件描述性信息所对应的文件
                        var pluginDescriptor = dfd.Value;   //插件描述性信息实体类

                        //确保插件的版本有效
                        if (!pluginDescriptor.SupportedVersions.Contains(MapleVersion.CurrentVersion, StringComparer.InvariantCultureIgnoreCase))
                        {
                            incompatiblePlugins.Add(pluginDescriptor.SystemName);
                            continue;
                        }

                        //信息验证
                        if (string.IsNullOrWhiteSpace(pluginDescriptor.SystemName))
                            throw new Exception($"未设置插件 '{descriptionFile.FullName}' 名称. 请给插件定义唯一的名称再重新编译.");
                        if (referencedPlugins.Contains(pluginDescriptor))
                            throw new Exception($"插件 '{pluginDescriptor.SystemName}' 名称已经存在，请给插件定义唯一的名称再重新编译");

                        //判断插件是否已经被安装
                        pluginDescriptor.Installed = installedPluginSystemNames
                            .FirstOrDefault(x => x.Equals(pluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase)) != null;

                        try
                        {
                            if (descriptionFile.Directory == null)
                                throw new Exception($"Directory cannot be resolved for '{descriptionFile.Name}' description file");

                            //确保插件所需的DLL在 plugins/bin 目录中存在，如果不存在则拷贝过去
                            var pluginFiles = descriptionFile.Directory.GetFiles("*.dll", SearchOption.AllDirectories)
                                //just make sure we're not registering shadow copied plugins
                                .Where(x => !binFiles.Select(q => q.FullName).Contains(x.FullName))
                                .Where(x => IsPackagePluginFolder(x.Directory))
                                .ToList();

                            //查询插件所对应的DLL文件
                            var mainPluginFile = pluginFiles
                                .FirstOrDefault(x => x.Name.Equals(pluginDescriptor.AssemblyFileName, StringComparison.InvariantCultureIgnoreCase));

                            //如果插件所在文件不存在，则忽略
                            if (mainPluginFile == null)
                            {
                                incompatiblePlugins.Add(pluginDescriptor.SystemName);
                                continue;
                            }

                            pluginDescriptor.OriginalAssemblyFile = mainPluginFile;

                            //执行插件部署
                            pluginDescriptor.ReferencedAssembly = PerformFileDeploy(mainPluginFile, applicationPartManager, config);
                            //拷贝插件所需但尚未加载的DLL
                            foreach (var plugin in pluginFiles
                                .Where(x => !x.Name.Equals(mainPluginFile.Name, StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => !IsAlreadyLoaded(x)))
                                PerformFileDeploy(plugin, applicationPartManager, config);

                            //初始化插件类型
                            foreach (var t in pluginDescriptor.ReferencedAssembly.GetTypes())
                                if (typeof(IPlugin).IsAssignableFrom(t))
                                    if (!t.IsInterface)
                                        if (t.IsClass && !t.IsAbstract)
                                        {
                                            pluginDescriptor.PluginType = t;
                                            break;
                                        }

                            referencedPlugins.Add(pluginDescriptor);
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            //插件加载失败，抛出异常
                            var msg = $"Plugin '{pluginDescriptor.FriendlyName}'. ";
                            foreach (var e in ex.LoaderExceptions)
                                msg += e.Message + Environment.NewLine;

                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                        catch (Exception ex)
                        {
                            //插件加载失败，抛出异常
                            var msg = $"Plugin '{pluginDescriptor.FriendlyName}'. {ex.Message}";

                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var msg = string.Empty;
                    for (var e = ex; e != null; e = e.InnerException)
                        msg += e.Message + Environment.NewLine;

                    var fail = new Exception(msg, ex);
                    throw fail;
                }

                ReferencedPlugins = referencedPlugins;
                IncompatiblePlugins = incompatiblePlugins;
            }
        }
        /// <summary>
        /// 将插件更新至已安装的插件列表中
        /// </summary>
        /// <param name="systemName"></param>
        public static void MarkPluginAsInstalled(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName));

            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
            //判断文件是否存在
            if (!File.Exists(filePath))
            {
                //如果文件不存在，那么创建文件
                using (File.Create(filePath)) { }
            }
            //获取已安装的插件名称
            var installedPluginSystemNames = GetInstalledPluginNames(filePath);
            //如果插件不存在则添加之
            var alreadyMarkedAsInstalled = installedPluginSystemNames.Any(pluginName => pluginName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
            if (!alreadyMarkedAsInstalled)
                installedPluginSystemNames.Add(systemName);
            //保存已安装的插件清单
            SaveInstalledPluginNames(installedPluginSystemNames, filePath);
        }
        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="systemName"></param>
        public static void MarkPluginAsUninstalled(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName));

            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);

            //查看文件是否存在
            if (!File.Exists(filePath))
            {
                //如果不存在则创建之
                using (File.Create(filePath)) { }
            }

            //获取已安装的插件清单
            var installedPluginSystemNames = GetInstalledPluginNames(filePath);

            //移除需要卸载的插件
            var alreadyMarkedAsInstalled = installedPluginSystemNames.Any(pluginName => pluginName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
            if (alreadyMarkedAsInstalled)
                installedPluginSystemNames.Remove(systemName);

            //保存
            SaveInstalledPluginNames(installedPluginSystemNames, filePath);
        }
        /// <summary>
        /// 卸载所有插件
        /// </summary>
        public static void MarkAllPluginsAsUninstalled()
        {
            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// 查找某一类型对应的插件描述信息
        /// </summary>
        /// <param name="typeInAssembly"></param>
        /// <returns></returns>
        public static PluginDescriptor FindPlugin(Type typeInAssembly)
        {
            if (typeInAssembly == null)
                throw new ArgumentNullException(nameof(typeInAssembly));

            if (ReferencedPlugins == null)
                return null;

            return ReferencedPlugins.FirstOrDefault(plugin => plugin.ReferencedAssembly != null
                && plugin.ReferencedAssembly.FullName.Equals(typeInAssembly.Assembly.FullName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// 读取文件获得插件的描述性信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static PluginDescriptor GetPluginDescriptorFromFile(string filePath)
        {
            var text = File.ReadAllText(filePath);

            return GetPluginDescriptorFromText(text);
        }

        /// <summary>
        /// 获得插件的描述性信息
        /// </summary>
        /// <param name="text">Description text</param>
        /// <returns>Plugin descriptor</returns>
        public static PluginDescriptor GetPluginDescriptorFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new PluginDescriptor();
            var descriptor = JsonConvert.DeserializeObject<PluginDescriptor>(text);
            if (!descriptor.SupportedVersions.Any())
                descriptor.SupportedVersions.Add("1.00");
            return descriptor;
        }

        /// <summary>
        /// 将插件描述信息保存至插件对应的描述文件中
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor</param>
        public static void SavePluginDescriptor(PluginDescriptor pluginDescriptor)
        {
            if (pluginDescriptor == null)
                throw new ArgumentException(nameof(pluginDescriptor));

            //get the description file path
            if (pluginDescriptor.OriginalAssemblyFile == null)
                throw new Exception($"无法获取 {pluginDescriptor.SystemName} 插件对应的主文件（程序集）.");

            var filePath = Path.Combine(pluginDescriptor.OriginalAssemblyFile.Directory.FullName, PluginDescriptionFileName);
            if (!File.Exists(filePath))
                throw new Exception($"插件 {pluginDescriptor.SystemName} 的描述性文件不存在. {filePath}");

            //save the file
            var text = JsonConvert.SerializeObject(pluginDescriptor, Formatting.Indented);
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// 删除插件
        /// </summary>
        /// <param name="pluginDescriptor"></param>
        /// <returns></returns>
        public static bool DeletePlugin(PluginDescriptor pluginDescriptor)
        {
            if (pluginDescriptor == null)
                return false;
            //如果插件已经被安装，那么插件不允许删除
            if (pluginDescriptor.Installed)
                return false;
            if (pluginDescriptor.OriginalAssemblyFile.Directory.Exists)
                CommonHelper.DeleteDirectory(pluginDescriptor.OriginalAssemblyFile.DirectoryName);
            return true;
        }

        #endregion

        #region Properties


        /// <summary>
        /// 配置文件 “已安装的插件清单”的相对路径
        /// </summary>
        public static string InstalledPluginsFilePath => "~/App_Data/installedPlugins.json";

        /// <summary>
        /// 插件所在目录的相对路径
        /// </summary>
        public static string PluginsPath => "~/Plugins";

        /// <summary>
        /// 插件所在目录的名称
        /// </summary>
        public static string PluginsPathName => "Plugins";

        /// <summary>
        /// 插件对应的程序集所在目录的相对路径
        /// </summary>
        public static string ShadowCopyPath => "~/Plugins/bin";

        /// <summary>
        /// Gets the path to plugins refs folder
        /// </summary>
        public static string RefsPathName => "refs";

        /// <summary>
        /// 存储插件配置信息的文件名
        /// </summary>
        public static string PluginDescriptionFileName => "plugin.json";

        /// <summary>
        /// 已安装的插件集合
        /// </summary>
        public static IEnumerable<PluginDescriptor> ReferencedPlugins { get; set; }

        /// <summary>
        /// 无法安装的插件名称集合
        /// </summary>
        public static IEnumerable<string> IncompatiblePlugins { get; set; }

        #endregion
    }
}
