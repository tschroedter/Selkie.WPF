﻿using System.Diagnostics.CodeAnalysis;
using Selkie.Windsor;

namespace Selkie.WPF.Models.Interfaces
{
    //ncrunch: no coverage start 
    [ExcludeFromCodeCoverage]
    public class Installer : BaseInstaller <Installer>
    {
        public override string GetPrefixOfDllsToInstall()
        {
            return "Selkie.";
        }
    }
}