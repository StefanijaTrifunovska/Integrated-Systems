﻿
namespace WebApplication1.Web.Data.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;

    [GeneratedCode("EntityFramework.Migrations", "6.2.0-61023")]
    public sealed partial class InitialSetup : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(InitialSetup));

        string IMigrationMetadata.Id
        {
            get { return "201806290459082_InitialSetup"; }
        }

        string IMigrationMetadata.Source
        {
            get { return null; }
        }

        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}