public interface IPoolable
{
	string PoolID { get; }
	int ObjectsCount { get; }

	bool IsActive { get; }
}
