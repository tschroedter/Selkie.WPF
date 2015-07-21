using System;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Main
{
    public class Bootstrapper
    {
        private static volatile IWindsorContainer s_TheWindsorContainer;
        private static readonly object s_SyncRoot = new Object();

        public static IWindsorContainer Container
        {
            get
            {
                if (s_TheWindsorContainer == null)
                {
                    lock (s_SyncRoot)
                    {
                        if (s_TheWindsorContainer == null)
                        {
                            s_TheWindsorContainer = new WindsorContainer().Install(FromAssembly.This());
                        }
                    }
                }

                return s_TheWindsorContainer;
            }
        }
    }
}
