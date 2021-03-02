using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;


namespace DigitallyCreated.Utilities.Ef
{
	/// <summary>
	/// This static class contains helper methods for working with Entity Framework
	/// </summary>
	public static class EfUtil
	{
		private static readonly IDictionary<Type, IEntityPropertyClearer> _PropertyClearers = new Dictionary<Type, IEntityPropertyClearer>();


		/// <summary>
		/// Detaches all the objects from the query away from the specified <see cref="ObjectContext"/>
		/// and returns them in an <see cref="IList{T}"/>.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		/// <typeparam name="T">The type returned by the <see cref="IQueryable{T}"/></typeparam>
		/// <param name="queryable">The query</param>
		/// <param name="context">The context</param>
		/// <returns>A list of objects that have been detached from the object context</returns>
		public static IList<T> Detach<T>(IQueryable<T> queryable, ObjectContext context)
		{
			List<T> list = queryable.ToList();
			foreach (T item in list)
				context.Detach(item);
			return list;
		}


		/// <summary>
		/// Marks all the scalar properties of the entity as modified on the specified 
		/// <see cref="ObjectContext"/>.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		/// <typeparam name="TEntity">The type of the entity</typeparam>
		/// <param name="entity">The entity</param>
		/// <param name="context">The <see cref="ObjectContext"/> that the entity is attached to</param>
		public static void SetAllModified<TEntity>(this TEntity entity, ObjectContext context) 
			where TEntity : IEntityWithKey
		{
			ObjectStateEntry stateEntry = context.ObjectStateManager.GetObjectStateEntry(entity.EntityKey);
			IEnumerable<string> propertyNameList = stateEntry.CurrentValues.DataRecordInfo.FieldMetadata.Select(pn => pn.FieldType.Name);

			foreach (string propName in propertyNameList)
				stateEntry.SetModifiedProperty(propName);
		}


		/// <summary>
		/// Sets a single property on an entity as modified on the specified <see cref="ObjectContext"/>.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		/// <typeparam name="TEntity">The type of the entity</typeparam>
		/// <typeparam name="TProp">The type of the property</typeparam>
		/// <param name="entity">The entity</param>
		/// <param name="propertySelector">
		/// A lambda (expression tree) that returns the property on <typeparamref name="TEntity"/>.
		/// </param>
		/// <param name="context">The <see cref="ObjectContext"/> that the entity is attached to</param>
		/// <returns>The entity (for fluent syntax)</returns>
		public static TEntity SetModified<TEntity, TProp>(this TEntity entity, Expression<Func<TEntity, TProp>> propertySelector, ObjectContext context)
			where TEntity : IEntityWithKey
		{
			MemberExpression memberExpr = propertySelector.Body as MemberExpression;
			if (memberExpr == null)
				throw new ArgumentException("The expression " + propertySelector + " is not a valid expression for this method. It must return a property on the parameter.");

			PropertyInfo propertyInfo = memberExpr.Member as PropertyInfo;
			if (propertyInfo == null)
				throw new ArgumentException("The expression " + propertySelector + " is not a valid expression for this method. It must return a property on the parameter.");
			
			ObjectStateEntry stateEntry = context.ObjectStateManager.GetObjectStateEntry(entity.EntityKey);

			stateEntry.SetModifiedProperty(propertyInfo.Name);

			return entity;
		}


		/// <summary>
		/// Copies all the scalar properties from an Entity Framework entity object to another object instance.
		/// Scalar properties are detected as properties with the attribute <see cref="EdmScalarPropertyAttribute"/>.
		/// </summary>
		/// <typeparam name="T">The type of object being copied</typeparam>
		/// <param name="fromEntityObj">Source object</param>
		/// <param name="toEntityObj">Destination object</param>
		/// <param name="includeEntityKeyProperties">
		/// True if you want to copy the entity key properties, false otherwise
		/// </param>
		public static void CopyScalarProperties<T>(T fromEntityObj, T toEntityObj, bool includeEntityKeyProperties)
		{
			IEnumerable<PropertyInfo> properties =
				from property in typeof(T).GetProperties()
				let attr = property.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), true)
					.Cast<EdmScalarPropertyAttribute>()
					.FirstOrDefault()
				where attr != null && (includeEntityKeyProperties || attr.EntityKeyProperty == false)
				select property;

			foreach (PropertyInfo property in properties)
				property.SetValue(toEntityObj, property.GetValue(fromEntityObj, null), null);
		}


		/// <summary>
		/// Clears all non-scalar properties on an <see cref="EntityObject"/>. This means it will clear all
		/// <see cref="EntityCollection{TEntity}"/>s, and all single multiplicity relation ends (all properties
		/// attributed with <see cref="EdmRelationshipNavigationPropertyAttribute"/>).
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is thread-safe.
		/// </para>
		/// <para>
		/// This method is very fast, as it will generate a type that is able to clear the entity when it
		/// first encounters that entity type, and then will reuse this type when it encounters that
		/// entity type again.
		/// </para>
		/// </remarks>
		/// <param name="entity">The entity</param>
		public static void ClearNonScalarProperties(EntityObject entity)
		{
			ClearNonScalarProperties(entity, false);
		}


		/// <summary>
		/// Clears all non-scalar properties on an <see cref="EntityObject"/>. This means it will clear all
		/// <see cref="EntityCollection{TEntity}"/>s, and all single multiplicity relation ends (all properties
		/// attributed with <see cref="EdmRelationshipNavigationPropertyAttribute"/>), and optionally all entity 
		/// key properties (properties attributed with <see cref="EdmScalarPropertyAttribute"/> and with 
		/// <see cref="EdmScalarPropertyAttribute.EntityKeyProperty"/> set to true).
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is thread-safe.
		/// </para>
		/// <para>
		/// This method is very fast, as it will generate a type that is able to clear the entity when it
		/// first encounters that entity type, and then will reuse this type when it encounters that
		/// entity type again.
		/// </para>
		/// </remarks>
		/// <param name="entity">The entity</param>
		/// <param name="clearEntityKeyProperties">Whether or not to clear entity key properties</param>
		public static void ClearNonScalarProperties(EntityObject entity, bool clearEntityKeyProperties)
		{
			IEntityPropertyClearer propertyClearer;

			lock (_PropertyClearers)
			{
				if (_PropertyClearers.ContainsKey(entity.GetType()) == false)
				{
					propertyClearer = GeneratePropertyClearer(entity);
					_PropertyClearers.Add(entity.GetType(), propertyClearer);
				}
				else
					propertyClearer = _PropertyClearers[entity.GetType()];
			}

			propertyClearer.Clear(entity, clearEntityKeyProperties);
		}


		/// <summary>
		/// Generates a class that clears the non-scalar properties on an entity.
		/// </summary>
		/// <param name="entity">The entity</param>
		/// <returns>The generated class</returns>
		private static IEntityPropertyClearer GeneratePropertyClearer(EntityObject entity)
		{
			Type entityType = entity.GetType();

			//Set up the compile unit
			CodeCompileUnit compileUnit = new CodeCompileUnit();
			compileUnit.ReferencedAssemblies.Add(typeof(System.ComponentModel.INotifyPropertyChanging).Assembly.Location); //System.dll
			compileUnit.ReferencedAssemblies.Add(typeof(EfUtil).Assembly.Location);
			compileUnit.ReferencedAssemblies.Add(typeof(EntityObject).Assembly.Location);
			compileUnit.ReferencedAssemblies.Add(entityType.Assembly.Location);

			//Create the namespace
			string namespaceName = typeof(EfUtil).Namespace + ".CodeGen";
			CodeNamespace codeGenNamespace = new CodeNamespace(namespaceName);
			compileUnit.Namespaces.Add(codeGenNamespace);

			//Create the class
			string genTypeName = entityType.FullName.Replace('.', '_') + "PropertyClearer";
			CodeTypeDeclaration genClass = new CodeTypeDeclaration(genTypeName);
			genClass.IsClass = true;
			codeGenNamespace.Types.Add(genClass);

			Type baseType = typeof(AbstractEntityPropertyClearer<>).MakeGenericType(entityType);
			genClass.BaseTypes.Add(new CodeTypeReference(baseType));
			
			//Create the method
			CodeMemberMethod clearEntityMethod = new CodeMemberMethod();
			genClass.Members.Add(clearEntityMethod);
			clearEntityMethod.Name = "ClearEntity";
			clearEntityMethod.ReturnType = new CodeTypeReference(typeof(void));
			clearEntityMethod.Parameters.Add(new CodeParameterDeclarationExpression(entityType, "entity"));
			clearEntityMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "clearEntityKeyProperties"));
			clearEntityMethod.Attributes = MemberAttributes.Override | MemberAttributes.Family;

			//Find all EntityCollection<> properties
			IEnumerable<PropertyInfo> entityCollections = from property in entityType.GetProperties()
			                                              where
			                                              	property.PropertyType.IsGenericType &&
			                                              	property.PropertyType.IsGenericTypeDefinition == false &&
			                                              	property.PropertyType.GetGenericTypeDefinition() ==
			                                              	typeof(EntityCollection<>)
			                                              select property;

			//Emit Clear() calls on the EntityCollection<>s
			foreach (PropertyInfo propertyInfo in entityCollections)
			{
				CodePropertyReferenceExpression propertyReferenceExpression = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("entity"), propertyInfo.Name);
				clearEntityMethod.Statements.Add(new CodeMethodInvokeExpression(propertyReferenceExpression, "Clear"));
			}

			//Find all single multiplicity relation ends
			IEnumerable<PropertyInfo> relationSingleEndProperties = from property in entityType.GetProperties()
																	from attribute in property.GetCustomAttributes(typeof(EdmRelationshipNavigationPropertyAttribute), true).Cast<EdmRelationshipNavigationPropertyAttribute>()
																	select property;

			//Emit assignments that set the properties to their default value
			foreach (PropertyInfo propertyInfo in relationSingleEndProperties)
			{
				CodeExpression defaultExpression = new CodeDefaultValueExpression(new CodeTypeReference(propertyInfo.PropertyType));

				CodePropertyReferenceExpression propertyReferenceExpression = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("entity"), propertyInfo.Name);
				clearEntityMethod.Statements.Add(new CodeAssignStatement(propertyReferenceExpression, defaultExpression));
			}

			//Find all entity key properties
			IEnumerable<PropertyInfo> idProperties = from property in entityType.GetProperties()
													 from attribute in property.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), true).Cast<EdmScalarPropertyAttribute>()
													 where attribute.EntityKeyProperty
													 select property;

			//Emit the if check for wiping ID properties
			CodeConditionStatement ifCondition = new CodeConditionStatement(new CodeArgumentReferenceExpression("clearEntityKeyProperties"));
			clearEntityMethod.Statements.Add(ifCondition);

			//Emit assignments that set the properties to their default value
			foreach (PropertyInfo propertyInfo in idProperties)
			{
				CodeExpression defaultExpression = new CodeDefaultValueExpression(new CodeTypeReference(propertyInfo.PropertyType));

				CodePropertyReferenceExpression propertyReferenceExpression = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("entity"), propertyInfo.Name);
				ifCondition.TrueStatements.Add(new CodeAssignStatement(propertyReferenceExpression, defaultExpression));
			}

			//Compile the code and load the assembly
			CSharpCodeProvider provider = new CSharpCodeProvider();
			CompilerParameters parameters = new CompilerParameters();
			parameters.GenerateInMemory = true;
#if DEBUG
			parameters.IncludeDebugInformation = true;
#endif
			CompilerResults results = provider.CompileAssemblyFromDom(parameters, compileUnit);

			//Check for compile errors
			if (results.Errors.Count != 0)
			{
				StringBuilder error = new StringBuilder("Failed to generate the property clearer class. Compiler output:\r\n");
				foreach (string str in results.Output)
				{
					error.Append(str);
					error.Append("\r\n");
				}
				throw new Exception(error.ToString());	
			}

			//Instantiate the type and return it
			Type type = results.CompiledAssembly.GetType(namespaceName + "." + genTypeName);
			return (IEntityPropertyClearer)Activator.CreateInstance(type);
		}
	}
}