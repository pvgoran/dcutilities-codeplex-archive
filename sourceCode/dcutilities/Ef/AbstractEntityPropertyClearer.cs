using System;
using System.Data.Objects.DataClasses;


namespace DigitallyCreated.Utilities.Ef
{
	/// <summary>
	/// Abstract class that allows for easier implementation of the <see cref="IEntityPropertyClearer"/>
	/// interface.
	/// </summary>
	/// <typeparam name="TEntity">The entity type</typeparam>
	public abstract class AbstractEntityPropertyClearer<TEntity> : IEntityPropertyClearer
		where TEntity : EntityObject
	{
		/// <summary>
		/// The type of entity that this clearer can clear
		/// </summary>
		public Type EntityType
		{
			get { return typeof(TEntity); }
		}


		/// <summary>
		/// Clear the entity object's non-scalar properties
		/// </summary>
		/// <param name="entity">The entity</param>
		/// <param name="clearEntityKeyProperties">If entity key properties should be cleared too</param>
		/// <exception cref="InvalidCastException">
		/// If <paramref name="entity"/> cannot be casted to <see cref="IEntityPropertyClearer.EntityType"/>
		/// </exception>
		public void Clear(object entity, bool clearEntityKeyProperties)
		{
			if (entity is TEntity)
				ClearEntity((TEntity)entity, clearEntityKeyProperties);
		}


		/// <summary>
		/// Clear the entity object's non-scalar properties
		/// </summary>
		/// <param name="entity">The entity</param>
		/// <param name="clearEntityKeyProperties">If entity key properties should be cleared too</param>
		protected abstract void ClearEntity(TEntity entity, bool clearEntityKeyProperties);
	}
}