using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// This class is an <see cref="ITagDefinition"/> and makes it easy for inheritors to implement simple BBcode
	/// tags where the BBcode tags are directly replaced by matching XHTML tags upon rendering
	/// </summary>
	/// <remarks>
	/// If this class is too restrictive, consider inheriting from <see cref="OpenCloseTagDefinition"/> directly.
	/// </remarks>
	/// <seealso cref="ITagInstance"/>
	/// <seealso cref="OpenCloseTagDefinition"/>
	public class SimpleTagDefinition : OpenCloseTagDefinition
	{
		private readonly Func<IOpenTagInstance, string> _ReplacementOpenTagFactory;
		private readonly Func<ICloseTagInstance, string> _ReplacementCloseTagFactory;
		private readonly bool _CacheableRender;

		/// <summary>
		/// A function that takes an <see cref="IOpenTagInstance"/> and creates some XHTML which is returned
		/// and replaced in place of the <see cref="IOpenTagInstance"/> in the render string.
		/// </summary>
		protected Func<IOpenTagInstance, string> ReplacementOpenTagFactory { get { return _ReplacementOpenTagFactory; } }

		/// <summary>
		/// A function that takes an <see cref="ICloseTagInstance"/> and creates some XHTML which is returned
		/// and replaced in place of the <see cref="ICloseTagInstance"/> in the render string.
		/// </summary>
		protected Func<ICloseTagInstance, string> ReplacementCloseTagFactory { get { return _ReplacementCloseTagFactory; } }


		/// <summary>
		/// Constructor, creates a <see cref="SimpleTagDefinition"/>
		/// </summary>
		/// <param name="openTagRegex">
		/// The <see cref="Regex"/> that is used to find <see cref="IOpenTagInstance"/>s
		/// </param>
		/// <param name="replacementOpenTagFactory">
		/// A function that takes an <see cref="IOpenTagInstance"/> and creates some XHTML which is returned
		/// and replaced in place of the <see cref="IOpenTagInstance"/> in the render string.
		/// </param>
		/// <param name="closeTagRegex">
		/// The <see cref="Regex"/> that is used to find <see cref="ICloseTagInstance"/>s
		/// </param>
		/// <param name="replacementCloseTagFactory">
		/// A function that takes an <see cref="ICloseTagInstance"/> and creates some XHTML which is returned
		/// and replaced in place of the <see cref="ICloseTagInstance"/> in the render string.
		/// </param>
		/// <param name="closeTag">The BBCode close tag for this tag definition</param>
		/// <param name="openTagVetoRulesSet">
		/// Collections of <see cref="IVetoRule"/>s that are used by the 
		/// <see cref="IOpenTagInstance.CheckForVetoAgainstAnotherTag"/> and 
		/// <see cref="IOpenTagInstance.CheckForSelfVeto"/> methods to determine tag validity.
		/// </param>
		/// <param name="closeTagVetoRules">
		/// A collection of <see cref="IVetoRule"/>s that are used by the 
		/// <see cref="ICloseTagInstance.CheckIfValidClose"/> method to determine tag validity.
		/// </param>
		/// <param name="rendersToInlineElement">
		/// Whether or not the tags will render to a inline XHTML elements (such as a span tags) or not (such as
		/// blockquote tags).
		/// </param>
		/// <param name="cacheableRender">
		/// True if the results of using <paramref name="replacementOpenTagFactory"/> and
		/// <paramref name="replacementCloseTagFactory"/> result in a cacheable render, false otherwise. For more
		/// information about cacheable renders, see <see cref="RenderContext.IsCacheable"/>.
		/// </param>
		public SimpleTagDefinition(Regex openTagRegex, Func<IOpenTagInstance, string> replacementOpenTagFactory, Regex closeTagRegex, Func<ICloseTagInstance, string> replacementCloseTagFactory, string closeTag, OpenTagVetoRulesSet openTagVetoRulesSet, IEnumerable<IVetoRule> closeTagVetoRules, bool rendersToInlineElement, bool cacheableRender)
			: base(openTagRegex, closeTagRegex, closeTag, openTagVetoRulesSet, closeTagVetoRules, rendersToInlineElement)
		{
			_ReplacementOpenTagFactory = replacementOpenTagFactory;
			_ReplacementCloseTagFactory = replacementCloseTagFactory;
			_CacheableRender = cacheableRender;
		}


		/// <summary>
		/// Causes the <see cref="ITagDefinition"/> to render all of its <see cref="ITagInstance"/>s into XHTML.
		/// </summary>
		/// <remarks>
		/// This method is always called after <see cref="ITagDefinition.RemoveWhitespace"/> is called on any 
		/// <see cref="ITagDefinition"/>. The order in which this method is called across the different 
		/// <see cref="ITagDefinition"/>s is undefined.
		/// </remarks>
		/// <param name="context">
		/// The <see cref="IRenderContext"/> contains all the <see cref="ITagInstance"/>s, the input text,
		/// and methods that allow for the rendering of <see cref="ITagInstance"/>s.
		/// </param>
		public override void Render(IRenderContext context)
		{
			IEnumerable<IOpenTagInstance> tagInstances = context.Tags.Where(t => t.ParentDefinition == this)
			                                                         .OfType<IOpenTagInstance>();

			foreach (IOpenTagInstance tagInstance in tagInstances)
			{
				string replacementTag = ReplacementOpenTagFactory(tagInstance);
				context.RenderTag(tagInstance, replacementTag, true);

				replacementTag = ReplacementCloseTagFactory(tagInstance.CloseTag);
				context.RenderTag(tagInstance.CloseTag, replacementTag, true);
			}

			context.RegisterRenderCacheability(_CacheableRender);
		}
	}
}