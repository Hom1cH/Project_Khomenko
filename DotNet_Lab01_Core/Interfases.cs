namespace DotNet_Lab01_Core;

public interface IShowable
{
    void ShowInfo();
}

public interface IProgressable
{
    int Progress { get; }
    void UpdateProgress(int value);

}

public interface IExecutable
{
    void Start();
    void Complete();
}

public interface ICompute
{
    double ComputeWorkload();
}