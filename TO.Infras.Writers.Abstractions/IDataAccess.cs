
using GodotTask;

namespace Infras.Writers.Abstractions
{
    /// <summary>
    /// 基础数据访问接口(Godot专用)
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        /// 从存储加载数据到内存(使用Godot的GDTask)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dataKey">数据键</param>
        /// <param name="target"> </param>
        /// <returns>表示加载操作的GDTask</returns>
        GDTask LoadAsync<T>( T target) where T : class?;
        
        /// <summary>
        /// 将内存数据保存到存储(使用Godot的GDTask)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dataKey">数据键</param>

        /// <returns>表示保存操作的GDTask</returns>
        GDTask<T?> SaveAsync<T>() where T : class;
        
        /// <summary>
        /// 检查数据是否存在
        /// </summary>
        /// <param name="dataKey">数据键</param>
        /// <returns>是否存在</returns>
        bool Exists(string dataKey);
        
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="dataKey">数据键</param>
        void Delete(string dataKey);
    }
}
