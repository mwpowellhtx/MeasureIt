//using System;
//using System.Diagnostics;

//namespace MeasureIt
//{
//    using Xunit;
//    using AverageTimeAdapter = AverageTimePerformanceCounterAdapter;
//    using ICategoryAttribute = IPerformanceCounterCategoryAttribute;

//    internal static class DefaultTestExtensionMethods
//    {
//        private static void VerifyDefaultCategoryAttribute(IPerformanceCounterCategoryAttribute attr)
//        {
//            // TODO: TBD: necessary any longer?
//            //Assert.Equal(PerformanceCounterCategoryType.MultiInstance, attr.CategoryType);
//            //Assert.Null(attr.Help);
//            //attr.Name.VerifyGuid();
//        }

//        internal static void VerifyDefault(this ICategoryAttribute attr, Action<ICategoryAttribute> verify = null)
//        {
//            verify = verify ?? VerifyDefaultCategoryAttribute;
//            Assert.NotNull(attr);
//            verify(attr);
//        }
//    }
//}
