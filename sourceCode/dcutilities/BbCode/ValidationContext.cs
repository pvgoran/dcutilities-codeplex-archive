using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// Contains the current state of the tag validation process
	/// </summary>
	public class ValidationContext
	{
		private readonly List<ITagInstance> _Tags;
		private readonly Stack<IOpenTagInstance> _OpenTagStack;
		private readonly StringBuilder _InputString;

		/// <summary>
		/// The <see cref="ITagInstance"/>s found in the <see cref="InputString"/>, ordered by 
		/// their <see cref="ITagInstance.CharRange"/>s' <see cref="CharRange.StartAt"/>.
		/// </summary>
		/// <remarks>
		/// The validation process moves through these tags in order and removes invalid ones as it goes.
		/// </remarks>
		public IList<ITagInstance> Tags { get { return _Tags; } }

		/// <summary>
		/// The stack of the tags that are currently open at this point in the validation process
		/// </summary>
		public Stack<IOpenTagInstance> OpenTagStack { get { return _OpenTagStack; } }

		/// <summary>
		/// The input BBCode string
		/// </summary>
		public StringBuilder InputString { get { return _InputString; } }


		/// <summary>
		/// Constructor, creates a <see cref="ValidationContext"/>
		/// </summary>
		/// <param name="inputString">The input BBCode string</param>
		/// <param name="tags">
		/// The <see cref="ITagInstance"/>s found in the <see cref="InputString"/>, ordered by 
		/// their <see cref="ITagInstance.CharRange"/>s' <see cref="CharRange.StartAt"/>.
		/// </param>
		public ValidationContext(string inputString, IEnumerable<ITagInstance> tags)
		{
			_Tags = tags.ToList();
			_OpenTagStack = new Stack<IOpenTagInstance>();
			_InputString = new StringBuilder(inputString);
		}


		/// <summary>
		/// Converts this to a <see cref="RenderContext"/>
		/// </summary>
		/// <returns>The <see cref="RenderContext"/> created</returns>
		internal RenderContext ToRenderContext()
		{
			return new RenderContext(_InputString.ToString(), _Tags);
		}
	}
}