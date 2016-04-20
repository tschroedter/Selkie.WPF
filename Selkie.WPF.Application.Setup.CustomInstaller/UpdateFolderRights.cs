using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Selkie.WPF.Application.Setup.CustomInstaller
{
    [ExcludeFromCodeCoverage]
    [RunInstaller(true)]
    public partial class UpdateFolderRights : Installer
    {
        public UpdateFolderRights()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            SetPermission();

            base.Install(stateSaver);
        }

        public void SetPermission()
        {
            string path = Context.Parameters [ "assemblypath" ];
            string myAssembly = Path.GetFullPath(path);
            string directoryName = Path.GetDirectoryName(myAssembly);

            if ( directoryName == null )
            {
                throw new NullReferenceException("Could not get directory name for path '" + path + "'!");
            }

            string logPath = Path.Combine(directoryName,
                                          "Logs");
            Directory.CreateDirectory(logPath);
            ReplacePermissions(logPath,
                               WellKnownSidType.AuthenticatedUserSid,
                               FileSystemRights.FullControl);
        }

        private static void ReplacePermissions(string filepath,
                                               WellKnownSidType sidType,
                                               FileSystemRights allow)
        {
            FileSecurity sec = File.GetAccessControl(filepath);
            var sid = new SecurityIdentifier(sidType,
                                             null);
            sec.PurgeAccessRules(sid); //remove existing
            sec.AddAccessRule(new FileSystemAccessRule(sid,
                                                       allow,
                                                       AccessControlType.Allow));
            File.SetAccessControl(filepath,
                                  sec);
        }
    }
}