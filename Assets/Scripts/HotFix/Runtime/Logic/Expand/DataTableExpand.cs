using cfg;
using Framework;
using Luban;

namespace HotFix
{
    public static class DataTableExpand
    {
        public static T1 GetTableData<T, T1>(this TableManager tableManager, int id)
            where T : class, ITable 
            where T1 : BeanBase  
        {
            T table = tableManager.GetTable<T>();
            if (table == null)
                return null;
            var data = table.GetBeanBase(id);
            if(data == null)
                return null;
            return (T1)table.GetBeanBase(id);
        }
    }
}