using System;
using System.Collections.Generic;
using System.Reflection;

namespace Infrastructure.Data.Utilities
{
#pragma warning disable 1570
    /// <summary>
    /// Allows the comparison of objects based on a specific object's field.
    /// 
    /// Although can be used for other purposes, this was written to compare objects in 
    /// an IEnumerable object to find distinct objects.
    /// 
    /// Usage:  Distinct(new PropertyComparer<ObjectType>("ObjectProperty"))
    /// Example:  
    ///     var tools = (from row in data.AsEnumerable()
    ///         where facility.Equals(row.Field<string>("FACILITY")) && mod2.Id.Equals(row.Field<string>("MODULE")) && ts1.Id.Equals(row.Field<string>("TOOLSET"))
    ///         select new Tool
    ///         {
    ///             Id = row.Field<string>("TOOL"),
    ///             Name = row.Field<string>("TOOL_DISPLAY"),
    ///             FacilityId = row.Field<string>("FACILITY"),
    ///             ModuleId = row.Field<string>("MODULE"),
    ///             EquipmentFamilyId = row.Field<string>("TOOLSET")
    ///         }).Distinct(new PropertyComparer<Tool>("Id")).OrderBy(o => o.Name, new AlphanumComparator()).ToList();
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
#pragma warning restore 1570
    public class PropertyComparator<TEntity> : IEqualityComparer<TEntity>
    {

        private readonly PropertyInfo _propertyInfo;

        /// <summary>
        /// Creates a new instance of PropertyComparer.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property on type TEntity to perform the comparison on.
        /// </param>
        public PropertyComparator(string propertyName)
        {
            //store a reference to the property info object for use during the comparison
            _propertyInfo = typeof(TEntity).GetProperty(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            
            if (_propertyInfo == null)
            {
                throw new ArgumentException(string.Format("{0} is not a property of type {1}.", propertyName, typeof(TEntity)));
            }

        }

        #region IEqualityComparer<T> Members

        public bool Equals(TEntity x, TEntity y)
        {
            //get the current value of the comparison property of x and of y
            object xValue = _propertyInfo.GetValue(x, null);
            object yValue = _propertyInfo.GetValue(y, null);

            //if the xValue is null then we consider them equal if and only if yValue is null
            if (xValue == null)
            {
                return yValue == null;
            }

            //use the default comparer for whatever type the comparison property is.
            return xValue.Equals(yValue);
        }

        public int GetHashCode(TEntity obj)
        {
            //get the value of the comparison property out of obj
            object propertyValue = _propertyInfo.GetValue(obj, null);

            if (propertyValue == null)
            {
                return 0;
            }
            
            return propertyValue.GetHashCode();

        }

        #endregion

    }
}
