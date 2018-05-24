using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DbMappers;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.Internal
{
    /// <summary>
    /// 数据库访问程序集私有实现类
    /// </summary>
    internal class InternalDataOperator<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        private string insertSQL = "";
        private string updateSQL = "";
        private string deleteSQL = "";
        private readonly IDbTranslator dbTranslator;

        public IEntityMapper EntityInfo { get; protected set; }

        public InternalDataOperator()
        {
            this.EntityInfo = EntityMapperFactory.Instance.GetEntityMapper<TEntity, TPrimaryKey>();
        }

        public bool Create(IDataProvider dataContext, IEntity<TPrimaryKey> entity)
        {
            if (entity == null)
                return false;
            //插件插入的SQL语句
            if (insertSQL.IsNullOrEmpty())
                insertSQL = buildInsertSQL();



            //if (this.EntityInfo.Entity.KeyScheme == KeyScheme.SerialNo && this.EntityInfo.KeyMembers.Count == 1)
            //{
            //    //生成流水号ID
            //    MemberHandler mh = this.EntityInfo.KeyMembers[0];
            //    string key = "";
            //    if (string.IsNullOrEmpty(this.EntityInfo.Entity.KeyPrefix))
            //        key = "999";
            //    else
            //        key = this.EntityInfo.Entity.KeyPrefix;
            //    string id = dataContext.NewSerialNo(key);
            //    mh.SetValue(model, id);
            //}

            //SqlStatement sqlStatement = m_InsertBuilder.ToSqlStatement(this.EntityInfo, this.m_Driver.Translator, model);
            //int result = dataContext.ExecuteNonQuery(sqlStatement);

            ////处理返回值
            //this.PopulateOutParams(model, sqlStatement);
            //return result > 0;
            return false;
        }


        /// <summary>
        /// 生成Insert SQL语句
        /// </summary>
        /// <returns></returns>
        private string buildInsertSQL()
        {
            if (this.EntityInfo.PKeyProperties.Count == 0)
                throw new Exception("当前实体中未设置主键");

            StringBuilder sBuilderFiled = new StringBuilder();
            StringBuilder sBuilderValue = new StringBuilder();

            foreach (var propertyMapper in this.EntityInfo.PKeyProperties)
            {
                sBuilderFiled.Append(dbTranslator.Quote(propertyMapper.ColumnName));
                sBuilderFiled.Append(",");

                sBuilderValue.Append(dbTranslator.QuoteParameter(propertyMapper.ColumnName));
                sBuilderValue.Append(",");
            }

            foreach (var propertyMapper in this.EntityInfo.OtherProperties)
            {
                sBuilderFiled.Append(dbTranslator.Quote(propertyMapper.ColumnName));
                sBuilderFiled.Append(",");

                sBuilderValue.Append(dbTranslator.QuoteParameter(propertyMapper.ColumnName));
                sBuilderValue.Append(",");
            }

            return string.Format("INSERT INTO {0} ({1}) VALUES ({2}) {3};\n",
                dbTranslator.Quote(this.EntityInfo.TableName),
                sBuilderFiled.ToString().TrimEnd(','),
                sBuilderValue.ToString().TrimEnd(','));
        }

        //private SqlStatement createInsertSqlStatement(IEntityMapper entityMapper, object entity)
        //{
        //    var dpc = new DataParameterCollection();

        //    foreach (var propertyMapper in entityMapper.PKeyProperties)
        //    {
        //        dpc.Add(this.getDataParameter(propertyMapper, propertyMapper.FastGetValue(entity)));
        //    }

        //    foreach (var propertyMapper in entityMapper.OtherProperties)
        //    {
        //        dpc.Add(this.getDataParameter(propertyMapper, propertyMapper.FastGetValue(entity)));
        //    }

        //    return new SqlStatement(System.Data.CommandType.Text, this.m_Sql, dpc);
        //}


        //protected DataParameter getDataParameter(IPropertyMapper propertyMapper, object value)
        //{
        //    return getDataParameter(propertyMapper, value, System.Data.ParameterDirection.Input);
        //}
        //protected DataParameter getDataParameter(IPropertyMapper propertyMapper, object value, System.Data.ParameterDirection pd)
        //{
        //    return new DataParameter(propertyMapper.ColumnName, value, propertyMapper.PropertyInfo.PropertyType, propertyMapper.DbType, propertyMapper.Size, pd);
        //}
    }
}
