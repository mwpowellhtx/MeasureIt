﻿using System.Web.Mvc;

namespace MeasureIt.AspNet.Mvc.Autofac
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
