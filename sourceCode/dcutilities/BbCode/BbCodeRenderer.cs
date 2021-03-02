using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using DigitallyCreated.Utilities.BbCode.Configuration;
using DigitallyCreated.Utilities.Bcl;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// The <see cref="BbCodeRenderer"/> is able to take some input text and using a set of
	/// <see cref="ITagDefinition"/>s, render the BB tags found in the input text into XHTML.
	/// </summary>
	/// <seealso cref="ITagDefinition"/>
	public class BbCodeRenderer
	{
		private readonly IEnumerable<ITagDefinition> _TagDefinitions;


		/// <summary>
		/// Constructor, creates a <see cref="BbCodeRenderer"/> that renders the tags whose 
		/// <see cref="ITagDefinition"/>s are specified by <paramref name="tagDefinitions"/>
		/// </summary>
		/// <param name="tagDefinitions">The <see cref="ITagDefinition"/>s of the tags that will be rendered</param>
		public BbCodeRenderer(IEnumerable<ITagDefinition> tagDefinitions)
		{
			_TagDefinitions = tagDefinitions;
		}


		/// <summary>
		/// Constructor, creates a <see cref="BbCodeRenderer"/> that renders the set of tags whose
		/// <see cref="ITagDefinition"/>s are specified in the configuration file.
		/// </summary>
		/// <param name="configTagDefSetName">
		/// The name of the tag definition set defined in the configuration file, or null if you want to use the
		/// default tag definition set.
		/// </param>
		public BbCodeRenderer(string configTagDefSetName)
		{
			if (configTagDefSetName == null)
				configTagDefSetName = BbCodeConfigurationSection.GetSection().TagDefinitionSets.DefaultSetValue;

			_TagDefinitions = (from tagDefSet in BbCodeConfigurationSection.GetSection().TagDefinitionSets.Cast<TagDefinitionConfigurationElementCollection>()
							   where tagDefSet.Name == configTagDefSetName
							   from tagDefElement in tagDefSet.Cast<TagDefinitionConfigurationElement>()
							   select tagDefElement)
									.Distinct(TagDefConfElementEqualityComparer.Instance)
									.Select(t => t.CreateTagDefinition())
									.ToList();
		}


		/// <summary>
		/// Constructor, creates a <see cref="BbCodeRenderer"/> that renders the set of tags whose
		/// <see cref="ITagDefinition"/>s are specified in the default tag definition set in the configuration file.
		/// </summary>
		public BbCodeRenderer()
			: this(null as string)
		{
		}


		/// <summary>
		/// Renders the BBCode tags in the input string
		/// </summary>
		/// <param name="input">The input string containing text mixed in with BBCode tags</param>
		/// <returns>The results of the render</returns>
		public RenderResults Render(string input)
		{
			//Identify the instances of BBCode tags in the input text
			IEnumerable<ITagInstance> tagInstances = (from definition in _TagDefinitions
			                                          from instance in definition.IdentifyTagInstances(input)
			                                          orderby instance.CharRange.StartAt ascending
			                                          select instance);

			//Validate the instances to ensure they respect their defined rules
			ValidationContext validationContext = new ValidationContext(input, tagInstances);
			ValidateAndRepairSyntax(validationContext);

			RenderContext renderContext = validationContext.ToRenderContext();

			//Instruct each tag definition to remove any whitespace they wish to remove around their
			//tag instances
			foreach (ITagDefinition tagDefinition in _TagDefinitions)
				tagDefinition.RemoveWhitespace(renderContext);

			//Instruct each tag definition to render all their tag instances
			foreach (ITagDefinition tagDefinition in _TagDefinitions)
				tagDefinition.Render(renderContext);

			//Escape any HTML in the input string and prepare the results
			return new RenderResults(HtmlEscapeAndInsertBrs(renderContext), renderContext.IsCacheable);
		}


		/// <summary>
		/// Validates the BBCode tags and ensures they respect the rules they define. For example, some tags
		/// disallow other tags inside them. This is where those rules are enforced and errant tags 
		/// (<see cref="ITagInstance"/>s) are removed.
		/// </summary>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		private void ValidateAndRepairSyntax(ValidationContext context)
		{
			for (int i = 0; i < context.Tags.Count; i++)
			{
				ITagInstance tagInstance = context.Tags[i];
				bool vetoed = CheckForPreviousTagOverlap(context, i);

				if (vetoed == false && tagInstance is IOpenTagInstance)
				{
					IOpenTagInstance openTagInstance = (IOpenTagInstance)tagInstance;

					//If any other tag vetoes this one, OR this tag vetoes itself
					vetoed = context.OpenTagStack.Any(t => t.CheckForVetoAgainstAnotherTag(openTagInstance, context)) ||
							 openTagInstance.CheckForSelfVeto(context);

					if (vetoed == false)
						context.OpenTagStack.Push(openTagInstance);
				}

				if (vetoed == false && tagInstance is ICloseTagInstance)
				{
					//If we find a close tag without any open tag, OR
					//if the current innermost tag (top of the stack) is not the open tag for this close tag
					vetoed = context.OpenTagStack.Any() == false ||
					         context.OpenTagStack.Peek().ParentDefinition != tagInstance.ParentDefinition;

					if (vetoed == false)
					{
						IOpenTagInstance openTagInstance = context.OpenTagStack.Pop();
						openTagInstance.CloseTag = (ICloseTagInstance)tagInstance;
					}
				}

				if (vetoed)
				{
					context.Tags.RemoveAt(i);
					i--;
				}
			}

			//Close any tags that were left open
			while (context.OpenTagStack.Any())
			{
				IOpenTagInstance openTagInstance = context.OpenTagStack.Peek();

				string tagText;
				ICloseTagInstance closeTag = openTagInstance.ParentDefinition.MakeCloseTagFor(openTagInstance, context.InputString.Length, out tagText);
				if (closeTag.CheckIfValidClose(context))
				{
					context.InputString.Append(tagText);
					context.Tags.Add(closeTag);
					openTagInstance.CloseTag = closeTag;
				}
				else
					context.Tags.Remove(openTagInstance);

				context.OpenTagStack.Pop();
			}
		}


		/// <summary>
		/// Checks if the current tag's character range overlaps the previous tag's character range
		/// </summary>
		/// <remarks>
		/// Checking backwards for every tag previous the current is unnecessary because each previous
		/// tag is vetoed if it overlaps with the tag before it.
		/// </remarks>
		/// <param name="context">The <see cref="ValidationContext"/></param>
		/// <param name="tagIndex">The index of the current tag in <see cref="ValidationContext.Tags"/></param>
		/// <returns>True if it does overlap, false otherwise</returns>
		private bool CheckForPreviousTagOverlap(ValidationContext context, int tagIndex)
		{
			ITagInstance tagInstance = context.Tags[tagIndex];

			if (tagIndex == 0)
				return false;

			ITagInstance previousTagInstance = context.Tags[tagIndex - 1];
			int endIndex = previousTagInstance.CharRange.StartAt + previousTagInstance.CharRange.Length - 1;
			return tagInstance.CharRange.StartAt <= endIndex;
		}


		/// <summary>
		/// HTML escape any text that hasn't been marked as an already escaped range during the render process
		/// and replace any newlines with &lt;br/&gt; tags.
		/// </summary>
		/// <param name="renderContext">The <see cref="RenderContext"/></param>
		/// <returns>The rendered string now HTML escaped</returns>
		private string HtmlEscapeAndInsertBrs(RenderContext renderContext)
		{
			string renderString = renderContext.GetRenderString();
			IEnumerable<CharRange> escapedCharRanges = (new[] { new CharRange(0, 0) }).Concat(renderContext.HtmlEscapedRanges).Concat(new[] { new CharRange(renderString.Length, 0) });
			IEnumerator<CharRange> enumerator = escapedCharRanges.GetEnumerator();
			StringBuilder builder = new StringBuilder(renderString);

			CharRange firstRange;
			enumerator.MoveNext();
			CharRange secondRange = enumerator.Current;
			enumerator.MoveNext();

			int indexOffset = 0;
			bool canEnumerateMore = true;
			while (canEnumerateMore)
			{
				firstRange = secondRange;
				secondRange = enumerator.Current;

				int escapeFromIndex = indexOffset + firstRange.StartAt + firstRange.Length;
				int escapeLength = indexOffset + secondRange.StartAt - escapeFromIndex;

				if (escapeLength > 0)
				{
					string strToEscape = builder.ToString(escapeFromIndex, escapeLength);
					string escapedString = WebUtility.HtmlEncode(strToEscape);
					builder.Remove(escapeFromIndex, escapeLength);
					builder.Insert(escapeFromIndex, escapedString);
					indexOffset += escapedString.Length - strToEscape.Length;
				}

				canEnumerateMore = enumerator.MoveNext();
			}

			//Insert brs
			builder.Replace("\r", String.Empty);
			builder.Replace("\n", "<br/>");

			return builder.ToString();
		}
	}
}