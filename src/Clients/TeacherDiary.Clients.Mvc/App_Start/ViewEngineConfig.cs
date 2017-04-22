﻿using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace TeacherDiary.Clients.Mvc
{
    [ExcludeFromCodeCoverage]
    public class ViewEngineConfig
    {
        public static void RegisterViewEngine()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }
    }
}