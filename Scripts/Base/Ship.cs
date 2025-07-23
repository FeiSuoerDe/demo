using System;

namespace Demo.Scripts.Base
{
    /// <summary>
    /// 船只的抽象基类，定义了所有船只共有的属性和行为
    /// </summary>
    public abstract class Ship
    {
        /// <summary>
        /// 船只的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 船只的航行速度
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// 船只的当前生命值
        /// </summary>
        public float Health { get; set; }

        /// <summary>
        /// 初始化船只的构造函数
        /// </summary>
        /// <param name="name">船只名称</param>
        /// <param name="speed">航行速度</param>
        /// <param name="health">初始生命值</param>
        public Ship(string name, float speed, float health)
        {
            Name = name;
            Speed = speed;
            Health = health;
        }

        /// <summary>
        /// 使船只移动的虚方法，可被子类重写
        /// </summary>
        public virtual void Move()
        {
            Console.WriteLine($"{Name} is moving at speed {Speed}.");
        }

        /// <summary>
        /// 处理船只受到伤害的虚方法
        /// </summary>
        /// <param name="damage">受到的伤害值</param>
        public virtual void TakeDamage(float damage)
        {
            Health -= damage;
            Console.WriteLine($"{Name} took {damage} damage. Remaining health: {Health}");
            if (Health <= 0)
            {
                Destroy();
            }
        }

        /// <summary>
        /// 处理船只被摧毁的虚方法
        /// </summary>
        public virtual void Destroy()
        {
            Console.WriteLine($"{Name} has been destroyed.");
        }
    }
}