using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Maple.Web.Framework.Mvc.ModelBinding
{
    /// <summary>
    /// 通过多语言库显示字段对应的中文名称
    /// </summary>
    public class MapleResourceDisplayNameAttribute : DisplayNameAttribute, IModelAttribute
    {
        #region Fields

        private string _resourceValue = string.Empty;

        #endregion

        #region Ctor

        /// <summary>
        /// 通过多语言库显示字段对应的中文名称
        /// </summary>
        /// <param name="resourceKey">Key of the locale resource</param>
        public MapleResourceDisplayNameAttribute(string resourceKey) : base(resourceKey)
        {
            ResourceKey = resourceKey;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets key of the locale resource 
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Getss the display name
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return ResourceKey;


                ////get working language identifier
                //var workingLanguageId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;

                ////get locale resource value
                //_resourceValue = EngineContext.Current.Resolve<ILocalizationService>().GetResource(ResourceKey, workingLanguageId, true, ResourceKey);

                //return _resourceValue;
            }
        }

        /// <summary>
        /// Gets name of the attribute
        /// </summary>
        public string Name
        {
            get { return nameof(MapleResourceDisplayNameAttribute); }
        }

        #endregion
    }
}
