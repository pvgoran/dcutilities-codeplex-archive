using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace DigitallyCreated.Utilities.Bcl.ExpressionTrees
{
	/// <summary>
	/// Tests around the <see cref="ExpressionTreeRewriter"/>
	/// </summary>
	[TestClass]
	public class ExpressionTreeRewriterTests
	{
		/// <summary>
		/// Tests the <see cref="EntityNullTestRewriter"/> with an entity with an int primary key
		/// </summary>
		[TestMethod]
		public void TestEntityNullRewritingWithIntKey()
		{
			IList<IntEntity> entities = new List<IntEntity>
			{
				new IntEntity { ID = 1 },
				new IntEntity { ID = 2 },
			};

			IQueryable<IntEntity> queryable = entities.AsQueryable()
				.Where(e => RewriterMarkers.EntityNullTest(e.ID))
				.Rewrite();
			List<IntEntity> results = queryable.ToList();

			dynamic expression = queryable.Expression;
			IQueryable<IntEntity> whereQuery = entities.AsQueryable().Where(e => e.ID == 1);

			Assert.IsInstanceOfType(expression, typeof(MethodCallExpression));
			Assert.AreEqual(((MethodCallExpression)whereQuery.Expression).Method, expression.Method);
			Assert.IsInstanceOfType(expression.Arguments[0], typeof(ConstantExpression));
			Assert.AreEqual(typeof(EnumerableQuery<IntEntity>), expression.Arguments[0].Type);
			Assert.IsInstanceOfType(expression.Arguments[1], typeof(UnaryExpression));
			Assert.AreEqual(ExpressionType.Quote, expression.Arguments[1].NodeType);
			Assert.IsInstanceOfType(expression.Arguments[1].Operand, typeof(LambdaExpression));
			Assert.IsInstanceOfType(expression.Arguments[1].Operand.Parameters[0], typeof(ParameterExpression));
			Assert.AreEqual(typeof(IntEntity), expression.Arguments[1].Operand.Parameters[0].Type);
			Assert.IsInstanceOfType(expression.Arguments[1].Operand.Body, typeof(BinaryExpression));
			Assert.AreEqual(ExpressionType.NotEqual, expression.Arguments[1].Operand.Body.NodeType);
			Assert.IsInstanceOfType(expression.Arguments[1].Operand.Body.Left, typeof(UnaryExpression));
			Assert.AreEqual(ExpressionType.Convert, expression.Arguments[1].Operand.Body.Left.NodeType);
			Assert.AreEqual(typeof(int?), expression.Arguments[1].Operand.Body.Left.Type);
			Assert.IsInstanceOfType(expression.Arguments[1].Operand.Body.Left.Operand, typeof(MemberExpression));
			Assert.AreEqual(typeof(IntEntity).GetProperty("ID"), expression.Arguments[1].Operand.Body.Left.Operand.Member);
			Assert.IsInstanceOfType(expression.Arguments[1].Operand.Body.Right, typeof(ConstantExpression));
			Assert.IsNull(expression.Arguments[1].Operand.Body.Right.Value);

			Assert.AreEqual(2, results.Count);
			Assert.AreSame(entities[0], results[0]);
			Assert.AreSame(entities[1], results[1]);
		}


		/// <summary>
		/// Tests the <see cref="EntityNullTestRewriter"/> with an entity with an string primary key
		/// </summary>
		[TestMethod]
		public void TestEntityNullRewritingWithStringKey()
		{
			IList<StringEntity> entities = new List<StringEntity>
			{
				new StringEntity { ID = "A" },
				new StringEntity { ID = "B" },
				new StringEntity { ID = null },
			};

			IQueryable<StringEntity> queryable = entities.AsQueryable()
				.Where(e => RewriterMarkers.EntityNullTest(e.ID))
				.Rewrite();
			List<StringEntity> results = queryable.ToList();

			dynamic expression = queryable.Expression;
			IQueryable<StringEntity> whereQuery = entities.AsQueryable().Where(e => e.ID == "A");

			Assert.IsInstanceOfType(expression, typeof(MethodCallExpression));
			Assert.AreEqual(((MethodCallExpression)whereQuery.Expression).Method, expression.Method);
			Assert.IsInstanceOfType(expression.Arguments[0], typeof(ConstantExpression));
			Assert.AreEqual(typeof(EnumerableQuery<StringEntity>), expression.Arguments[0].Type);
			Assert.IsInstanceOfType(expression.Arguments[1], typeof(UnaryExpression));
			Assert.AreEqual(ExpressionType.Quote, expression.Arguments[1].NodeType);
			Assert.IsInstanceOfType(expression.Arguments[1].Operand, typeof(LambdaExpression));
			Assert.IsInstanceOfType(expression.Arguments[1].Operand.Parameters[0], typeof(ParameterExpression));
			Assert.AreEqual(typeof(StringEntity), expression.Arguments[1].Operand.Parameters[0].Type);
			Assert.IsInstanceOfType(expression.Arguments[1].Operand.Body, typeof(BinaryExpression));
			Assert.AreEqual(ExpressionType.NotEqual, expression.Arguments[1].Operand.Body.NodeType);
			Assert.IsInstanceOfType(expression.Arguments[1].Operand.Body.Left, typeof(UnaryExpression));
			Assert.AreEqual(ExpressionType.Convert, expression.Arguments[1].Operand.Body.Left.NodeType);
			Assert.AreEqual(typeof(string), expression.Arguments[1].Operand.Body.Left.Type);
			Assert.IsInstanceOfType(expression.Arguments[1].Operand.Body.Left.Operand, typeof(MemberExpression));
			Assert.AreEqual(typeof(StringEntity).GetProperty("ID"), expression.Arguments[1].Operand.Body.Left.Operand.Member);
			Assert.IsInstanceOfType(expression.Arguments[1].Operand.Body.Right, typeof(ConstantExpression));
			Assert.IsNull(expression.Arguments[1].Operand.Body.Right.Value);

			Assert.AreEqual(2, results.Count);
			Assert.AreSame(entities[0], results[0]);
			Assert.AreSame(entities[1], results[1]);
		}


		/// <summary>
		/// Tests the <see cref="ProjectionRewriter{TFrom,TTo}"/>
		/// </summary>
		[TestMethod]
		public void TestProjectionRewriter()
		{
			IList<PersonEntity> people = new List<PersonEntity>
			{
				new PersonEntity { ID = 1, FirstName = "Daniel", LastName = "Chambers" },
				new PersonEntity { ID = 2, FirstName = "Colin", LastName = "Savage" },
			};

			IQueryable<PersonReport> queryable = people.AsQueryable()
				.Select(p => ToPersonReport(p))
				.Rewrite();

			List<PersonReport> results = queryable.ToList();

			Assert.AreEqual(2, results.Count);
			Assert.AreEqual(1, results[0].ID);
			Assert.AreEqual(2, results[1].ID);
			Assert.AreEqual("Daniel", results[0].FirstName);
			Assert.AreEqual("Colin", results[1].FirstName);
		}

		/// <summary>
		/// Tests using the lambda property rewriter
		/// </summary>
		[TestMethod]
		public void TestRewriteUsingLambdaPropertyRewriter()
		{
			IList<PersonEntity> people = new List<PersonEntity>
			{
				new PersonEntity { ID = 1, FirstName = "Daniel", LastName = "Chambers" },
				new PersonEntity { ID = 2, FirstName = "Colin", LastName = "Savage" },
			};

			IQueryable<PersonReport> queryable = people.AsQueryable()
				.Select(p => ToPersonReport(p))
				.Rewrite();

			List<PersonReport> results = queryable.ToList();

			Assert.AreEqual(2, results.Count);
			Assert.AreEqual(1, results[0].ID);
			Assert.AreEqual(2, results[1].ID);
			Assert.AreEqual("Daniel", results[0].FirstName);
			Assert.AreEqual("Colin", results[1].FirstName);
		}


		/// <summary>
		/// Tests an expected failure when the marker method takes a subclass while the lambda takes the base class
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestFailUsingLambdaPropertyRewriterUsingSubclassesIn()
		{
			IList<PersonEntitySubclass> people = new List<PersonEntitySubclass>
			{
				new PersonEntitySubclass { ID = 1, FirstName = "Daniel", LastName = "Chambers" },
				new PersonEntitySubclass { ID = 2, FirstName = "Colin", LastName = "Savage" },
			};

			IQueryable<PersonReport> queryable = people.AsQueryable()
				.Select(p => ToPersonReportSubclassIn(p))
				.Rewrite();
		}


		/// <summary>
		/// Tests an expected failure when the marker method returns a base class while the lambda returns a
		/// subclass
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestFailUsingLambdaPropertyRewriterUsingSubclassesOut()
		{
			IList<PersonEntitySubclass> people = new List<PersonEntitySubclass>
			{
				new PersonEntitySubclass { ID = 1, FirstName = "Daniel", LastName = "Chambers" },
				new PersonEntitySubclass { ID = 2, FirstName = "Colin", LastName = "Savage" },
			};

			IQueryable<PersonReport> queryable = people.AsQueryable()
				.Select(p => ToPersonReportSubclassOut(p))
				.Rewrite();
		}


		/// <summary>
		/// Tests an expected failure when the lambda property is not a public property and is instead internal
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestFailUsingLambdaPropertyRewriterWithInternalProperty()
		{
			IList<PersonEntity> people = new List<PersonEntity>
			{
				new PersonEntity { ID = 1, FirstName = "Daniel", LastName = "Chambers" },
				new PersonEntity { ID = 2, FirstName = "Colin", LastName = "Savage" },
			};

			IQueryable<PersonReport> queryable = people.AsQueryable()
				.Select(p => InternalToPersonReport(p))
				.Rewrite();
		}


		[RewriteUsingLambdaProperty(typeof(ExpressionTreeRewriterTests), "ToPersonReportLambda")]
		private PersonReport ToPersonReport(PersonEntity personEntity)
		{
			throw new InvalidOperationException();
		}

		[RewriteUsingLambdaProperty(typeof(ExpressionTreeRewriterTests), "ToPersonReportLambda")]
		private PersonReport ToPersonReportSubclassIn(PersonEntitySubclass personEntity)
		{
			throw new InvalidOperationException();
		}

		[RewriteUsingLambdaProperty(typeof(ExpressionTreeRewriterTests), "ToPersonReportSubclassOutLambda")]
		private PersonReportSubclass ToPersonReportSubclassOut(PersonEntity personEntity)
		{
			throw new InvalidOperationException();
		}

		[RewriteUsingLambdaProperty(typeof(ExpressionTreeRewriterTests), "InternalToPersonReportLambda")]
		private PersonReport InternalToPersonReport(PersonEntity personEntity)
		{
			throw new InvalidOperationException();
		}

		private class IntEntity
		{
			public int ID { get; set; }
		}

		private class StringEntity
		{
			public string ID { get; set; }
		}

		public class PersonEntity
		{
			public int ID { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
		}

		public class PersonEntitySubclass : PersonEntity
		{
		}

		public class PersonReport
		{
			public int ID { get; set; }
			public string FirstName { get; set; }
		}

		public class PersonReportSubclass : PersonReport
		{
		}

		public static Expression<Func<PersonEntity, PersonReport>> ToPersonReportLambda
		{
			get { return e => new PersonReport { ID = e.ID, FirstName = e.FirstName }; }
		}

		public static Expression<Func<PersonEntity, PersonReportSubclass>> ToPersonReportSubclassOutLambda
		{
			get { return e => new PersonReportSubclass { ID = e.ID, FirstName = e.FirstName }; }
		}

		internal static Expression<Func<PersonEntity, PersonReport>> InternalToPersonReportLambda
		{
			get { return e => new PersonReport { ID = e.ID, FirstName = e.FirstName }; }
		}
	}
}