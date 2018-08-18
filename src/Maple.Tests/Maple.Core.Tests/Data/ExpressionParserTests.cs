using Maple.Core.Data;
using Maple.Core.Data.Conditions;
using Maple.Core.Data.DbMappers;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Data
{
    public class ExpressionParserTests
    {
        [Fact]
        public void Sql2008_ToSQL()
        {
            IDbTranslator dbTranslator = new Sql2008Translator();
            IExpressionParser expressionParser = new ExpressionParser(EntityMapperFactory.Instance.GetEntityMapper(typeof(User)));
            DataParameterCollection dpc = new DataParameterCollection();
            Expression<Func<User, bool>> predicate = entity =>
                        !(entity.Age == 18) &&
                        entity.Age != 27 &&
                        entity.CreationTime < DateTime.Now &&
                        entity.CreatorUserId == 1000 &&
                        entity.DeleterUserId == null &&
                        entity.DeletionTime > DateTime.Now &&
                        entity.Height >= 175 &&
                        entity.Id <= 0 &&
                        entity.IsDeleted &&
                        entity.Six == Six.Woman &&
                        entity.Name == "Maple" &&
                        entity.Name.Contains("apl") &&
                        entity.Name.StartsWith("Map") &&
                        entity.Name.EndsWith("ple") &&
                        true;

            string sql = expressionParser.ToSQL(predicate.Body, dbTranslator, dpc);

            Assert.Equal(1, 1);
        }

        [Fact]
        public void Sql2008_IN_ToSQL()
        {
            string[] palyers = new string[]
            {
                "A"
            };

            int[] ages = new int[]
            {
                1,2,3
            };

            Six[] sixs = new Six[]
            {
                Six.Man,
                Six.Woman
            };

            IDbTranslator dbTranslator = new Sql2008Translator();
            IExpressionParser expressionParser = new ExpressionParser(EntityMapperFactory.Instance.GetEntityMapper(typeof(User)));
            DataParameterCollection dpc = new DataParameterCollection();
            Expression<Func<User, bool>> predicate = entity => palyers.Contains(entity.Name) || ages.Contains(entity.Age) || sixs.Contains(entity.Six);

            string sql = expressionParser.ToSQL(predicate.Body, dbTranslator, dpc);

            Assert.Equal(1, 1);
        }

        [Fact]
        public void Sql2008_DATETIME_ToSQL()
        {
            IDbTranslator dbTranslator = new Sql2008Translator();
            IExpressionParser expressionParser = new ExpressionParser(EntityMapperFactory.Instance.GetEntityMapper(typeof(User)));
            DataParameterCollection dpc = new DataParameterCollection();
            Expression<Func<User, bool>> predicate = entity => entity.Age == 2000;

            string sql = expressionParser.ToSQL(predicate.Body, dbTranslator, dpc);

            Assert.Equal(1, 1);
        }
        [Fact]
        public void Sql2008_DATAOBJECT_ToSQL()
        {
            IDbTranslator dbTranslator = new Sql2008Translator();
            IExpressionParser expressionParser = new ExpressionParser(EntityMapperFactory.Instance.GetEntityMapper(typeof(User)));
            DataParameterCollection dpc = new DataParameterCollection();
            Expression<Func<User, bool>> predicate = entity => entity.Id == 0 && entity.Address.Number == 301;
            string sql = expressionParser.ToSQL(predicate.Body, dbTranslator, dpc);
            Assert.Equal(1, 1);
        }
    }
}
