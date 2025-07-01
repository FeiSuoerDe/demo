#pragma warning disable CS1591
#pragma warning disable CS0436

using System.Runtime.CompilerServices;
using demo.addons.GDTask.CompilerServices;

namespace demo.addons.GDTask
{
    [AsyncMethodBuilder(typeof(AsyncGdTaskVoidMethodBuilder))]
    public readonly struct GDTaskVoid
    {
        public void Forget()
        {
        }
    }
}

