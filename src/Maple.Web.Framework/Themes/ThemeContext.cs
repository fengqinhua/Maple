﻿using System;
using System.Linq;

namespace Maple.Web.Framework.Themes
{
    public partial class ThemeContext : IThemeContext
    {
        private string _workingThemeName = "red";
        public string WorkingThemeName { get => _workingThemeName; set => _workingThemeName = value; }
    }
    ///// <summary>
    ///// 主题的上下文的实现类
    ///// </summary>
    //public partial class ThemeContext : IThemeContext
    //{
    //    private readonly IGenericAttributeService _genericAttributeService;
    //    private readonly IStoreContext _storeContext;
    //    private readonly IThemeProvider _themeProvider;
    //    private readonly IWorkContext _workContext;
    //    private readonly StoreInformationSettings _storeInformationSettings;
    //    private string _cachedThemeName;

    //    #region 构造函数

    //    /// <summary>
    //    /// Ctor
    //    /// </summary>
    //    /// <param name="genericAttributeService">Generic attribute service</param>
    //    /// <param name="storeContext">Store context</param>
    //    /// <param name="themeProvider">Theme provider</param>
    //    /// <param name="workContext">Work context</param>
    //    /// <param name="storeInformationSettings">Store information settings</param>
    //    public ThemeContext(IGenericAttributeService genericAttributeService,
    //        IStoreContext storeContext,
    //        IThemeProvider themeProvider,
    //        IWorkContext workContext,
    //        StoreInformationSettings storeInformationSettings)
    //    {
    //        this._genericAttributeService = genericAttributeService;
    //        this._storeContext = storeContext;
    //        this._themeProvider = themeProvider;
    //        this._workContext = workContext;
    //        this._storeInformationSettings = storeInformationSettings;
    //    }

    //    #endregion

    //    #region 属性字段

    //    /// <summary>
    //    /// Get or set current theme system name
    //    /// </summary>
    //    public string WorkingThemeName
    //    {
    //        get
    //        {
    //            if (!string.IsNullOrEmpty(_cachedThemeName))
    //                return _cachedThemeName;

    //            var themeName = string.Empty;

    //            //whether customers are allowed to select a theme
    //            if (_storeInformationSettings.AllowCustomerToSelectTheme && _workContext.CurrentCustomer != null)
    //                themeName = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.WorkingThemeName, _genericAttributeService, _storeContext.CurrentStore.Id);

    //            //if not, try to get default store theme
    //            if (string.IsNullOrEmpty(themeName))
    //                themeName = _storeInformationSettings.DefaultStoreTheme;

    //            //ensure that this theme exists
    //            if (!_themeProvider.ThemeExists(themeName))
    //            {
    //                //if it does not exist, try to get the first one
    //                themeName = _themeProvider.GetThemes().FirstOrDefault()?.SystemName 
    //                    ?? throw new Exception("No theme could be loaded");
    //            }

    //            //cache theme system name
    //            this._cachedThemeName = themeName;

    //            return themeName;
    //        }
    //        set
    //        {
    //            //whether customers are allowed to select a theme
    //            if (!_storeInformationSettings.AllowCustomerToSelectTheme || _workContext.CurrentCustomer == null)
    //                return;

    //            //save selected by customer theme system name
    //            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, 
    //                SystemCustomerAttributeNames.WorkingThemeName, value, _storeContext.CurrentStore.Id);

    //            //clear cache
    //            this._cachedThemeName = null;
    //        }
    //    }

    //    #endregion
    //}
}
