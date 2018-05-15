using Maple.Web.Framework.Mvc.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Maple.Core;

namespace Maple.Web.Framework.Mvc.ModelBinding
{
    /// <summary>
    /// 针对Action中BaseMapleModel类型的参数自定义IModelBinderProvider
    /// </summary>
    public class MapleModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Creates a Maple model binder based on passed context
        /// </summary>
        /// <param name="context">Model binder provider context</param>
        /// <returns>Model binder</returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            Check.NotNull(context, nameof(context));

            var modelType = context.Metadata.ModelType;
            if (!typeof(BaseMapleModel).IsAssignableFrom(modelType))
                return null;

            //use MapleModelBinder as a ComplexTypeModelBinder for BaseMapleModel
            if (context.Metadata.IsComplexType && !context.Metadata.IsCollectionType)
            {
                //create binders for all model properties
                var propertyBinders = context.Metadata.Properties
                    .ToDictionary(modelProperty => modelProperty, modelProperty => context.CreateBinder(modelProperty));

                return new MapleModelBinder(propertyBinders);
            }

            //or return null to further search for a suitable binder
            return null;
        }
    }
}
