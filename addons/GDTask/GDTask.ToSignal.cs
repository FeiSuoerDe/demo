using System.Threading;
using Godot;

namespace demo.addons.GDTask
{
    public partial struct GDTask
    {
        public static async GdTask<Variant[]> ToSignal(GodotObject self, StringName signal)
        {
            return await self.ToSignal(self, signal);
        }

        public static async GdTask<Variant[]?> ToSignal(GodotObject self, StringName signal, CancellationToken ct)
        {
            var tcs = new GdTaskCompletionSource<Variant[]?>();
            ct.Register(() => tcs.TrySetCanceled(ct));
            Create(async () =>
            {
                var result = await self.ToSignal(self, signal);
                tcs.TrySetResult(result);
            }).Forget();
            return await tcs.Task;
        }
    }
}
