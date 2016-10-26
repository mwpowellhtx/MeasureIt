namespace MeasureIt.Descriptors
{
    public interface ISimilarity<in T>
    {
        bool IsSimilarTo(T other);
    }
}
