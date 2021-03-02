using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// The <see cref="RenderContext"/> implements <see cref="IRenderContext"/> and 
	/// <see cref="IWhitespaceRemovalContext"/> and is used (through those interfaces) by 
	/// <see cref="ITagDefinition"/>s to perform rendering tasks.
	/// </summary>
	/// <seealso cref="IWhitespaceRemovalContext"/>
	/// <seealso cref="IRenderContext"/>
	/// <seealso cref="ITagDefinition"/>
	public class RenderContext : IRenderContext, IWhitespaceRemovalContext
	{
		private static readonly Regex _BeforeTagWhitespaceRegex = new Regex(@"(?<nl>\r?\n[\f\t\v\x85 ]*)*", RegexOptions.RightToLeft);
		private static readonly Regex _AfterTagWhitespaceRegex = new Regex(@"(?<nl>[\f\t\v\x85 ]*\r?\n)*");
		private readonly List<ITagInstance> _Tags;
		private readonly IList<ITagInstance> _ReadOnlyTags;
		private readonly StringBuilder _RenderString;
		private readonly SortedSet<CharRange> _HtmlEscapedRanges;
		private bool _IsCacheable;

		/// <summary>
		/// A read only list of <see cref="ITagInstance"/>s found in the input BBCode string. The tags are ordered
		/// by the <see cref="CharRange.StartAt"/> property of their <see cref="ITagInstance.CharRange"/>
		/// </summary>
		public IList<ITagInstance> Tags { get { return _ReadOnlyTags; } }

		/// <summary>
		/// Defines a series of <see cref="CharRange"/> that have already been HTML escaped (or simply contain
		/// HTML tags you don't want to be escaped). The series is ordered by <see cref="CharRange.StartAt"/>
		/// </summary>
		public IEnumerable<CharRange> HtmlEscapedRanges { get { return _HtmlEscapedRanges; } }

		/// <summary>
		/// Returns whether the rendering result is cacheable (ie. it will always return the same
		/// output given the same <see cref="ITagDefinition"/>s and the same input).
		/// </summary>
		/// <remarks>
		/// <see cref="ITagDefinition"/>s should call <see cref="IRenderContext.RegisterRenderCacheability"/> if they wish
		/// to register that they are rendering something that is not cacheable. If one 
		/// <see cref="ITagDefinition"/> registers that it's doing non-cacheable stuff, it makes the entire
		/// render string uncacheable.
		/// </remarks>
		public bool IsCacheable { get { return _IsCacheable; } }


		/// <summary>
		/// Constructor, creates a <see cref="RenderContext"/> for the specified input string and tags.
		/// </summary>
		/// <param name="inputString">The input string</param>
		/// <param name="tags">
		/// The validated tags identified in the input string ordered by their <see cref="ITagInstance.CharRange"/>s
		/// </param>
		public RenderContext(string inputString, List<ITagInstance> tags)
		{
			_Tags = tags;
			_ReadOnlyTags = _Tags.AsReadOnly();
			_HtmlEscapedRanges = new SortedSet<CharRange>();
			_RenderString = new StringBuilder(inputString);
			_IsCacheable = true;
		}


		/// <summary>
		/// Gets the entire render string as it looks at this point in time. This means it may be in a partially
		/// rendered (or not rendered at all) form.
		/// </summary>
		/// <returns>The whole render string</returns>
		public string GetRenderString()
		{
			return _RenderString.ToString();
		}


		/// <summary>
		/// Gets a subset of the render string as it looks at this point in time. This means it may be in a partially
		/// rendered (or not rendered at all) form.
		/// </summary>
		/// <param name="startIndex">The index to start at</param>
		/// <param name="length">The length of the substring to return</param>
		/// <returns>The subset of the render string</returns>
		public string GetRenderString(int startIndex, int length)
		{
			return _RenderString.ToString(startIndex, length);
		}


		/// <summary>
		/// Gets the subset of the render string that is between the specified <see cref="ITagInstance"/>s as it
		/// looks at this point in time. This means i may be in a partially rendering (or not rendered at all) form.
		/// </summary>
		/// <param name="afterTag">The first <see cref="ITagInstance"/></param>
		/// <param name="untilTag">The second <see cref="ITagInstance"/></param>
		/// <returns>The subset of the render string</returns>
		public string GetRenderString(ITagInstance afterTag, ITagInstance untilTag)
		{
			int startIndex = afterTag.CharRange.StartAt + afterTag.CharRange.Length;
			return GetRenderString(startIndex, untilTag.CharRange.StartAt - startIndex);
		}


		/// <summary>
		/// Should be called by <see cref="ITagDefinition"/>s that render things that are not cacheable. For
		/// example, an <see cref="ITagDefinition"/> that reads from a database and renders something produces
		/// non-cacheable output (as the output would change if the database changed).
		/// </summary>
		/// <remarks>
		/// If an <see cref="ITagDefinition"/> does not call this method, it is assumed that its output is
		/// cacheable.
		/// </remarks>
		/// <param name="cacheable">Whether or not the <see cref="ITagDefinition"/>'s render is cacheable</param>
		public void RegisterRenderCacheability(bool cacheable)
		{
			_IsCacheable &= cacheable;
		}


		/// <summary>
		/// Replaces the range of characters defined by the <paramref name="tagInstance"/> with the specified
		/// <paramref name="replacementText"/>.
		/// </summary>
		/// <remarks>
		/// If the <paramref name="replacementText"/> is longer or shorter than the range of characters defined by
		/// the <paramref name="tagInstance"/>, the <see cref="IBbStringContext.Tags"/> and 
		/// <see cref="IRenderContext.HtmlEscapedRanges"/> are automatically shifted to reflect the changes.
		/// </remarks>
		/// <param name="tagInstance">The <see cref="ITagInstance"/> whose character range will be replaced</param>
		/// <param name="replacementText">The replacement text</param>
		/// <param name="registerAsHtmlEscaped">
		/// Whether or not to register the rendered character range as an escaped character range (see
		/// <see cref="IRenderContext.HtmlEscapedRanges"/>).
		/// </param>
		public void RenderTag(ITagInstance tagInstance, string replacementText, bool registerAsHtmlEscaped)
		{
			Render(tagInstance.CharRange.StartAt, tagInstance.CharRange.Length, replacementText, registerAsHtmlEscaped);
			tagInstance.Rendered = true;
		}


		/// <summary>
		/// Replaces the specified range of characters with the specified <paramref name="replacementText"/>. Note
		/// that the range of characters cannot overlap with a range of characters that is owned by a 
		/// <see cref="ITagInstance"/> from <see cref="IBbStringContext.Tags"/>.
		/// </summary>
		/// <param name="startAtIndex">The index of the first character in the range</param>
		/// <param name="length">The length of the character range</param>
		/// <param name="replacementText">The replacement text</param>
		/// <param name="registerAsHtmlEscaped">
		/// Whether or not to register the rendered character range as an escaped character range (see
		/// <see cref="IRenderContext.HtmlEscapedRanges"/>).
		/// </param>
		public void RenderNonTagRange(int startAtIndex, int length, string replacementText, bool registerAsHtmlEscaped)
		{
			if (length < 0)
				throw new ArgumentException("length must be at least 0", "length");

			//This validation check could be cut for efficiency... but would hurt developers' debugging efforts
			int endIndex = length == 0 ? startAtIndex + length - 1 : startAtIndex;
			foreach (ITagInstance tagInstance in _Tags)
			{
				if (endIndex < tagInstance.CharRange.StartAt)
					continue;
				if (startAtIndex >= tagInstance.CharRange.StartAt + tagInstance.CharRange.Length)
					break;

				throw new InvalidOperationException("Trying to render a non-tag range over a range in which a tag actually exists!");
			}

			Render(startAtIndex, length, replacementText, registerAsHtmlEscaped);
		}


		/// <summary>
		/// Replaces the specified range of characters with the the specified <paramref name="replacementText"/>.
		/// </summary>
		/// <param name="startAtIndex">The index of the first character in the range</param>
		/// <param name="length">The length of the character range</param>
		/// <param name="replacementText">The replacement text</param>
		/// <param name="registerAsHtmlEscaped">
		/// Whether or not to register the rendered character range as an escaped character range (see
		/// <see cref="IRenderContext.HtmlEscapedRanges"/>).
		/// </param>
		private void Render(int startAtIndex, int length, string replacementText, bool registerAsHtmlEscaped)
		{
			_RenderString.Remove(startAtIndex, length);
			_RenderString.Insert(startAtIndex, replacementText);

			CharRange insertedCharRange;
			if (length == 0)
				insertedCharRange = new CharRange(startAtIndex, replacementText.Length);
			else
				insertedCharRange = new CharRange(startAtIndex + length - 1, replacementText.Length - length);

			ShiftCharRangesWithInsertedCharRange(_HtmlEscapedRanges, insertedCharRange);
			if (registerAsHtmlEscaped)
				_HtmlEscapedRanges.Add(new CharRange(startAtIndex, replacementText.Length));
			ShiftCharRangesWithInsertedCharRange(_Tags.Select(t => t.CharRange), insertedCharRange);
		}


		/// <summary>
		/// Shifts the <see cref="CharRange"/>s the appropriate amount that would be caused by inserting the character
		/// range defined by <paramref name="insertedCharRange"/>.
		/// </summary>
		/// <param name="charRanges">
		/// The <see cref="CharRange"/>s ordered by their <see cref="CharRange.StartAt"/>
		/// </param>
		/// <param name="insertedCharRange">The inserted character range</param>
		private void ShiftCharRangesWithInsertedCharRange(IEnumerable<CharRange> charRanges, CharRange insertedCharRange)
		{
			if (insertedCharRange.Length == 0)
				return;

			foreach (CharRange charRange in charRanges)
			{
				int indexAfter = charRange.StartAt + charRange.Length;

				//Characters were inserted after the range (skip)
				if (indexAfter <= insertedCharRange.StartAt)
					continue;

				//Characters were inserted inside the char range (makes the range length change)
				if (charRange.Length > 0 && //Cant insert inside a 0 length range
					indexAfter - 1 >= insertedCharRange.StartAt &&
					charRange.StartAt < insertedCharRange.StartAt)
					charRange.Length += insertedCharRange.Length;
				//Characters were inserted before the char range (moves the range forward or backward)
				else
					charRange.StartAt += insertedCharRange.Length;
			}
		}


		/// <summary>
		/// Returns an <see cref="IEnumerable{T}"/> of the tags including and following the specified 
		/// <paramref name="tagInstance"/> in the ordered sequence of tags defined by <see cref="IBbStringContext.Tags"/>.
		/// </summary>
		/// <param name="tagInstance">The <see cref="ITagInstance"/> to return the tags after</param>
		/// <returns>
		/// The <see cref="ITagInstance"/>s starting with and after <paramref name="tagInstance"/> 
		/// from the <see cref="IBbStringContext.Tags"/> list.
		/// </returns>
		public IEnumerable<ITagInstance> GetTagsOnwardsFrom(ITagInstance tagInstance)
		{
			int index = _Tags.BinarySearch(tagInstance, TagInstanceComparer.Instance);
			if (index < 0)
				throw new ArgumentException("tagInstance must be in the Tags enumeration", "tagInstance");

			return EnumerateFromIndex(_Tags, index);
		}


		/// <summary>
		/// Returns an <see cref="IEnumerable{T}"/> of the tags following the specified 
		/// <paramref name="tagInstance"/> in the ordered sequence of tags defined by <see cref="IBbStringContext.Tags"/>.
		/// </summary>
		/// <param name="tagInstance">The <see cref="ITagInstance"/> to return the tags after</param>
		/// <returns>
		/// The <see cref="ITagInstance"/>s after (and not including) <paramref name="tagInstance"/> 
		/// from the <see cref="IBbStringContext.Tags"/> list.
		/// </returns>
		public IEnumerable<ITagInstance> GetTagsAfter(ITagInstance tagInstance)
		{
			int index = _Tags.BinarySearch(tagInstance, TagInstanceComparer.Instance);
			if (index < 0)
				throw new ArgumentException("tagInstance must be in the Tags enumeration", "tagInstance");

			return EnumerateFromIndex(_Tags, index + 1);
		}


		/// <summary>
		/// Enumerates from the specified index in the <paramref name="list"/> onwards.
		/// </summary>
		/// <param name="list">The list to enumerate from</param>
		/// <param name="index">The index to start at</param>
		/// <returns>An <see cref="IEnumerable{T}"/></returns>
		private IEnumerable<ITagInstance> EnumerateFromIndex(IList<ITagInstance> list, int index)
		{
			for (int i = index; i < list.Count; i++)
			{
				yield return list[i];
			}
		}


		/// <summary>
		/// Removes up to a number of newlines (and whitespace between them) that are found before the specified 
		/// <paramref name="tagInstance"/>.
		/// </summary>
		/// <remarks>
		/// If <paramref name="numberOfNewlines"/> is higher than the actual number of newlines before the
		/// <paramref name="tagInstance"/>, only the actual number of newlines will be removed.
		/// </remarks>
		/// <param name="tagInstance">The tag instance to remove whitespace from before</param>
		/// <param name="numberOfNewlines">The number of newlines to remove</param>
		public void RemoveWhitespaceBeforeTag(ITagInstance tagInstance, int numberOfNewlines)
		{
			if (numberOfNewlines <= 0)
				throw new ArgumentOutOfRangeException("numberOfNewlines", "numberOfNewlines must be greater than 0.");

			if (tagInstance.CharRange.StartAt == 0)
				return;

			Match match = _BeforeTagWhitespaceRegex.Match(GetRenderString(), tagInstance.CharRange.StartAt);
			if (match.Success == false)
				return;

			CaptureCollection captures = match.Groups["nl"].Captures;
			if (captures.Count <= numberOfNewlines)
				RenderNonTagRange(match.Index, match.Length, String.Empty, false);
			else
			{
				int startAt = captures[numberOfNewlines - 1].Index;
				int length = captures.Cast<Capture>().Take(numberOfNewlines).Sum(c => c.Length);
				RenderNonTagRange(startAt, length, String.Empty, false);
			}
		}


		/// <summary>
		/// Removes up to a number of newlines (and whitespace between them) that are found after the specified 
		/// <paramref name="tagInstance"/>.
		/// </summary>
		/// <remarks>
		/// If <paramref name="numberOfNewlines"/> is higher than the actual number of newlines after the
		/// <paramref name="tagInstance"/>, only the actual number of newlines will be removed.
		/// </remarks>
		/// <param name="tagInstance">The tag instance to remove whitespace from after</param>
		/// <param name="numberOfNewlines">The number of newlines to remove</param>
		public void RemoveWhitespaceAfterTag(ITagInstance tagInstance, int numberOfNewlines)
		{
			if (numberOfNewlines <= 0)
				throw new ArgumentOutOfRangeException("numberOfNewlines", "numberOfNewlines must be greater than 0.");

			if (tagInstance.CharRange.StartAt >= _RenderString.Length)
				return;

			int startAt = tagInstance.CharRange.StartAt + tagInstance.CharRange.Length;
			Match match = _AfterTagWhitespaceRegex.Match(GetRenderString(), startAt);
			if (match.Success == false)
				return;

			CaptureCollection captures = match.Groups["nl"].Captures;
			if (captures.Count <= numberOfNewlines)
				RenderNonTagRange(match.Index, match.Length, String.Empty, false);
			else
			{
				int length = captures.Cast<Capture>().Take(numberOfNewlines).Sum(c => c.Length);
				RenderNonTagRange(startAt, length, String.Empty, false);
			}
		}


		/// <summary>
		/// An <see cref="IComparer{T}"/> that compares <see cref="ITagInstance"/>s by their
		/// <see cref="ITagInstance.CharRange"/>'s <see cref="CharRange.StartAt"/>.
		/// </summary>
		private class TagInstanceComparer : IComparer<ITagInstance>
		{
			public static TagInstanceComparer Instance = new TagInstanceComparer();


			/// <summary>
			/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
			/// </summary>
			/// <param name="x">The first object to compare.</param>
			/// <param name="y">The second object to compare.</param>
			/// <returns>
			/// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.
			/// </returns>
			public int Compare(ITagInstance x, ITagInstance y)
			{
				return x.CharRange.StartAt.CompareTo(y.CharRange.StartAt);
			}
		}
	}
}