namespace MeasureIt.Collections.Generic
{
    internal delegate void BeforeOperationDelegate<in T>(T item);

    internal delegate void AfterOperationDelegate<in T>(T item);
}
