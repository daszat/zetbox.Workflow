﻿

namespace zetbox.Workflow.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Autofac;
    using NUnit.Framework;
    using Zetbox.API;

    [SetUpFixture]
    public class SetUpFixture : Zetbox.API.AbstractConsumerTests.AbstractSetUpFixture
    {
        protected override void SetupBuilder(ContainerBuilder builder)
        {
            base.SetupBuilder(builder);

            // register local overrides here
        }

        protected override string GetConfigFile()
        {
            return "Zetbox.Workflow.Client.Tests.xml";
        }

        protected override HostType GetHostType()
        {
            return HostType.Client;
        }
    }
}
