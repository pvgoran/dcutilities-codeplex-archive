using System;
using System.Linq;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// This <see cref="IVetoRule"/> enforces that no tag must be nested inside the tag that uses
	/// this rule, except for the ones created by the specified <see cref="ITagDefinition"/>s.
	/// </summary>
	public class MustNotNestAnyTagsInMeExceptVetoRule : IVetoRule
	{
		private readonly Type[] _AllowableTagDefinitionTypes;

		/// <summary>
		/// Constructor, creates a <see cref="MustNotNestAnyTagsInMeExceptVetoRule"/>
		/// </summary>
		/// <param name="allowableTagDefinitionTypes">
		/// The <see cref="Type"/>s of the <see cref="ITagDefinition"/> whose <see cref="ITagInstance"/>s
		/// are allowed to nest inside the tag that uses this rule.
		/// </param>
		public MustNotNestAnyTagsInMeExceptVetoRule(params Type[] allowableTagDefinitionTypes)
		{
			_AllowableTagDefinitionTypes = allowableTagDefinitionTypes;
			foreach (Type type in allowableTagDefinitionTypes)
			{
				if (typeof(ITagDefinition).IsAssignableFrom(type) == false)
					throw new ArgumentException(type.Name + " is not an ITagDefinition", "allowableTagDefinitionTypes");
			}
		}


		/// <summary>
		/// Makes an assessment as to whether <paramref name="tagInstance"/> is a valid tag
		/// </summary>
		/// <param name="tagInstance">The <see cref="ITagInstance"/></param>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <returns>True if the tag is not valid should be vetoed, false otherwise</returns>
		public bool CheckForVeto(ITagInstance tagInstance, ValidationContext context)
		{
			return _AllowableTagDefinitionTypes.Any(t => t == tagInstance.ParentDefinition.GetType()) == false;
		}
	}
}