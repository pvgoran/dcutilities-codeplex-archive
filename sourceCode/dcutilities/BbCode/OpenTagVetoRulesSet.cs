using System.Collections.Generic;


namespace DigitallyCreated.Utilities.BbCode
{
	/// <summary>
	/// Contains the <see cref="IVetoRule"/> sets that an <see cref="OpenTagInstance"/> uses
	/// </summary>
	public class OpenTagVetoRulesSet
	{
		/// <summary>
		/// The <see cref="IVetoRule"/>s used for the <see cref="IOpenTagInstance.CheckForVetoAgainstAnotherTag"/>
		/// method
		/// </summary>
		public IEnumerable<IVetoRule> OtherTagVetoRules { get; set; }

		/// <summary>
		/// The <see cref="IVetoRule"/>s used for the <see cref="IOpenTagInstance.CheckForSelfVeto"/> method
		/// </summary>
		public IEnumerable<IVetoRule> SelfVetoRules { get; set; }
	}
}