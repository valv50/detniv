using ShipmentDiscount.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentDiscount.Helper
{
    public class Common
    {
        public static List<Func<T, decimal>> CompileRule<T>(/*List<T> targetEntity,*/ List<Rule> rules)
        {
            var compiledRules = new List<Func<T, decimal>>();

            // Loop through the rules and compile them against the properties of the supplied shallow object 
            rules.ForEach(rule =>
            {
            //    var genericType = Expression.Parameter(typeof(T));
            //    var key = MemberExpression.Property(genericType, rule.ComparisonPredicate);
            //    var propertyType = typeof(T).GetProperty(rule.ComparisonPredicate).PropertyType;
            //    var value = Expression.Constant(Convert.ChangeType(rule.ComparisonValue, propertyType));
            //    var binaryExpression = Expression.MakeBinary(rule.ComparisonOperator, key, value);

            //    compiledRules.Add(Expression.Lambda<Func<T, decimal>>(binaryExpression, genericType).Compile());
            });

            // Return the compiled rules to the caller
            return compiledRules;
        }

        public static void SetPrice(ShippingContext shippingContext, List<Rule> rule)
        {
            shippingContext.set
        }
    }
}
