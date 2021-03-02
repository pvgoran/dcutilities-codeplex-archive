using System;
using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;


namespace DigitallyCreated.Utilities.MsBuild
{
	/// <summary>
	/// This task uses the command-line hg.exe executable to determine what the current version hash of
	/// the specified repository is.
	/// </summary>
	/// <remarks>
	/// This class requires the patch to hg.exe to be in the system's PATH environment variable.
	/// </remarks>
	public class MercurialVersionTask : Task
	{
		/// <summary>
		/// The path to the repository
		/// </summary>
		[Required]
		public string RepositoryPath { get; set; }

		/// <summary>
		/// The version hash of the repository
		/// </summary>
		[Output]
		public string MercurialVersion { get; set; }


		/// <summary>
		/// Executes the task.
		/// </summary>
		/// <returns>
		/// true if the task successfully executed; otherwise, false.
		/// </returns>
		public override bool Execute()
		{
			try
			{
				MercurialVersion = GetMercurialVersion(RepositoryPath);
				Log.LogMessage(MessageImportance.Low, String.Format("Mercurial revision for repository \"{0}\" is {1}", RepositoryPath, MercurialVersion));
				return true;
			}
			catch (Exception e)
			{
				Log.LogError("Could not get the mercurial revision, unhandled exception occurred!");
				Log.LogErrorFromException(e, true, true, RepositoryPath);
				return false;
			}
		}


		/// <summary>
		/// Gets the version hash of the specified repository
		/// </summary>
		/// <param name="repositoryPath">The path to the repository</param>
		/// <returns>The version hash</returns>
		private string GetMercurialVersion(string repositoryPath)
		{
			Process hg = new Process();
			hg.StartInfo.UseShellExecute = false;
			hg.StartInfo.RedirectStandardError = true;
			hg.StartInfo.RedirectStandardOutput = true;
			hg.StartInfo.CreateNoWindow = true;
			hg.StartInfo.FileName = "hg";
			hg.StartInfo.Arguments = "id";
			hg.StartInfo.WorkingDirectory = repositoryPath;
			hg.Start();

			string output = hg.StandardOutput.ReadToEnd().Trim();
			string error = hg.StandardError.ReadToEnd().Trim();

			Log.LogMessage(MessageImportance.Low, "hg.exe Standard Output: {0}", output);
			Log.LogMessage(MessageImportance.Low, "hg.exe Standard Error: {0}", error);

			hg.WaitForExit();

			if (String.IsNullOrEmpty(error) == false)
				throw new Exception(String.Format("hg.exe error: {0}", error));

			string[] tokens = output.Split(' ');
			return tokens[0];
		}
	}
}