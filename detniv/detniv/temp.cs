using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class PrecompiledRules
    {
        ///
        /// A method used to precompile rules for a provided type
        /// 
        public static List<Func<T, bool>> CompileRule<T>(List<T> targetEntity, List<Rule> rules)
        {
            var compiledRules = new List<Func<T, bool>>();

            // Loop through the rules and compile them against the properties of the supplied shallow object 
            rules.ForEach(rule =>
            {
                var genericType = Expression.Parameter(typeof(T));
                var key = MemberExpression.Property(genericType, rule.ComparisonPredicate);
                var propertyType = typeof(T).GetProperty(rule.ComparisonPredicate).PropertyType;
                var value = Expression.Constant(Convert.ChangeType(rule.ComparisonValue, propertyType));
                var binaryExpression = Expression.MakeBinary(rule.ComparisonOperator, key, value);

                compiledRules.Add(Expression.Lambda<Func<T, bool>>(binaryExpression, genericType).Compile());
            });

            // Return the compiled rules to the caller
            return compiledRules;
        }
    }

    public class Rule
    {
        ///
        /// Denotes the rules predictate (e.g. Name); comparison operator(e.g. ExpressionType.GreaterThan); value (e.g. "Cole")
        /// 
        public string ComparisonPredicate { get; set; }
        public ExpressionType ComparisonOperator { get; set; }
        public string ComparisonValue { get; set; }

        /// 
        /// The rule method that 
        /// 
        public Rule(string comparisonPredicate, ExpressionType comparisonOperator, string comparisonValue)
        {
            ComparisonPredicate = comparisonPredicate;
            ComparisonOperator = comparisonOperator;
            ComparisonValue = comparisonValue;
        }
    }

    public class Car //: ICar
    {
        public int Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
    }

    class Program
    {
        static void Main1(string[] args)
        {
            List<Rule> rules = new List<Rule>
{
             // Create some rules using LINQ.ExpressionTypes for the comparison operators
             new Rule ( "Year", ExpressionType.GreaterThan, "2012"),
             new Rule ( "Make", ExpressionType.Equal, "El Diablo"),
             new Rule ( "Model", ExpressionType.Equal, "Torch" )
};

            var compiledMakeModelYearRules = PrecompiledRules.CompileRule(new List<Car>(), rules);

            // Create a list to house your test cars
            List<Car> cars = new List<Car>();

            // Create a car that's year and model fail the rules validations      
            Car car1_Bad = new Car
            {
                Year = 2011,
                Make = "El Diablo",
                Model = "Torche"
            };

            // Create a car that meets all the conditions of the rules validations
            Car car2_Good = new Car
            {
                Year = 2015,
                Make = "El Diablo",
                Model = "Torch"
            };

            // Add your cars to the list
            cars.Add(car1_Bad);
            cars.Add(car2_Good);

            // Iterate through your list of cars to see which ones meet the rules vs. the ones that don't
            cars.ForEach(car => {
                if (compiledMakeModelYearRules.TakeWhile(rule => rule(car)).Count() > 0)
                {
                    Console.WriteLine(string.Concat("Car model: ", car.Model, " Passed the compiled rules engine check!"));
                }
                else
                {
                    Console.WriteLine(string.Concat("Car model: ", car.Model, " Failed the compiled rules engine check!"));
                }
            });

            Console.WriteLine(string.Empty);
            Console.WriteLine("Press any key to end...");
            Console.ReadKey();

        }
    }

    //class Employee
    //{
    //    public string Name { get; set; }
    //    public string Address { get; set; }
    //    public int BadgeNumber { get; set; }
    //    public decimal salary { get; set; }
    //    public int age { get; set; }
    //}

    //class ValidationResult
    //{
    //    public bool Valid { get; private set; }
    //    public string Message { get; private set; }

    //    private ValidationResult(bool success, string message = null)
    //    {

    //    }

    //    public static ValidationResult Success()
    //    {
    //        return new ValidationResult(true);
    //    }

    //    public static ValidationResult Fail()
    //    {
    //        return new ValidationResult(true);
    //    }

    //    public ValidationResult WithMessage(string message)
    //    {
    //        return new ValidationResult(this.Valid, message);
    //    }
    //}

    //class ValidationContext
    //{
    //    //might want to make this "freezable"/"popsicle" and perhaps
    //    //you might want to validate cross-entity somehow
    //    //will you always make a new entity containing 2 or 3 sub entities for this case?
    //    List<Rule> rules = new List<Rule>();

    //    public ValidationContext(params Rule[] rules)
    //    {
    //        this.rules = rules.ToList();
    //    }

    //    //how do you know each rule applies to which entity?
    //    private List<ValidationResult> GetResults()
    //    {
    //        var results = new List<ValidationResult>();
    //        foreach (var rule in rules)
    //        {
    //            results.Add(rule.Validate(this));
    //        }

    //        return results;
    //    }

    //    public void AddRule(Rule r)
    //    {
    //        this.rules.Add(r);
    //    }

    //}

    //abstract class Rule
    //{
    //    public abstract ValidationResult Validate(ValidationContext context);

    //    public static Rule<TEntityType> Create<TEntityType>(TEntityType entity, Func<TEntityType, ValidationResult> rule)
    //    {
    //        Rule<TEntityType> newRule = new Rule<TEntityType>();
    //        newRule.rule = rule;
    //        newRule.entity = entity;
    //        return newRule;
    //    }
    //}

    //class Rule<T> : Rule
    //{
    //    internal T entity;
    //    internal Func<T, ValidationResult> rule;

    //    public override ValidationResult Validate(ValidationContext context)
    //    {
    //        return rule(entity);
    //    }
    //}

    ////usage: only rule creation since I need sleep. I have to hold an interview in 4 hours, I hope you are happy :)
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        var employee = new Employee { age = 80 };
    //        var employeeMustBe80 = Rule.Create(employee,
    //                                           e => e.age > 80 ?
    //                                           ValidationResult.Success().WithMessage("he should retire") :
    //                                           ValidationResult.Fail().WithMessage("this guy gets no pension"));

    //        employeeMustBe80.Validate();
    //    }
    //}

}
