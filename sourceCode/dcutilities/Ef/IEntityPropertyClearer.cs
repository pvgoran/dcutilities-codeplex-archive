using System;


namespace DigitallyCreated.Utilities.Ef
{
	/// <summary>
	/// An <see cref="IEntityPropertyClearer"/> is able to clear all non-scalar properties on an entity.
	/// </summary>
	/// <remarks>
	/// This interface is used by 
	/// <see cref="EfUtil.ClearNonScalarProperties(System.Data.Objects.DataClasses.EntityObject,bool)"/> 
	/// when it generates a type to do the property clearing. It should not be used by 3rd party users of this library.
	/// </remarks>
	public interface IEntityPropertyClearer
	{
		/// <summary>
		/// The type of entity that this clearer can clear
		/// </summary>
		Type EntityType { get; }


		/// <summary>
		/// Clear the entity object's non-scalar properties
		/// </summary>
		/// <param name="entity">The entity</param>
		/// <param name="clearEntityKeyProperties">If entity key properties should be cleared too</param>
		/// <exception cref="InvalidCastException">
		/// If <paramref name="entity"/> cannot be casted to <see cref="EntityType"/>
		/// </exception>
		void Clear(object entity, bool clearEntityKeyProperties);
	}
}