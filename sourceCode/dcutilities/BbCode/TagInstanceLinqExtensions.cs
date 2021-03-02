using System.Collections.Generic;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// Contains LINQ extensions that can help when traversing a list of <see cref="ITagInstance"/>s such
	/// as the <see cref="IBbStringContext.Tags"/> list.
	/// </summary>
	public static class TagInstanceLinqExtensions
	{
		/// <summary>
		/// Returns an <see cref="IEnumerable{T}"/> that only returns <see cref="ITagInstance"/>s that are at a
		/// certain depth on the tag stack that is created by watching tags open and close inside each other. This
		/// method must be used on a validated tag list.
		/// </summary>
		/// <remarks>
		/// The depth of the stack starts at 0. The first <see cref="IOpenTagInstance"/> read from 
		/// <paramref name="tagInstances"/> is considered to be at stack level 0, and if the next tag is another
		/// <see cref="IOpenTagInstance"/>, that tag is considered to be at stack level 1 and so on. Encountering a 
		/// <see cref="ICloseTagInstance"/> reduces the stack level by 1.
		/// </remarks>
		/// <param name="tagInstances">
		/// The <see cref="ITagInstance"/>s to read. They must be ordered by their 
		/// <see cref="ITagInstance.CharRange"/>s' <see cref="CharRange.StartAt"/>.
		/// </param>
		/// <param name="stackDepth">The stack depth at which the returned tags must be at</param>
		/// <returns>
		/// An <see cref="IEnumerable{T}"/> of the tags that are at the specified <paramref name="stackDepth"/>,
		/// ordered by their <see cref="ITagInstance.CharRange"/>s' <see cref="CharRange.StartAt"/>.
		/// </returns>
		public static IEnumerable<ITagInstance> WhereStackDepthIs(this IEnumerable<ITagInstance> tagInstances, int stackDepth)
		{
			int currentStackDepth = 0;

			foreach (ITagInstance tagInstance in tagInstances)
			{
				if (tagInstance is IOpenTagInstance)
				{
					if (currentStackDepth == stackDepth)
						yield return tagInstance;

					if (tagInstance is ICloseTagInstance == false)
						currentStackDepth++;
				}
				else
				{
					currentStackDepth--;

					if (currentStackDepth == stackDepth)
						yield return tagInstance;
				}
			}
		}
	}
}