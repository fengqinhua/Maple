using Maple.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maple.Web.Framework.Mvc.ModelBinding
{
    /// <summary>
    /// 
    /// </summary>
    public class MapleMetadataProvider : IDisplayMetadataProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Display metadata provider context</param>
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            //获取所有自定义属性
            var additionalValues = context.Attributes.OfType<IModelAttribute>().ToList();

            //尝试将它们添加为元数据的附加值
            foreach (var additionalValue in additionalValues)
            {
                if (context.DisplayMetadata.AdditionalValues.ContainsKey(additionalValue.Name))
                    throw new MapleException("There is already an attribute with the name '{0}' on this model", additionalValue.Name);

                context.DisplayMetadata.AdditionalValues.Add(additionalValue.Name, additionalValue);
            }
        }
    }
}
