using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Maple.Core.Infrastructure
{
    /// <summary>
    /// 接口：可从程序集中发现各种类型的工具
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        /// 查找类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="onlyConcreteClasses">是否仅持续具体的实现类，不查询抽象类</param>
        /// <returns>结果</returns>
        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

        /// <summary>
        /// 查找类型
        /// </summary>
        /// <param name="assignTypeFrom">指定的类型</param>
        /// <param name="onlyConcreteClasses">是否仅持续具体的实现类，不查询抽象类</param>
        /// <returns>结果</returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        /// <summary>
        /// 查找类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="assemblies">程序集集合</param>
        /// <param name="onlyConcreteClasses">是否仅持续具体的实现类，不查询抽象类</param>
        /// <returns>结果</returns>
        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// 查找类型
        /// </summary>
        /// <param name="assignTypeFrom">指定的类型</param>
        /// <param name="assemblies">程序集集合</param>
        /// <param name="onlyConcreteClasses">是否仅持续具体的实现类，不查询抽象类</param>
        /// <returns>结果</returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// 获取所有相关的程序集
        /// </summary>
        /// <returns>程序集集合</returns>
        IList<Assembly> GetAssemblies();

    }
}
