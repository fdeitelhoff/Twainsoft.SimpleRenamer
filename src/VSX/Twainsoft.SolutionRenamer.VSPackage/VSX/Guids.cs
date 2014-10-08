using System;

namespace Twainsoft.SolutionRenamer.VSPackage.VSX
{
    static class GuidList
    {
        public const string SolutionRenamerVsPackagePkgString = "140b0282-2aaa-4427-a545-4fd4452656af";
        private const string SolutionRenamerVsPackageCmdSetString = "3ea51fcb-84e7-4277-98f1-609cb500857e";
        //public const string guidToolWindowPersistanceString = "329dddf7-8d30-46e9-997b-d9863990df74";

        public static readonly Guid SolutionRenamerVsPackageCmdSet = new Guid(SolutionRenamerVsPackageCmdSetString);
    };
}