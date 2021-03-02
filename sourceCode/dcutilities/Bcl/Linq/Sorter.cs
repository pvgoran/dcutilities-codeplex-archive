using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace DigitallyCreated.Utilities.Bcl.Linq
{
	/// <summary>
	/// A Sorter allows you to set and save a sort order over multiple properties on an object. It links in
	/// smoothly with LINQ by providing an extra OrderBy extension method to <see cref="IQueryable{T}"/>.
	/// It supports a  fluent syntax, so you can chain method calls together.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Sorter has the ability to be encoded into a short string, suitable for putting in URLs. 
	/// </para>
	/// <para>
	/// Here is an example for a Customer class than has properties<c>FirstName</c> and <c>LastName</c>: 
	/// <c>FirstName'LastName!</c>. This means that Customers should be sorted first by property 
	/// <c>FirstName</c>ascending and then by property <c>LastName</c> descending. The exclamation mark
	/// means descending order (its absence means ascending order). The apostrophe is used as a separator
	/// between property names.
	/// </para>
	/// <para>
	/// You can use a prefix in front of names if you like. For example, with the prefix "C" (for customer)
	/// you could have: <c>C.FirstName!'C.LastName!</c>. This means that Customers should be ordered by 
	/// <c>FirstName</c> descending, and then <c>LastName</c> descending. The full stop character is used
	/// to delimit the prefix. The prefix to use is set in the constructor. You may not use a prefix that
	/// contains a full stop character.
	/// </para>
	/// <para>
	/// You are able to set translation names for properties, in order to shrink the encoded string. By
	/// using the appropriate constructor you can set a dictionary that whose keys are the property names,
	/// and whose values are the names you want them translated to. For example, with the dictionary 
	/// <c>c => c.FirstName -> "F", c => c.LastName -> "L"</c>, you could get an encoded string like 
	/// <c>L'F</c>. This means sort Customers by <c>LastName</c> ascending then by <c>FirstName</c>
	/// ascending. You can use prefixes here too, the same example this time with prefix "C" would be 
	/// <c>C.L'C.F</c>.
	/// </para>
	/// <para>
	/// Methods on Sorter either take the full encoded string (for example, 
	/// <see cref="Decode(string,string)"/>), or they take a string array. The string array version of the
	/// encoding is effectively the same as doing a <see cref="String.Split(char[])"/> on the single string
	/// version, splitting on the apostrophe character. So the encoded string <c>FirstName,LastName!</c> in
	/// string array format is simply <c>FirstName</c> (in array element 0) and <c>LastName!</c> (in array
	/// element 1).
	/// </para>
	/// <para>
	/// The Sorter supports serialization to XML and back again (it implements <see cref="IXmlSerializable"/>
	/// and is annotated with <see cref="XmlSchemaProviderAttribute"/>). This means that it can be serialized 
	/// and sent across a WCF service to another .NET client that is using the Sorter class successfully.
	/// Ostensibly a service/service client written in another language (ie Java) could support the Sorter,
	/// but this would be difficult and unlikely since they would have to implement similar functionality
	/// in their language. So for web services that are public facing (or known to have non-.NET clients)
	/// it would be wise <i>not</i> to use the Sorter.
	/// </para>
	/// </remarks>
	/// <typeparam name="T">The type that will be sorted</typeparam>
	[XmlSchemaProvider("ProvideXmlSerializationSchema")]
	public class Sorter<T> : ICloneable, IXmlSerializable
	{
		private static readonly XmlSchema _SerializationSchema;
		private readonly IList<KeyValuePair<LambdaExpression, bool>> _OrderedPropertySelectors;
		private readonly IList<KeyValuePair<LambdaExpression, bool>> _ReadOnlyOrderedPropertySelectors;
		private readonly Type _TypeT;
		private IDictionary<string, LambdaExpression> _FakeNameToPropertySelectorDictionary;
		private IList<KeyValuePair<LambdaExpression, string>> _PropertySelectorToFakeNameList;


		/// <summary>
		/// Gets property selectors (expression trees that indicate a property on
		/// type <typeparamref name="T"/>) in the sort order, paired with their sort
		/// direction (true is ascending, false is descending)
		/// </summary>
		/// <remarks>
		/// This returns a read-only collection and therefore cannot be modified
		/// </remarks>
		public IList<KeyValuePair<LambdaExpression, bool>> OrderedPropertySelectors
		{
			get { return _ReadOnlyOrderedPropertySelectors; }
		}


		/// <summary>
		/// The prefix that is used before the parameter name in encoded strings
		/// (see class remarks for more information)
		/// </summary>
		public string Prefix { get; private set; }


		/// <summary>
		/// Static constructor
		/// </summary>
		static Sorter()
		{
			using (Stream stream = typeof(Sorter<>).Assembly.GetManifestResourceStream("DigitallyCreated.Utilities.Bcl.Linq.SorterSerializationSchema.xsd"))
			{
				if (stream == null)
					throw new Exception("Failed to load the embedded resource DigitallyCreated.Utilities.Bcl.Linq.SorterSerializationSchema.xsd");

				_SerializationSchema = XmlSchema.Read(stream, null);
			}
		}

        
		/// <summary>
		/// Constructor, creates a Sorter with a null prefix and no translation dictionary
		/// </summary>
		public Sorter()
			: this(null as string)
		{
		}


		/// <summary>
		/// Constructor, creates an empty Sorter with no translation dictionary
		/// </summary>
		/// <param name="prefix">The encoded properties prefix (see class remarks for more information)</param>
		public Sorter(string prefix)
			: this(prefix, null as IDictionary<Expression<Func<T, object>>, string>)
		{
		}


		/// <summary>
		/// Constructor, creates an empty Sorter with a null prefix
		/// </summary>
		/// <param name="translationDictionary">
		/// A translation dictionary that allows you control the names found in the encoded string created by
		/// this sorter. The key of each entry should be a property selector expression for type
		/// <typeparamref name="T"/> and the value should be the name you want used for that selected property
		/// in the encoded string.
		/// </param>
		public Sorter(IDictionary<Expression<Func<T, object>>, string> translationDictionary)
			: this(null, translationDictionary)
		{
		}


		/// <summary>
		/// Constructor, creates an empty Sorter
		/// </summary>
		/// <param name="prefix">The encoded properties prefix (see class remarks for more information)</param>
		/// <param name="translationDictionary">
		/// A translation dictionary that allows you control the names found in the encoded string created by
		/// this sorter. The key of each entry should be a property selector expression for type
		/// <typeparamref name="T"/> and the value should be the name you want used for that selected property
		/// in the encoded string.
		/// </param>
		public Sorter(string prefix, IDictionary<Expression<Func<T, object>>, string> translationDictionary)
		{
			_OrderedPropertySelectors = new List<KeyValuePair<LambdaExpression, bool>>();
			_ReadOnlyOrderedPropertySelectors = new ReadOnlyCollection<KeyValuePair<LambdaExpression, bool>>(_OrderedPropertySelectors);
			_TypeT = typeof(T);
			if (prefix != null && (prefix == String.Empty || prefix.Contains('.')))
				throw new ArgumentException("prefix cannot contain a '.' character or be an empty string.");
			Prefix = prefix;
			SetNameTranslationDictionary(translationDictionary);
		}


		/// <summary>
		/// Constructor, creates a Sorter with no translation dictionary and sets it with information decoded
		/// from the string properties passed in
		/// </summary>
		/// <param name="prefix">The encoded properties prefix (see class remarks for more information)</param>
		/// <param name="properties">
		/// String array encoded properties (see class remarks for more information)
		/// </param>
		public Sorter(string prefix, params string[] properties)
			: this(prefix)
		{
			AddProperties(properties);
		}


		/// <summary>
		/// Constructor, creates a Sorter and sets it with information decoded from the string properties
		/// passed in
		/// </summary>
		/// <param name="prefix">The encoded properties prefix (see class remarks for more information)</param>
		/// <param name="translationDictionary">
		/// A translation dictionary that allows you control the names found in the encoded string created by
		/// this sorter. The key of each entry should be a property selector expression for type
		/// <typeparamref name="T"/> and the value should be the name you want used for that selected property
		/// in the encoded string.
		/// </param>
		/// <param name="properties">
		/// String array encoded properties (see class remarks for more information)
		/// </param>
		public Sorter(string prefix, IDictionary<Expression<Func<T, object>>, string> translationDictionary, params string[] properties)
			: this(prefix, translationDictionary)
		{
			AddProperties(properties);
		}


		/// <summary>
		/// Private copy constructor. Use <see cref="Clone"/> instead.
		/// </summary>
		private Sorter(Sorter<T> other)
		{
			_OrderedPropertySelectors = new List<KeyValuePair<LambdaExpression, bool>>(other._OrderedPropertySelectors);
			_ReadOnlyOrderedPropertySelectors = new ReadOnlyCollection<KeyValuePair<LambdaExpression, bool>>(_OrderedPropertySelectors);
			_TypeT = other._TypeT;
			_FakeNameToPropertySelectorDictionary = other._FakeNameToPropertySelectorDictionary;
			_PropertySelectorToFakeNameList = other._PropertySelectorToFakeNameList;
			Prefix = other.Prefix;
		}


		/// <summary>
		/// Sets a translation dictionary that allows you control the names found in the encoded string created
		/// by this sorter. The key of each entry should be a property selector expression for type
		/// <typeparamref name="T"/> and the value should be the name you want used for that selected property
		/// in the encoded string.
		/// </summary>
		/// <param name="translationDictionary">The dictionary</param>
		private void SetNameTranslationDictionary(IDictionary<Expression<Func<T, object>>, string> translationDictionary)
		{
			if (translationDictionary == null)
			{
				_PropertySelectorToFakeNameList = null;
				_FakeNameToPropertySelectorDictionary = null;
			}
			else
			{
				_PropertySelectorToFakeNameList = new List<KeyValuePair<LambdaExpression, string>>();
				_FakeNameToPropertySelectorDictionary = new Dictionary<string, LambdaExpression>();

				foreach (KeyValuePair<Expression<Func<T, object>>, string> pair in translationDictionary)
				{
					ValidatePropertySelectorExpression(pair.Key);
					LambdaExpression propertySelector = RebuildPropertySelector(pair.Key); //See called method's remarks for more info

					_FakeNameToPropertySelectorDictionary.Add(pair.Value, propertySelector);
					_PropertySelectorToFakeNameList.Add(new KeyValuePair<LambdaExpression, string>(propertySelector, pair.Value));
				}
					
			}
		}


		/// <summary>
		/// Rebuilds a property selector. This rebuilds the expression and ensures that it returns as the
		/// type of the selected property.
		/// </summary>
		/// <remarks>
		/// This is useful for rebuilding property selectors created in a translation dictionary. Translation
		/// dictionary property selectors are polluted because their return type is object, not the type of the
		/// property being selected. This leads to auto-boxing of properties that are a value type, which
		/// produces an expression tree that LINQ to Entities does not support.
		/// </remarks>
		/// <param name="propertySelector">The property selector to rebuild</param>
		/// <returns>The rebuild property selector</returns>
		private LambdaExpression RebuildPropertySelector(LambdaExpression propertySelector)
		{
			IList<PropertyInfo> properties = GetPropertyInfoChainForPropertySelector(propertySelector);

			ParameterExpression paramExpression = Expression.Parameter(_TypeT, "t");

			Expression accessFromExpression = paramExpression;
			foreach (PropertyInfo property in properties)
				accessFromExpression = Expression.Property(accessFromExpression, property);

			return Expression.Lambda(accessFromExpression, paramExpression);
		}


		/// <summary>
		/// Adds a property on <typeparamref name="T"/> to be sorted by this sortable.
		/// </summary>
		/// <typeparam name="TProp">The type of the property specified</typeparam>
		/// <param name="property">
		/// A lambda (expression tree) that returns the property on <typeparamref name="T"/> to sort by.
		/// </param>
		/// <param name="ascending">
		/// True if you want the property to be sorted in ascending order, false for descending order
		/// </param>
		/// <returns>This sorter (for fluent syntax)</returns>
		public Sorter<T> AddProperty<TProp>(Expression<Func<T, TProp>> property, bool ascending)
		{
			ValidatePropertySelectorExpression(property);

			_OrderedPropertySelectors.Add(new KeyValuePair<LambdaExpression, bool>(property, ascending));

			return this;
		}


		/// <summary>
		/// Adds all the properties found on <typeparamref name="T"/> for sorting 
		/// in alphabetical order
		/// </summary>
		/// <param name="ascending">
		/// True if you want the property to be sorted in ascending order, false for descending order
		/// </param>
		/// <returns>This sorter (for fluent syntax)</returns>
		public Sorter<T> AddAllProperties(bool ascending)
		{
			IOrderedEnumerable<PropertyInfo> properties = _TypeT.GetProperties().OrderBy(x => x.Name);
			foreach (PropertyInfo propertyInfo in properties)
			{
				bool ignoredOut;
				KeyValuePair<LambdaExpression, bool> pair = new KeyValuePair<LambdaExpression, bool>(DecodePropertyString(propertyInfo.Name, out ignoredOut), ascending);
				_OrderedPropertySelectors.Add(pair);
			}

			return this;
		}


		/// <summary>
		/// Adds a number of properties (string array encoded) to be sorted.
		/// </summary>
		/// <param name="properties">
		/// String array encoded properties (see class remarks for more information)
		/// </param>
		/// <returns>This sorter (for fluent syntax)</returns>
		public Sorter<T> AddProperties(IEnumerable<string> properties)
		{
			foreach (string propertyToken in properties)
			{
				bool ascending;
				LambdaExpression propertySelector = DecodePropertyString(propertyToken, out ascending);

				KeyValuePair<LambdaExpression, bool> pair = new KeyValuePair<LambdaExpression, bool>(propertySelector, ascending);
				_OrderedPropertySelectors.Add(pair);
			}

			return this;
		}


		/// <summary>
		/// Adds a number of properties (string array encoded) to be sorted.
		/// </summary>
		/// <param name="properties">
		/// String array encoded properties (see class remarks for more information)
		/// </param>
		/// <returns>This sorter (for fluent syntax)</returns>
		public Sorter<T> AddProperties(params string[] properties)
		{
			AddProperties((IEnumerable<string>)properties);

			return this;
		}


		/// <summary>
		/// Removes a particular property selector. Note that this property selector must an instance
		/// returned from the <see cref="OrderedPropertySelectors"/> property.
		/// </summary>
		/// <param name="propertySelector">The property selector to remove</param>
		/// <returns>This sorter (for fluent syntax)</returns>
		public Sorter<T> RemoveProperty(LambdaExpression propertySelector)
		{
			int index = -1;
			for (int i = 0; i < _OrderedPropertySelectors.Count; i++)
			{
				if (_OrderedPropertySelectors[i].Key == propertySelector)
				{
					index = i;
					break;
				}
			}
			if (index != -1)
				_ReadOnlyOrderedPropertySelectors.RemoveAt(index);

			return this;
		}


		/// <summary>
		/// Gets the sort direction set for the specified property
		/// </summary>
		/// <typeparam name="TProp">The selected property's type</typeparam>
		/// <param name="propertySelector">The property selector expression</param>
		/// <returns>True if the property is set to sort ascending, false if it is set to sort descending</returns>
		/// <exception cref="InvalidOperationException">
		/// If the specified property has not been added to this sorter
		/// </exception>
		public bool GetSortDirectionForProperty<TProp>(Expression<Func<T, TProp>> propertySelector)
		{
			return OrderedPropertySelectors.Where(ps => PropertySelectorsEqual(ps.Key, propertySelector)).First().Value;
		}


		/// <summary>
		/// Validates a property property selector lambda expression tree
		/// </summary>
		/// <typeparam name="TProp">The property's type</typeparam>
		/// <param name="propertyExpression">The lambda</param>
		private void ValidatePropertySelectorExpression<TProp>(Expression<Func<T, TProp>> propertyExpression)
		{
			try
			{
				ValidatePropertySelectorExpression(propertyExpression.Body, propertyExpression.Parameters[0]);
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException("The expression " + propertyExpression + " is not a valid expression for this method. See inner exception for more details.", e);
			}

		}


		/// <summary>
		/// Recursively dismantles and validates a property selector lambda expression tree.
		/// </summary>
		/// <param name="expression">The expression to dismantle and validate</param>
		/// <param name="expectedParamExpression">
		/// The expected <see cref="ParameterExpression"/> that the final <see cref="MemberExpression"/> should
		/// be accessing
		/// </param>
		private void ValidatePropertySelectorExpression(Expression expression, ParameterExpression expectedParamExpression)
		{
			if (expression is UnaryExpression)
			{
				//UnaryExpressions can occur in translation dictionary-generated property selectors
				//when the selected property is of value type and is autoboxed to object type
				UnaryExpression unaryExpression = (UnaryExpression)expression;

				if (unaryExpression.NodeType != ExpressionType.Convert)
					throw new ArgumentException(String.Format("Found UnaryExpression that does not perform a convert ({0}). Instead, it is of {1} type", unaryExpression, unaryExpression.NodeType));

				ValidatePropertySelectorExpression(unaryExpression.Operand, expectedParamExpression);
			}
			else if (expression is MemberExpression)
			{
				MemberExpression memberExpression = (MemberExpression)expression;

				if (memberExpression.Member is PropertyInfo == false)
					throw new ArgumentException(String.Format("Found MemberExpression that does not access a property ({0}). Instead, it accesses a {1}", memberExpression, memberExpression.Member.MemberType));

				ValidatePropertySelectorExpression(memberExpression.Expression, expectedParamExpression);
			}
			else if (expression is ParameterExpression)
			{
				if (expression != expectedParamExpression)
					throw new ArgumentException(String.Format("Unexpected parameter expression found ({0})", expression));
			}
			else
				throw new ArgumentException(String.Format("Unexpected expression found of type {0} ({1})", expression.GetType().Name, expression));
		}


		/// <summary>
		/// Decodes an encoded property name and retrieves the property name and sort order from it
		/// </summary>
		/// <param name="propertyToken">The encoded property</param>
		/// <param name="ascending">Ascending sort order if true, descending if false</param>
		/// <returns>The property name</returns>
		private LambdaExpression DecodePropertyString(string propertyToken, out bool ascending)
		{
			string propertyNameChain;

			if (propertyToken[propertyToken.Length - 1] == '!')
			{
				propertyNameChain = propertyToken.Substring(0, propertyToken.Length - 1);
				ascending = false;
			}
			else
			{
				propertyNameChain = propertyToken;
				ascending = true;
			}

			if (Prefix != null)
			{
				int index = propertyNameChain.IndexOf('.');
				if (index == -1)
					throw new ArgumentException(String.Format("The property string {0} is invalid. A prefix has been specified, but no . has been found.", propertyToken));

				string substr = propertyNameChain.Substring(0, index);
				if (substr != Prefix)
					throw new ArgumentException(String.Format("The property string {0} is invalid. A prefix ({1}) has been specified, but everything before the first . is not the prefix", propertyToken, Prefix));

				propertyNameChain = propertyNameChain.Substring(index + 1);
			}

			if (_FakeNameToPropertySelectorDictionary != null && _FakeNameToPropertySelectorDictionary.ContainsKey(propertyNameChain))
				return _FakeNameToPropertySelectorDictionary[propertyNameChain];

			return BuildPropertySelectorFromPropertyNameChain(propertyNameChain);
		}


		/// <summary>
		/// Builds a property selector <see cref="LambdaExpression"/> from a property name chain string
		/// </summary>
		/// <param name="propertyNameChain">
		/// The property name chain string (for example, Order.FirstItem.Name)
		/// </param>
		/// <returns>The property selector expression</returns>
		private LambdaExpression BuildPropertySelectorFromPropertyNameChain(string propertyNameChain)
		{
			string[] propertyNames = propertyNameChain.Split('.');
			ParameterExpression paramExpression = Expression.Parameter(_TypeT, "t");

			Expression accessFromExpression = paramExpression;
			foreach (string propertyName in propertyNames)
				accessFromExpression = Expression.Property(accessFromExpression, propertyName);

			return Expression.Lambda(accessFromExpression, paramExpression);
		}


		/// <summary>
		/// Makes the property specified the first property to be sorted and pushes
		/// all other sortable properties down one in the sort order
		/// </summary>
		/// <param name="property">
		/// An encoded property (see class remarks for more information)
		/// </param>
		/// <returns>This sorter (for fluent syntax)</returns>>
		public Sorter<T> SortBy(string property)
		{
			bool ascending;
			LambdaExpression propertySelector = DecodePropertyString(property, out ascending);

			SortBy(propertySelector, ascending);

			return this;
		}


		/// <summary>
		/// Makes the property specified the first property to be sorted and pushes all other sortable
		/// properties down one in the sort order
		/// </summary>
		/// <typeparam name="TProp">The type of the property specified</typeparam>
		/// <param name="property">
		/// A lambda (expression tree) that returns the property on <typeparamref name="T"/> to sort by.
		/// </param>
		/// <param name="ascending">
		/// True if you want the property to be sorted in ascending order, false for descending order
		/// </param>
		/// <returns>This sorter (for fluent syntax)</returns>
		public Sorter<T> SortBy<TProp>(Expression<Func<T, TProp>> property, bool ascending)
		{
			ValidatePropertySelectorExpression(property);

			SortBy(property as LambdaExpression, ascending);

			return this;
		}


		/// <summary>
		/// Makes the property specified the first property to be sorted and pushes
		/// all other sortable properties down one in the sort order
		/// </summary>
		/// <param name="propertySelector">The property selector lambda</param>
		/// <param name="ascending">
		/// True if you want the property to be sorted in ascending order, false for descending order
		/// </param>
		private void SortBy(LambdaExpression propertySelector, bool ascending)
		{
			int index = -1;
			for (int i = 0; i < _OrderedPropertySelectors.Count; i++)
			{
				KeyValuePair<LambdaExpression, bool> pair = _OrderedPropertySelectors[i];

				if (PropertySelectorsEqual(pair.Key, propertySelector))
				{
					index = i;
					break;
				}
			}

			if (index == -1)
				throw new ArgumentException("property must be a property that has been added in the past");

			KeyValuePair<LambdaExpression, bool> keySelectorPair = _OrderedPropertySelectors[index];
			_OrderedPropertySelectors.RemoveAt(index);
			_OrderedPropertySelectors.Insert(0, new KeyValuePair<LambdaExpression, bool>(keySelectorPair.Key, ascending));
		}


		/// <summary>
		/// Compares two property selector expression trees to see whether they're the same
		/// </summary>
		/// <param name="expr1">The first expression</param>
		/// <param name="expr2">The second expression</param>
		/// <returns>True if they are the same, false otherwise</returns>
		public bool PropertySelectorsEqual(LambdaExpression expr1, LambdaExpression expr2)
		{
			if (expr1.Parameters[0].Type != _TypeT || expr2.Parameters[0].Type != _TypeT)
				return false;

			IList<String> expr1Props = GetPropertyNameChain(expr1);
			IList<String> expr2Props = GetPropertyNameChain(expr2);

			if (expr1Props.Count != expr2Props.Count)
				return false;

			for (int i = 0; i < expr1Props.Count; i++)
			{
				if (expr1Props[i] != expr2Props[i])
					return false;
			}

			return true;
		}


		/// <summary>
		/// Gets the property name chain for a property selector expression. So
		/// <c>x => x.Customer.Name</c> would return <c>{"Customer","Name"}</c>.
		/// </summary>
		/// <param name="propertySelectorExpression">The property selector expression tree</param>
		/// <returns>The property name chain</returns>
		private IList<string> GetPropertyNameChain(LambdaExpression propertySelectorExpression)
		{
			return GetPropertyInfoChainForPropertySelector(propertySelectorExpression).Select(p => p.Name).ToList();
		}


		/// <summary>
		/// Gets the chain of Properties for a property selector expressions.
		/// </summary>
		/// <param name="propertySelectorExpression">The property selector expression tree</param>
		/// <returns>The property name chain</returns>
		private IList<PropertyInfo> GetPropertyInfoChainForPropertySelector(LambdaExpression propertySelectorExpression)
		{
			List<PropertyInfo> properties = new List<PropertyInfo>();
			Expression expression = propertySelectorExpression.Body;

			while (expression is ParameterExpression == false)
			{
				if (expression is UnaryExpression)
				{
					//UnaryExpressions can occur in translation dictionary-generated property selectors
					//when the selected property is of value type and is autoboxed to object type
					UnaryExpression unaryExpression = (UnaryExpression)expression;
					expression = unaryExpression.Operand;
				}
				else
				{
					MemberExpression memberExpression = (MemberExpression)expression;
					properties.Add((PropertyInfo)memberExpression.Member);
					expression = memberExpression.Expression;
				}
			}

			properties.Reverse();

			return properties;
		}


		/// <summary>
		/// Writes the full property selector name for the specified property selector expression 
		/// to the specified <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="propertySelector">The property selector expression</param>
		/// <param name="builder">The <see cref="StringBuilder"/> to write to</param>
		private void WriteFullPropertyNameForPropertySelectorToBuilder(LambdaExpression propertySelector, StringBuilder builder)
		{
			IList<string> propertyNameChain = GetPropertyNameChain(propertySelector);
			foreach (string name in propertyNameChain)
			{
				builder.Append(name);
				builder.Append(".");
			}
			builder.Remove(builder.Length - 1, 1); //Remove trailing .
		}


		/// <summary>
		/// Encodes this Sorter into an encoded string array. See the remarks section
		/// on the class for more information about encoding.
		/// </summary>
		/// <returns>The sorting order encoded into a string array</returns>
		public string[] EncodeToArray()
		{
			string[] properties = new string[_OrderedPropertySelectors.Count];

			for (int i = 0; i < _OrderedPropertySelectors.Count; i++)
			{
				KeyValuePair<LambdaExpression, bool> pair = _OrderedPropertySelectors[i];
				StringBuilder builder = new StringBuilder();

				if (Prefix != null)
				{
					builder.Append(Prefix);
					builder.Append(".");
				}

				string translatedName = GetTranslatedNameForPropertySelector(pair.Key);
				if (translatedName != null)
					builder.Append(translatedName);
				else
					WriteFullPropertyNameForPropertySelectorToBuilder(pair.Key, builder);

				if (pair.Value == false)
					builder.Append("!");

				properties[i] = builder.ToString();
			}

			return properties;
		}


		/// <summary>
		/// Gets the translated name for the specified property selector expression,
		/// or null if one hasn't been specified
		/// </summary>
		/// <param name="propertySelector">The property selector</param>
		/// <returns>The translated name, or null if one was not specified</returns>
		private string GetTranslatedNameForPropertySelector(LambdaExpression propertySelector)
		{
			if (_PropertySelectorToFakeNameList == null)
				return null;

			foreach (KeyValuePair<LambdaExpression, string> pair in _PropertySelectorToFakeNameList)
			{
				if (PropertySelectorsEqual(pair.Key, propertySelector))
					return pair.Value;
			}

			return null;
		}


		/// <summary>
		/// Encodes this Sorter into a single string. See the remarks section on the class
		/// for more information about encoding
		/// </summary>
		/// <returns>The sorting order encoded into a single string</returns>
		public string Encode()
		{
			string[] properties = EncodeToArray();
			StringBuilder builder = new StringBuilder();

			foreach (string property in properties)
			{
				builder.Append(property);
				builder.Append("'");
			}

			if (builder.Length > 0)
				builder.Remove(builder.Length - 1, 1);

			return builder.ToString();
		}


		/// <summary>
		/// Decodes an encoded string and returns the new Sorter. See the remarks section on the class
		/// for more information about encoding
		/// </summary>
		/// <param name="prefix">The encoded properties prefix (see class remarks for more information)</param>
		/// <param name="encodedString">The encoded string</param>
		/// <returns>The Sorter created by decoding the string</returns>
		/// <exception cref="SorterException">If there was an error decoding the string</exception>
		public static Sorter<T> Decode(string prefix, string encodedString)
		{
			string[] split = encodedString.Split('\'');
			try
			{
				return new Sorter<T>(prefix, split);
			}
			catch (ArgumentException e)
			{
				throw new SorterException("Error decoding the string", e);
			}
		}


		/// <summary>
		/// Decodes an encoded string and returns the new Sorter, using a translation dictionary. See the
		/// remarks section on the class for more information about encoding
		/// </summary>
		/// <param name="prefix">The encoded properties prefix (see class remarks for more information)</param>
		/// <param name="translationDictionary">The translation dictionary</param>
		/// <param name="encodedString">The encoded string</param>
		/// <returns>The Sorter created by decoding the string</returns>
		/// <exception cref="SorterException">If there was an error decoding the string</exception>
		public static Sorter<T> Decode(string prefix, IDictionary<Expression<Func<T, object>>, string> translationDictionary, string encodedString)
		{
			string[] split = encodedString.Split('\'');

			try 
			{
				return new Sorter<T>(prefix, translationDictionary)
					.AddProperties(split);
			}
			catch (ArgumentException e)
			{
				throw new SorterException("Error decoding the string", e);
			}
		}


		/// <summary>
		/// Clones this Sorter
		/// </summary>
		/// <returns>The cloned Sorter</returns>
		object ICloneable.Clone()
		{
			return new Sorter<T>(this);
		}


		/// <summary>
		/// Clones this Sorter
		/// </summary>
		/// <returns>The cloned Sorter (strongly typed)</returns>
		public Sorter<T> Clone()
		{
			return new Sorter<T>(this);
		}


		/// <summary>
		/// Will be called during XML serialization in order to retrieve the XML Schema used for serialization.
		/// This behaviour is caused by the application of the 
		/// <see cref="XmlSchemaProviderAttribute"/> on the class.
		/// </summary>
		/// <param name="schemaSet">The <see cref="XmlSchemaSet"/> to add the schema to</param>
		/// <returns>
		/// The <see cref="XmlQualifiedName"/> that refers to the type that is to be used as the root type for
		/// serialization
		/// </returns>
		public static XmlQualifiedName ProvideXmlSerializationSchema(XmlSchemaSet schemaSet)
		{
			schemaSet.Add(_SerializationSchema);
			return new XmlQualifiedName("Sorter", _SerializationSchema.TargetNamespace);
		}


		/// <summary>
		/// Always returns null (as it must do as specified by the MSDN documentation for 
		/// <see cref="IXmlSerializable.GetSchema"/>).
		/// </summary>
		/// <returns>Null</returns>
		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}


		/// <summary>
		/// Initialises the <see cref="Sorter{T}"/> from its XML representation that was written by
		/// <see cref="IXmlSerializable.WriteXml"/>.
		/// </summary>
		/// <param name="reader">The <see cref="XmlReader"/> to read from</param>
		/// <exception cref="SerializationException">
		/// If you try to deserialize a <see cref="Sorter{T}"/> of one type into a <see cref="Sorter{T}"/>of
		/// another type. (ie a deserializing a <c>Sorter&lt;Customer&gt;</c> into a 
		/// <c>Sorter&lt;Employee&gt;</c>)
		/// </exception>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string sortObjectType = reader.GetAttribute("SortObjectType");
			if (_TypeT.FullName != sortObjectType)
				throw new SerializationException(String.Format("Deserialization error. Trying to deserialize a Sorter<{0}> into a Sorter<{1}>", sortObjectType, _TypeT.FullName));

			Prefix = reader.GetAttribute("Prefix");

			reader.ReadToDescendant("OrderedSortProperties");
			if (reader.IsEmptyElement == false)
			{
				_OrderedPropertySelectors.Clear();

				bool continueReading = reader.ReadToDescendant("SortItem");
				while (continueReading)
				{
					reader.ReadToDescendant("PropertySelector");
					string psStr = reader.ReadElementContentAsString();
					reader.ReadStartElement("SortAsc");
					bool sortAsc = reader.ReadContentAsBoolean();
					reader.ReadEndElement();

					LambdaExpression propertySelector = BuildPropertySelectorFromPropertyNameChain(psStr);
					_OrderedPropertySelectors.Add(new KeyValuePair<LambdaExpression, bool>(propertySelector, sortAsc));

					continueReading = reader.ReadToNextSibling("SortItem");
				}
			}

			reader.ReadToNextSibling("TranslationDictionary");
			if (reader.IsEmptyElement == false)
			{
				_PropertySelectorToFakeNameList = new List<KeyValuePair<LambdaExpression, string>>();
				_FakeNameToPropertySelectorDictionary = new Dictionary<string, LambdaExpression>();

				bool continueReading = reader.ReadToDescendant("Pair");
				while (continueReading)
				{
					reader.ReadToDescendant("FakeName");
					string key = reader.ReadElementContentAsString();
					reader.ReadStartElement("PropertySelector");
					string value = reader.ReadContentAsString();
					reader.ReadEndElement();

					LambdaExpression propertySelector = BuildPropertySelectorFromPropertyNameChain(value);
					_PropertySelectorToFakeNameList.Add(new KeyValuePair<LambdaExpression, string>(propertySelector, key));
					_FakeNameToPropertySelectorDictionary.Add(key, propertySelector);

					continueReading = reader.ReadToNextSibling("Pair");
				}
			}
		}


		/// <summary>
		/// Writes the <see cref="Sorter{T}"/> out to its XML representation
		/// </summary>
		/// <param name="writer">The <see cref="XmlWriter"/> to write out with</param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("SortObjectType", _TypeT.FullName);
			writer.WriteAttributeString("Prefix", Prefix);
			
			writer.WriteStartElement("OrderedSortProperties");
			foreach (KeyValuePair<LambdaExpression, bool> pair in _OrderedPropertySelectors)
			{
				writer.WriteStartElement("SortItem");
				
				StringBuilder builder = new StringBuilder();
				WriteFullPropertyNameForPropertySelectorToBuilder(pair.Key, builder);
				writer.WriteStartElement("PropertySelector");
				writer.WriteValue(builder.ToString());
				writer.WriteEndElement();

				writer.WriteStartElement("SortAsc");
				writer.WriteValue(pair.Value);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}
			writer.WriteEndElement();

			writer.WriteStartElement("TranslationDictionary");
			if (_FakeNameToPropertySelectorDictionary != null)
			{
				foreach (KeyValuePair<string, LambdaExpression> pair in _FakeNameToPropertySelectorDictionary)
				{
					writer.WriteStartElement("Pair");

					writer.WriteStartElement("FakeName");
					writer.WriteValue(pair.Key);
					writer.WriteEndElement();

					StringBuilder builder = new StringBuilder();
					WriteFullPropertyNameForPropertySelectorToBuilder(pair.Value, builder);
					writer.WriteStartElement("PropertySelector");
					writer.WriteValue(builder.ToString());
					writer.WriteEndElement();

					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
		}
	}


	/// <summary>
	/// Extension methods for <see cref="IQueryable{T}"/> to add Sorter functionality to LINQ.
	/// </summary>
	public static class SorterExtensions
	{
		/// <summary>
		/// Orders the query results by the settings set in the specified <see cref="Sorter{T}"/>
		/// </summary>
		/// <typeparam name="TSource">The type being queried</typeparam>
		/// <param name="queryable">The <see cref="IQueryable{T}"/></param>
		/// <param name="sorter">The <see cref="Sorter{T}"/> to order by with</param>
		/// <returns>The <see cref="IQueryable{T}"/> with ordering</returns>
		/// <exception cref="SorterException">
		/// If the Sorter passed in has no properties added to it for sorting.
		/// </exception>
		public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> queryable, Sorter<TSource> sorter)
		{
			if (sorter.OrderedPropertySelectors.Count == 0)
				throw new SorterException("No properties have been configured for sorting in the specified sorter!");

			KeyValuePair<LambdaExpression, bool> propertySelectorPair = sorter.OrderedPropertySelectors[0];

			string methodName = propertySelectorPair.Value ? "OrderBy" : "OrderByDescending";
			IOrderedQueryable<TSource> newQueryable = ApplyQuery(propertySelectorPair.Key, methodName, queryable);

			for (int i = 1; i < sorter.OrderedPropertySelectors.Count; i++)
			{
				propertySelectorPair = sorter.OrderedPropertySelectors[i];
				methodName = propertySelectorPair.Value ? "ThenBy" : "ThenByDescending";
				newQueryable = ApplyQuery(propertySelectorPair.Key, methodName, newQueryable);
			}

			return newQueryable;
		}


		/// <summary>
		/// Applies the new query component to the <see cref="IQueryable{T}"/> and returns the result
		/// </summary>
		/// <typeparam name="TSource">The type being queried</typeparam>
		/// <param name="propertySelector">The property selector for the <c>OrderBy</c>/<c>ThenBy</c> method</param>
		/// <param name="callMethod">The method name to put in the query expression tree</param>
		/// <param name="queryable">The <see cref="IQueryable{T}"/></param>
		/// <returns>The resultant <see cref="IOrderedQueryable{T}"/></returns>
		private static IOrderedQueryable<TSource> ApplyQuery<TSource>(LambdaExpression propertySelector, string callMethod, IQueryable<TSource> queryable)
		{
			//We have to get the generic type arguments from the expression type itself, in case
			//it is of type Func<TSource, object> rather than Func<TSource, TParam>
			Type[] funcTypeArgs = propertySelector.GetType().GetGenericArguments();
			Type[] typeArgs = funcTypeArgs[0].GetGenericArguments();

			MethodCallExpression methodCallExpression = Expression.Call(typeof(Queryable), callMethod, typeArgs, queryable.Expression, propertySelector);
			return (IOrderedQueryable<TSource>)queryable.Provider.CreateQuery<TSource>(methodCallExpression);
		}
	}
}