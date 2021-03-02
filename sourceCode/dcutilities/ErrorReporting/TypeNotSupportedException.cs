using System;

namespace DigitallyCreated.Utilities.ErrorReporting
{
	/// <summary>
	/// Indicates that a particular type is not supported by an object of a particular type
	/// </summary>
	public class TypeNotSupportedException : Exception
	{
		private readonly Type _NotSupportedType;
		private readonly Type _NotSupportedBy;


		/// <summary>
		/// Constructor, creates a <see cref="TypeNotSupportedException"/> exception
		/// </summary>
		/// <param name="notSupportedType">The type that is not supported</param>
		/// <param name="notSupportedBy">
		/// The type of object that doesn't support <paramref name="notSupportedType"/>
		/// </param>
		public TypeNotSupportedException(Type notSupportedType, Type notSupportedBy)
			: base(notSupportedType.FullName + " is not supported by " + notSupportedBy.FullName)
		{
			_NotSupportedType = notSupportedType;
			_NotSupportedBy = notSupportedBy;
		}


		/// <summary>
		/// The type of object that doesn't support <see cref="NotSupportedType"/>
		/// </summary>
		public Type NotSupportedBy
		{
			get { return _NotSupportedBy; }
		}


		/// <summary>
		/// The type that is not supported
		/// </summary>
		public Type NotSupportedType
		{
			get { return _NotSupportedType; }
		}
	}
}
