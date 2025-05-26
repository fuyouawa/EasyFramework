using Cysharp.Threading.Tasks;
using EasyFramework.Core;

public class TestApp : Architecture<TestApp>
{
    protected override UniTask OnInitAsync()
    {
        return UniTask.CompletedTask;
    }
}
