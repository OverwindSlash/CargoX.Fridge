using System;
using System.Collections.Generic;
using System.Text;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace CargoX.Fridge.S3
{
    [DependsOn(
        typeof(FridgeCoreModule))]
    public class FridgeS3Module : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FridgeS3Module).GetAssembly());
        }
    }
}
