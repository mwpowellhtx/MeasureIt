using System;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    internal interface IDefaultMoniker : IMoniker
    {
        Guid Id { get; }
    }
}
