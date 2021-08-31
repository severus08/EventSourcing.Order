using System;

namespace Order.Infrastructure.Enumaration
{
    public abstract class Enumaration
    {
        public int Id { get; set; }
        public string Name { get; set; }

        protected Enumaration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Enumaration()
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == default(object))
            {
                return false;
            }

            return GetType() == obj.GetType() && Id.Equals(((Enumaration) obj).Id);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class EnumExtensions
    {
        public static bool Compare(Enumaration a, Enumaration b)
        {
            if (a == default || b == default)
            {
                return false;
            }

            return a.Id == b.Id;
        }
    }
}