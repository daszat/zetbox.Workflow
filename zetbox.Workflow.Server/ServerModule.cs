
namespace zetbox.Workflow.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Autofac;
    using Zetbox.API;
    using Zetbox.API.Server;

    public class ServerModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            base.Load(moduleBuilder);

            moduleBuilder.RegisterZetboxImplementors(typeof(ServerModule).Assembly);

            // Register explicit overrides here
            moduleBuilder
                .RegisterType<SchedulerService>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
