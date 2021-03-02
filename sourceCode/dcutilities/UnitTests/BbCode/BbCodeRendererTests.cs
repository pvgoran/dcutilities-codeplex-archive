using DigitallyCreated.Utilities.BbCode.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DigitallyCreated.Utilities.BbCode
{
	[TestClass]
	public class BbCodeRendererTests
	{

		/// <summary>
		/// Gets or sets the test context which provides information about and functionality for the current
		/// test run.
		///</summary>
		public TestContext TestContext { get; set; }


		[TestMethod]
		public void TestBoldAndItalics()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[b]Wow[/b], this is [i]really[/i] [b]awesome[/b]!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"<strong>Wow</strong>, this is <em>really</em> <strong>awesome</strong>!", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestUnclosedTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[b]Wow, this is [i]really awesome!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"<strong>Wow, this is <em>really awesome!</em></strong>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestCloseTagsWithoutOpenTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render(@"Wow[/b], this is really[/i] awesome!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"Wow[/b], this is really[/i] awesome!", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestUnopenedCloseTagsInsideOpenTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[b]Wow[/b], this is [b]really[/i] awesome[/b]!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"<strong>Wow</strong>, this is <strong>really[/i] awesome</strong>!", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestEscapeTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ItalicsTagDefinition(), new EscapeTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[esc][b]Wow[/b], this is [i]really[/i][/esc] [b]awesome[/b]!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"[b]Wow[/b], this is [i]really[/i] <strong>awesome</strong>!", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestEscapeHalfATagPair()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ItalicsTagDefinition(), new EscapeTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[esc][b][/esc]Wow[/b], this is [esc][i][/esc]really[/i] [b]awesome[/b]!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"[b]Wow[/b], this is [i]really[/i] <strong>awesome</strong>!", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestBoldAndItalicsNoSelfNesting()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[b]Wow, [b]this[/b] is [i][i]really[/i] [/i] awesome[/b]!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"<strong>Wow, [b]this</strong> is <em>[i]really</em> [/i] awesome[/b]!", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestUrlTagWithInlineUrl()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new UrlTagDefinition() });
			RenderResults renderResults = renderer.Render(@"Wow, this site ([url]http://www.google.com[/url]) is really [b]awesome[/b]!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("Wow, this site (<a href=\"http://www.google.com\">http://www.google.com</a>) is really <strong>awesome</strong>!", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestUrlTagWithInTagUrl()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new UrlTagDefinition() });
			RenderResults renderResults = renderer.Render(@"Wow, [url=http://www.google.com]this site[/url] is really [b]awesome[/b]!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("Wow, <a href=\"http://www.google.com\">this site</a> is really <strong>awesome</strong>!", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestNoNestingInsideUrlTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new UrlTagDefinition() });
			RenderResults renderResults = renderer.Render(@"Wow, [url=http://www.google.com][b]this site[/b][/url] is really [b]awesome[/b]!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("Wow, <a href=\"http://www.google.com\">[b]this site[/b]</a> is really <strong>awesome</strong>!", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestAuthorlessQuoteTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new QuoteTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[quote]Light a man a fire, keep him warm for a night. Light a man on fire, keep him warm for a lifetime[/quote][b]LOL[/b]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<blockquote><p>Light a man a fire, keep him warm for a night. Light a man on fire, keep him warm for a lifetime</p></blockquote><strong>LOL</strong>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestAuthorQuoteTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new QuoteTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[quote=""Some Guy""]Light a man a fire, keep him warm for a night. Light a man on fire, keep him warm for a lifetime[/quote][b]LOL[/b]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<blockquote><p class=\"QuoteAuthor\">Some Guy wrote:</p><p>Light a man a fire, keep him warm for a night. Light a man on fire, keep him warm for a lifetime</p></blockquote><strong>LOL</strong>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestNoNestingQuoteInOtherTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new QuoteTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[quote=""Some Guy""]Light a man a fire, keep him warm for a night. Light a man on fire, keep him warm for a lifetime[/quote][b][quote]LOL[/quote][/b]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<blockquote><p class=\"QuoteAuthor\">Some Guy wrote:</p><p>Light a man a fire, keep him warm for a night. Light a man on fire, keep him warm for a lifetime</p></blockquote><strong>[quote]LOL[/quote]</strong>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestNestingQuoteTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new QuoteTagDefinition() });
			RenderResults renderResults = renderer.Render("[quote=\"Some Guy\"]\t [quote]Give me a quote[/quote][quote][b]No[/b][/quote]Here is one:[quote=\"Minna Thomas Antrim\"]Experience is a good teacher, but she send in terrific bills.[/quote]Like it?[/quote][b]LOL[/b]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<blockquote><p class=\"QuoteAuthor\">Some Guy wrote:</p>\t <blockquote><p>Give me a quote</p></blockquote><blockquote><p><strong>No</strong></p></blockquote><p>Here is one:</p><blockquote><p class=\"QuoteAuthor\">Minna Thomas Antrim wrote:</p><p>Experience is a good teacher, but she send in terrific bills.</p></blockquote><p>Like it?</p></blockquote><strong>LOL</strong>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestQuoteTagsAndEscapeTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new QuoteTagDefinition(), new EscapeTagDefinition() });
			RenderResults renderResults = renderer.Render("[quote=\"Some Guy\"]\t [quote]Give me a quote[/quote][esc][quote][b]No[/b][/quote][/esc]Here is one:[quote=\"Minna Thomas Antrim\"]Experience is a good teacher, but she send in terrific bills.[/quote]Like it?[/quote][b]LOL[/b]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<blockquote><p class=\"QuoteAuthor\">Some Guy wrote:</p>\t <blockquote><p>Give me a quote</p></blockquote><p>[quote][b]No[/b][/quote]Here is one:</p><blockquote><p class=\"QuoteAuthor\">Minna Thomas Antrim wrote:</p><p>Experience is a good teacher, but she send in terrific bills.</p></blockquote><p>Like it?</p></blockquote><strong>LOL</strong>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestTwoNonNestedQuoteTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new QuoteTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[quote]Quote1[/quote] [b]text[/b] [quote]Quote2[/quote]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<blockquote><p>Quote1</p></blockquote> <strong>text</strong> <blockquote><p>Quote2</p></blockquote>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestHtmlEscaping()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render(@"<h1>[b]Wow[/b]</h1>, this is [i]<h1>really</h1>[/i] [b]<h1>awe</h1>some[/b]<h1>!</h1>");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"&lt;h1&gt;<strong>Wow</strong>&lt;/h1&gt;, this is <em>&lt;h1&gt;really&lt;/h1&gt;</em> <strong>&lt;h1&gt;awe&lt;/h1&gt;some</strong>&lt;h1&gt;!&lt;/h1&gt;", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestEscapedHtmlInQuoteAuthor()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new QuoteTagDefinition() });
			RenderResults renderResults = renderer.Render(@"[quote=""<h1>Some Guy</h1>""]Light a man a fire, keep him warm for a night. Light a man on fire, keep him warm for a lifetime[/quote][b]LOL[/b]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<blockquote><p class=\"QuoteAuthor\">&lt;h1&gt;Some Guy&lt;/h1&gt; wrote:</p><p>Light a man a fire, keep him warm for a night. Light a man on fire, keep him warm for a lifetime</p></blockquote><strong>LOL</strong>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestBrInsertion()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render("[b]Wow\r\n[/b], this is\r\n [i]really[/i] \r\n[b]awesome[/b]!");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"<strong>Wow<br/></strong>, this is<br/> <em>really</em> <br/><strong>awesome</strong>!", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestSensibleBrHandlingAroundQuoteTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new QuoteTagDefinition() });
			RenderResults renderResults = renderer.Render("Before quote1\r\n\r\n[quote]\r\nquote1\r\n[/quote]\r\n\r\nAfter quote1\r\nBefore quote2\r\n[quote]\r\n\r\nquote2\r\n\r\n[/quote]\r\nAfter quote2\r\nBefore quote3[quote]quote3[/quote]After quote3\r\nBefore quote4\r\n\r\n\r\n[quote]quote4[/quote]\r\n\r\n\r\nAfter quote4");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"Before quote1<br/><blockquote><p>quote1</p></blockquote>After quote1<br/>Before quote2<blockquote><p><br/>quote2<br/></p></blockquote>After quote2<br/>Before quote3<blockquote><p>quote3</p></blockquote>After quote3<br/>Before quote4<br/><br/><blockquote><p>quote4</p></blockquote><br/>After quote4", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestSensibleBrHandlingAroundQuoteTagsWithExtraWhitespace()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new QuoteTagDefinition() });
			RenderResults renderResults = renderer.Render("Before quote1 \r\n \r\n [quote] \r\n quote1 \r\n [/quote] \r\n \r\n After quote1\r\nBefore quote2 \r\n [quote] \r\n \r\n quote2 \r\n \r\n [/quote] \r\n After quote2\r\nBefore quote3 [quote] quote3 [/quote] After quote3\r\nBefore quote4 \r\n \r\n \r\n [quote]quote4[/quote] \r\n \r\n \r\n After quote4");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"Before quote1 <br/> <blockquote><p> quote1 </p></blockquote> After quote1<br/>Before quote2 <blockquote><p> <br/> quote2 <br/> </p></blockquote> After quote2<br/>Before quote3 <blockquote><p> quote3 </p></blockquote> After quote3<br/>Before quote4 <br/> <br/> <blockquote><p>quote4</p></blockquote> <br/> After quote4", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestOnlyValidUrlsAllowedInUrlTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new UrlTagDefinition() });
			RenderResults renderResults = renderer.Render("[url=javascript:alert('hax!')]Link[/url] [url]javascript:alert('hax!')[/url] [url]<h1>test</h1>[/url]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[url=javascript:alert(&#39;hax!&#39;)]Link[/url] [url]javascript:alert(&#39;hax!&#39;)[/url] [url]&lt;h1&gt;test&lt;/h1&gt;[/url]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestValidColourTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ColourTagDefinition() });
			RenderResults renderResults = renderer.Render("[colour=red][b]red text[/b][/colour] [color=#00FF00]green text[/color]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<span style=\"color:red\"><strong>red text</strong></span> <span style=\"color:#00FF00\">green text</span>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestInvalidColourTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ColourTagDefinition() });
			RenderResults renderResults = renderer.Render("[colour=blerg][b]red text[/b][/colour] [color=#00HH00]green text[/color] [color]test[/color]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[colour=blerg]<strong>red text</strong>[/colour] [color=#00HH00]green text[/color] [color]test[/color]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestColourTagNoSelfNesting()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new BoldTagDefinition(), new ColourTagDefinition() });
			RenderResults renderResults = renderer.Render("[color=#00FF00][colour=blue]blue[/colour] text[/color]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<span style=\"color:#00FF00\">[colour=blue]blue</span> text[/color]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestStrikethroughAndUnderline()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new StrikethroughTagDefinition(), new UnderlineTagDefinition() });
			RenderResults renderResults = renderer.Render("[s]I did say [u]this[/u][/s] now I'm saying [u]this[/u]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<span style=\"text-decoration:line-through\">I did say <span style=\"text-decoration:underline\">this</span></span> now I&#39;m saying <span style=\"text-decoration:underline\">this</span>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestStrikethroughAndUnderlineNoSelfNesting()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new StrikethroughTagDefinition(), new UnderlineTagDefinition() });
			RenderResults renderResults = renderer.Render("[s][s]I did say [u]this[/u][/s][/s] now I'm saying [u][u]this[/u][/u]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<span style=\"text-decoration:line-through\">[s]I did say <span style=\"text-decoration:underline\">this</span></span>[/s] now I&#39;m saying <span style=\"text-decoration:underline\">[u]this</span>[/u]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestCodeTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new CodeTagDefinition(), new StrikethroughTagDefinition() });
			RenderResults renderResults = renderer.Render("[code]I did say [s]this[/s][/code] now I'm saying [code]this[/code]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<pre>I did say <span style=\"text-decoration:line-through\">this</span></pre> now I&#39;m saying <pre>this</pre>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestCodeTagNoSelfNesting()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new CodeTagDefinition(), new StrikethroughTagDefinition() });
			RenderResults renderResults = renderer.Render("[code]I [code]did say[/code] [s]this[/s][/code] now I'm saying [code]this[/code]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<pre>I [code]did say</pre> <span style=\"text-decoration:line-through\">this</span>[/code] now I&#39;m saying <pre>this</pre>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestSensibleBrHandlingAroundCodeTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new CodeTagDefinition() });
			RenderResults renderResults = renderer.Render("Before code1\r\n\r\n[code]\r\ncode1\r\n[/code]\r\n\r\nAfter code1\r\nBefore code2\r\n[code]\r\n\r\ncode2\r\n\r\n[/code]\r\nAfter code2\r\nBefore code3[code]code3[/code]After code3\r\nBefore code4\r\n\r\n\r\n[code]code4[/code]\r\n\r\n\r\nAfter code4");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"Before code1<br/><pre>code1</pre>After code1<br/>Before code2<pre><br/>code2<br/></pre>After code2<br/>Before code3<pre>code3</pre>After code3<br/>Before code4<br/><br/><pre>code4</pre><br/>After code4", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestSensibleBrHandlingAroundCodeTagsWithExtraWhitespace()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new CodeTagDefinition() });
			RenderResults renderResults = renderer.Render("Before code1 \r\n \r\n [code] \r\n code1 \r\n [/code] \r\n \r\n After code1\r\nBefore code2 \r\n [code] \r\n \r\n code2 \r\n \r\n [/code] \r\n After code2\r\nBefore code3 [code] code3 [/code] After code3\r\nBefore code4 \r\n \r\n \r\n [code]code4[/code] \r\n \r\n \r\n After code4");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"Before code1 <br/> <pre> code1 </pre> After code1<br/>Before code2 <pre> <br/> code2 <br/> </pre> After code2<br/>Before code3 <pre> code3 </pre> After code3<br/>Before code4 <br/> <br/> <pre>code4</pre> <br/> After code4", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestSizeTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new SizeTagDefinition(), new StrikethroughTagDefinition() });
			RenderResults renderResults = renderer.Render("[size=10]I did say [s]this[/s][/size] now I'm saying [size=200]this[/size]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<span style=\"font-size:10%\">I did say <span style=\"text-decoration:line-through\">this</span></span> now I&#39;m saying <span style=\"font-size:200%\">this</span>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestSizeTagMustNotSelfNest()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new SizeTagDefinition(), new StrikethroughTagDefinition() });
			RenderResults renderResults = renderer.Render("[size=10]I did [size=50]say[/size] [s]this[/s][/size] now I'm saying [size=200]this[/size]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<span style=\"font-size:10%\">I did [size=50]say</span> <span style=\"text-decoration:line-through\">this</span>[/size] now I&#39;m saying <span style=\"font-size:200%\">this</span>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestSizeTagWithInvalidSizes()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new SizeTagDefinition(), new StrikethroughTagDefinition() });
			RenderResults renderResults = renderer.Render("[size=0]I did say [s]this[/s][/size] [size=-1]now[/size] [size=a]I'm[/size] saying [size=201]this[/size]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[size=0]I did say <span style=\"text-decoration:line-through\">this</span>[/size] [size=-1]now[/size] [size=a]I&#39;m[/size] saying [size=201]this[/size]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestImageTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ImageTagDefinition(), new StrikethroughTagDefinition() });
			RenderResults renderResults = renderer.Render("[img]http://www.digitallycreated.net/Content/Images/OpenID-Small.png[/img] [img=20,10]http://www.digitallycreated.net/Content/Images/OpenID-Small.png[/img]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<img src=\"http://www.digitallycreated.net/Content/Images/OpenID-Small.png\" alt=\"\" /> <img src=\"http://www.digitallycreated.net/Content/Images/OpenID-Small.png\" width=\"20\" height=\"10\" alt=\"\" />", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestMustHaveValidUrlInImageTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ImageTagDefinition(), new StrikethroughTagDefinition() });
			RenderResults renderResults = renderer.Render("[img]javascript:alert('hax!')[/img] [img=20,10]javascript:alert('hax!')[/img]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[img]javascript:alert(&#39;hax!&#39;)[/img] [img=20,10]javascript:alert(&#39;hax!&#39;)[/img]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestInvalidSizesInImageTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ImageTagDefinition(), new StrikethroughTagDefinition() });
			RenderResults renderResults = renderer.Render("[img=10,b]http://www.digitallycreated.net/Content/Images/OpenID-Small.png[/img] [img=a,10]http://www.digitallycreated.net/Content/Images/OpenID-Small.png[/img]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[img=10,b]http://www.digitallycreated.net/Content/Images/OpenID-Small.png[/img] [img=a,10]http://www.digitallycreated.net/Content/Images/OpenID-Small.png[/img]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestNoNestingInsideImageTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ImageTagDefinition(), new StrikethroughTagDefinition() });
			RenderResults renderResults = renderer.Render("[img]http://www.digitallycreated.net/[s]Content/Images/OpenID-Small.png[/s][/img]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[img]http://www.digitallycreated.net/[s]Content/Images/OpenID-Small.png[/s][/img]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestFlashTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new FlashTagDefinition() });
			RenderResults renderResults = renderer.Render("[flash=480,360]http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id=1931004&fullscreen=1[/flash]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<object type=\"application/x-shockwave-flash\" style=\"width:480px; height:360px;\" data=\"http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id=1931004&fullscreen=1\"><param name=\"movie\" value=\"http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id=1931004&fullscreen=1\" /><param name=\"allowfullscreen\" value=\"true\" /></object>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestMustHaveValidUrlInFlashTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new FlashTagDefinition() });
			RenderResults renderResults = renderer.Render("[flash=480,360]javascript:alert('hax!')[/flash]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[flash=480,360]javascript:alert(&#39;hax!&#39;)[/flash]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestInvalidSizesInFlashTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new FlashTagDefinition() });
			RenderResults renderResults = renderer.Render("[flash=a,360]http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id=1931004&fullscreen=1[/flash] [flash=480,b]http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id=1931004&fullscreen=1[/flash]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[flash=a,360]http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id=1931004&amp;fullscreen=1[/flash] [flash=480,b]http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id=1931004&amp;fullscreen=1[/flash]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestNoNestingInsideFlashTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new FlashTagDefinition(), new StrikethroughTagDefinition() });
			RenderResults renderResults = renderer.Render("[flash=480,360]http://www.collegehumor.com/[s]moogaloop/moogaloop.swf?clip_id=1931004&fullscreen=1[/s][/flash]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[flash=480,360]http://www.collegehumor.com/[s]moogaloop/moogaloop.swf?clip_id=1931004&amp;fullscreen=1[/s][/flash]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestUnorderedListTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ListTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render("[list]First[*]Second[*]Third[/list] [list=u][*]First[*]Second[/list] [list=unordered]First[/list]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<ul><li>First</li><li>Second</li><li>Third</li></ul> <ul><li>First</li><li>Second</li></ul> <ul><li>First</li></ul>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestOrderedListTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ListTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render("[list=o] \r\n\t[*]First[*]Second[/list] [list=o][*]First[*]Second[/list] [list=ordered]First[/list]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<ol><li>First</li><li>Second</li></ol> <ol><li>First</li><li>Second</li></ol> <ol><li>First</li></ol>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestEmptyListTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ListTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render("[list][/list] [list] \t\r\n[/list]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[list][/list] [list] \t<br/>[/list]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestListTagNesting()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ListTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render("[list]\r\n[*][i]1[/i]\r\n[list]\r\n[*]1.1\r\n[/list]\r\n[/list]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<ul><li><em>1</em><ul><li>1.1</li></ul></li></ul>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestListTagMustNotNestInInlineElements()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ListTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render("[i][list][*]Bullet[/list][/i]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<em>[list][*]Bullet[/list]</em>", renderResults.RenderedString);
		}


		[TestMethod]
		public void TestSensibleBrHandlingAroundListTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ListTagDefinition() });
			RenderResults renderResults = renderer.Render("Before list1\r\n\r\n[list]\r\n[*]list1\r\n[/list]\r\n\r\nAfter list1\r\nBefore list2\r\n[list]\r\n\r\n[*]list2\r\n\r\n[/list]\r\nAfter list2\r\nBefore list3[list]list3[/list]After list3\r\nBefore list4\r\n\r\n\r\n[list]list4[/list]\r\n\r\n\r\nAfter list4");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"Before list1<br/><ul><li>list1</li></ul>After list1<br/>Before list2<ul><li>list2<br/></li></ul>After list2<br/>Before list3<ul><li>list3</li></ul>After list3<br/>Before list4<br/><br/><ul><li>list4</li></ul><br/>After list4", renderResults.RenderedString);
		}


		[TestMethod]
		public void TestSensibleBrHandlingAroundListTagsWithExtraWhitespace()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ListTagDefinition() });
			RenderResults renderResults = renderer.Render("Before list1 \r\n \r\n [list] \r\n [*] list1 \r\n [/list] \r\n \r\n After list1\r\nBefore list2 \r\n [list] \r\n \r\n [*]list2 \r\n \r\n [/list] \r\n After list2\r\nBefore list3 [list] list3 [/list] After list3\r\nBefore list4 \r\n \r\n \r\n [list]list4[/list] \r\n \r\n \r\n After list4");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual(@"Before list1 <br/> <ul><li> list1 </li></ul> After list1<br/>Before list2 <ul><li>list2 <br/> </li></ul> After list2<br/>Before list3 <ul><li> list3 </li></ul> After list3<br/>Before list4 <br/> <br/> <ul><li>list4</li></ul> <br/> After list4", renderResults.RenderedString);
		}


		[TestMethod]
		public void TestAlignTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new LeftAlignTagDefinition(), new CentreAlignTagDefinition(), new RightAlignTagDefinition() });
			RenderResults renderResults = renderer.Render("[left]left[/left] [centre]centre[/centre] [center]center[/center] [right]right[/right]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<div style=\"text-align:left\">left</div> <div style=\"text-align:center\">centre</div> <div style=\"text-align:center\">center</div> <div style=\"text-align:right\">right</div>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestAlignTagsMustNotNestInInlineElements()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new LeftAlignTagDefinition(), new CentreAlignTagDefinition(), new RightAlignTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render("[i][left]left[/left] [centre]centre[/centre] [right]right[/right][/i]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<em>[left]left[/left] [centre]centre[/centre] [right]right[/right]</em>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestAlignTagsMustNotNestInEachOther()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new LeftAlignTagDefinition(), new CentreAlignTagDefinition(), new RightAlignTagDefinition(), new ItalicsTagDefinition() });
			RenderResults renderResults = renderer.Render("[left][centre]centre[/centre] [right]right[/right] [left]left[/left][/left] [centre][left]left[/left] [right]right[/right] [centre]centre[/centre][/centre] [right][left]left[/left] [centre]centre[/centre] [right]right[/right][/right]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<div style=\"text-align:left\">[centre]centre[/centre] [right]right[/right] [left]left</div>[/left] <div style=\"text-align:center\">[left]left[/left] [right]right[/right] [centre]centre</div>[/centre] <div style=\"text-align:right\">[left]left[/left] [centre]centre[/centre] [right]right</div>[/right]", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestSingleSimpleQuoteTagNoPrecedingText()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new QuoteTagDefinition() });
			RenderResults renderResults = renderer.Render("[quote]\r\nTest\r\n[/quote]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<blockquote><p>Test</p></blockquote>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestCodeTagInAQuoteTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new QuoteTagDefinition(), new CodeTagDefinition(), });
			RenderResults renderResults = renderer.Render("[quote]\r\nTest\r\n\r\n[code]\r\nmy code\r\n[/code]\r\n[/quote]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<blockquote><p>Test<br/></p><pre>my code</pre></blockquote>", renderResults.RenderedString);
		}


		[TestMethod]
		public void TestUrlTagWithNoUrlAfterEquals()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new UrlTagDefinition() });
			RenderResults renderResults = renderer.Render("[url=]Test[/url]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[url=]Test[/url]", renderResults.RenderedString);
		}


		[TestMethod]
		public void TestSensibleBrHandlingWithNestedQuoteTags()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new QuoteTagDefinition() });
			RenderResults renderResults = renderer.Render("[quote]\r\n[quote]\r\nInner\r\n[/quote]\r\nOuter\r\n[/quote]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<blockquote><blockquote><p>Inner</p></blockquote><p>Outer</p></blockquote>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestNestImageTagInUrlTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new ImageTagDefinition(), new UrlTagDefinition() });
			RenderResults renderResults = renderer.Render("[url=http://www.google.com][img]http://www.google.com/logo.png[/img][/url]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<a href=\"http://www.google.com\"><img src=\"http://www.google.com/logo.png\" alt=\"\" /></a>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestPercentageEncodedUrlUsageInFlash()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new FlashTagDefinition() });
			RenderResults renderResults = renderer.Render("[flash=480,300]http://blip.tv/play/AYH7%2BysC[/flash]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("<object type=\"application/x-shockwave-flash\" style=\"width:480px; height:300px;\" data=\"http://blip.tv/play/AYH7%2BysC\"><param name=\"movie\" value=\"http://blip.tv/play/AYH7%2BysC\" /><param name=\"allowfullscreen\" value=\"true\" /></object>", renderResults.RenderedString);
		}

		[TestMethod]
		public void TestUnclosedUrlStartTag()
		{
			BbCodeRenderer renderer = new BbCodeRenderer(new ITagDefinition[] { new UrlTagDefinition() });
			RenderResults renderResults = renderer.Render("[url=http://www.google.comGoogle[/url]");
			Assert.IsTrue(renderResults.IsCacheable);
			Assert.AreEqual("[url=http://www.google.comGoogle[/url][/url]", renderResults.RenderedString);
		}
	}
}